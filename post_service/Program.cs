using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Xml;
using post_service.Models;

namespace post_service
{
    class Program
    {
        /// <summary>
        /// Получение информации о конкретном отправлении
        /// </summary>
        /// <param name="barcode">Код отправления</param>
        /// <param name="auth">Данные для авторизации</param>
        /// <returns>Информация о конкретном отправлении</returns>
        private static List<Operation> getOperationHistory(Barcode barcode, AuthInfo auth)
        {
            string textRequest =
                $@"<soap:Envelope xmlns:soap=""http://www.w3.org/2003/05/soap-envelope""
                    xmlns:oper=""http://russianpost.org/operationhistory""
                    xmlns:data=""http://russianpost.org/operationhistory/data""
                    xmlns:ns1=""http://schemas.xmlsoap.org/soap/envelope/"">
                    <soap:Header/>
                    <soap:Body>
                        <oper:getOperationHistory>
                        <!--Optional:-->
                            <data:OperationHistoryRequest>
                                <data:Barcode>{barcode.Code}</data:Barcode>
                                <data:MessageType>0</data:MessageType>
                                <data:Language>RUS</data:Language>
                            </data:OperationHistoryRequest>
                            <data:AuthorizationHeader>
                                <data:login>{auth.Login}</data:login>
                                <data:password>{auth.Password}</data:password>
                            </data:AuthorizationHeader>
                        </oper:getOperationHistory>
                    </soap:Body>
                </soap:Envelope>";
            byte[] byteUnicode = Encoding.Unicode.GetBytes(textRequest);
            byte[] byteUTF8 = Encoding.Convert(Encoding.Unicode, Encoding.UTF8, byteUnicode);
            textRequest = Encoding.UTF8.GetString(byteUTF8);

            string _url = @"https://tracking.russianpost.ru/rtm34"; // URL SOAP API-сервиса
            //string _method = @"getOperationHistory​"; // Метод, который вызывается на API сервисе
            _url = _url.Trim('/').Trim('\\'); // в конце адреса удалить слэш, если он имеется

            WebRequest _request = HttpWebRequest.Create(_url);
            //все эти настройки можно взять со страницы описания веб-сервиса
            _request.Method = "POST";
            _request.ContentType = "application/soap+xml;charset=UTF-8";
            _request.ContentLength = textRequest.Length;
            //_request.Headers.Add("SOAPAction", _url + @"/" + _method);

            // пишем тело
            StreamWriter _streamWriter = new StreamWriter(_request.GetRequestStream());
            _streamWriter.Write(textRequest);
            _streamWriter.Close();

            // читаем тело
            WebResponse _response = _request.GetResponse();
            StreamReader _streamReader = new StreamReader(_response.GetResponseStream());
            
            //Разбор XML
            XmlDocument document = new XmlDocument();
            document.LoadXml(_streamReader.ReadToEnd());
            XmlNode envelope = document.DocumentElement;
            XmlNode body = envelope.FirstChild;
            XmlNode getOperationHistoryResponse = body.FirstChild;
            XmlNode operationHistoryData = getOperationHistoryResponse.FirstChild;

            //Создание объектов, хранящих данные из XML
            List<Operation> operations = new List<Operation>();
            foreach (XmlNode historyRecord in operationHistoryData)
            {
                Operation operation = new Operation(historyRecord);
                operations.Add(operation);
            }
            return operations;
        }

        /// <summary>
        /// Получение билета на подготовку информации по списку идентификаторов отправлений
        /// </summary>
        /// <param name="barcodes">Список идентификаторов отправлений</param>
        /// <param name="auth">Данные для авторизации</param>
        /// <returns>Билет на подготовку информации</returns>
        private static Ticket getTicket(List<Barcode> barcodes, AuthInfo auth)
        {
            //Переименовать переменные barcodes - их легко спутать
            string strBarcodes = "";
            foreach(Barcode barcode in barcodes)
            {
                strBarcodes += $@"<fcl:Item Barcode = ""{barcode.Code}""/>";
            }
            string textRequest =
                $@"<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" 
                    xmlns:pos=""http://fclient.russianpost.org/postserver""
                    xmlns:fcl=""http://fclient.russianpost.org"">
                    <soapenv:Header/>
                    <soapenv:Body>
                        <pos:ticketRequest>
                            <request>
                                {strBarcodes}
                            </request>
                            <login>{auth.Login}</login>
                            <password>{auth.Password}</password>
                            <language>RUS</language>
                        </pos:ticketRequest>
                    </soapenv:Body>
                </soapenv:Envelope>";
            byte[] byteUnicode = Encoding.Unicode.GetBytes(textRequest);
            byte[] byteUTF8 = Encoding.Convert(Encoding.Unicode, Encoding.UTF8, byteUnicode);
            textRequest = Encoding.UTF8.GetString(byteUTF8);

            string _url = @"https://tracking.russianpost.ru/fc"; // URL SOAP API-сервиса
            //string _method = @"getOperationHistory​"; // Метод, который вызывается на API сервисе
            _url = _url.Trim('/').Trim('\\'); // в конце адреса удалить слэш, если он имеется

            WebRequest _request = HttpWebRequest.Create(_url);
            //все эти настройки можно взять со страницы описания веб-сервиса
            _request.Method = "POST";
            _request.ContentType = "text/xml;charset=UTF-8";
            _request.ContentLength = textRequest.Length;
            //_request.Headers.Add("SOAPAction", _url + @"/" + _method);

            // пишем тело
            StreamWriter _streamWriter = new StreamWriter(_request.GetRequestStream());
            _streamWriter.Write(textRequest);
            _streamWriter.Close();

            // читаем тело
            WebResponse _response = _request.GetResponse();
            StreamReader _streamReader = new StreamReader(_response.GetResponseStream());

            XmlDocument document = new XmlDocument();
            document.LoadXml(_streamReader.ReadToEnd());
            Ticket ticket = new Ticket(document.DocumentElement.InnerText);

            return ticket;
        }

        /// <summary>
        /// Получение информации об отправлениях по ранее полученному билету
        /// </summary>
        /// <param name="ticket">Билет на подготовку информации</param>
        /// <param name="auth">Данные для авторизации</param>
        /// <returns>Информация об отправлениях из списка</returns>
        private static List<Item> getResponseByTicket(Ticket ticket, AuthInfo auth)
        {
            string textRequest =
                $@"<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" 
                    xmlns:pos=""http://fclient.russianpost.org/postserver"">
                    <soapenv:Header/>
                    <soapenv:Body>
                        <pos:answerByTicketRequest>
                            <ticket>{ticket.Value}</ticket>
                            <login>{auth.Login}</login>
                            <password>{auth.Password}</password>
                        </pos:answerByTicketRequest>
                    </soapenv:Body>
                </soapenv:Envelope>";
            byte[] byteUnicode = Encoding.Unicode.GetBytes(textRequest);
            byte[] byteUTF8 = Encoding.Convert(Encoding.Unicode, Encoding.UTF8, byteUnicode);
            textRequest = Encoding.UTF8.GetString(byteUTF8);

            string _url = @"https://tracking.russianpost.ru/fc"; // URL SOAP API-сервиса
            //string _method = @"getOperationHistory​"; // Метод, который вызывается на API сервисе
            _url = _url.Trim('/').Trim('\\'); // в конце адреса удалить слэш, если он имеется

            WebRequest _request = HttpWebRequest.Create(_url);
            //все эти настройки можно взять со страницы описания веб-сервиса
            _request.Method = "POST";
            _request.ContentType = "text/xml;charset=UTF-8";
            _request.ContentLength = textRequest.Length;
            //_request.Headers.Add("SOAPAction", _url + @"/" + _method);

            // пишем тело
            StreamWriter _streamWriter = new StreamWriter(_request.GetRequestStream());
            _streamWriter.Write(textRequest);
            _streamWriter.Close();

            // читаем тело
            WebResponse _response = _request.GetResponse();
            StreamReader _streamReader = new StreamReader(_response.GetResponseStream());

            //Разбор XML
            XmlDocument document = new XmlDocument();
            document.LoadXml(_streamReader.ReadToEnd());
            XmlNode envelope = document.DocumentElement;
            XmlNode body = envelope.FirstChild;
            XmlNode answerByTicketResponse = body.FirstChild;
            XmlNode value = answerByTicketResponse.FirstChild;

            //Создание объектов, хранящих данные из XML
            List<Item> items = new List<Item>();
            foreach (XmlNode xmlItem in value)
            {
                Item item = new Item(xmlItem);
                items.Add(item);
            }
            return items;
        }

        static void Main()
        {
            Barcode barcode1 = new Barcode("LO754799163CN");
            Barcode barcode2 = new Barcode("RA644000001RU");
            List<Barcode> barcodes = new List<Barcode>() { barcode1, barcode2 };

            AuthInfo admin = new AuthInfo("EJyiDhijTZDvND", "D12mQ61jAJBS");
            AuthInfo user = new AuthInfo("RbQGQzMkvBLUCc", "GWeCJeA0Cw7s");

            var operations1 = getOperationHistory(barcode1, user);
            var operations2 = getOperationHistory(barcode2, user);

            Ticket ticket = getTicket(barcodes, admin);
            var items = getResponseByTicket(ticket, admin);

            for (int i = 0; i < operations2.Count; i++)
            {
                bool isTrue = false;
                if (operations2[i].AddressParameters.OperationAddress.Index == items[0].operations[i].AddressParameters.OperationAddress.Index)
                    if (operations2[i].OperationParameters.OperType.Id == items[0].operations[i].OperationParameters.OperType.Id)
                        if (operations2[i].OperationParameters.OperType.Name == items[0].operations[i].OperationParameters.OperType.Name)
                            if (operations2[i].OperationParameters.OperAttr.Id == items[0].operations[i].OperationParameters.OperAttr.Id)
                                //if (operations2[i].OperationParameters.OperDate == items[0].operations[i].OperationParameters.OperDate)
                                    isTrue = true;
                if (!isTrue)
                    throw new Exception();
            }

            for (int i = 0; i < operations1.Count; i++)
            {
                bool isTrue = false;
                //if (operations1[i].AddressParameters.OperationAddress.Index == items[1].operations[i].AddressParameters.OperationAddress.Index) //для Китая в одиночном запросе все в описании, а для пакетного - индекс в поле индекса
                    if (operations1[i].OperationParameters.OperType.Id == items[1].operations[i].OperationParameters.OperType.Id)
                        if (operations1[i].OperationParameters.OperType.Name == items[1].operations[i].OperationParameters.OperType.Name)
                            if (operations1[i].OperationParameters.OperAttr.Id == items[1].operations[i].OperationParameters.OperAttr.Id)
                                //if (operations1[i].OperationParameters.OperDate == items[1].operations[i].OperationParameters.OperDate) //разный формат
                                    isTrue = true;
                if (!isTrue)
                    throw new Exception();
            }

            Console.WriteLine("Для завершения работы нажмите Enter...");
            //Console.ReadLine();
        }
    }
}
