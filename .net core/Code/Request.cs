using post_service.Models;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;

namespace post_service.Code
{
    /// <summary>
    /// Реализация запросов к Почте России
    /// </summary>
    static class Request
    {
        /// <summary>
        /// Получение информации о конкретном отправлении
        /// </summary>
        /// <param name="barcode">Код отправления</param>
        /// <param name="auth">Данные для авторизации</param>
        /// <returns>Информация о конкретном отправлении</returns>
        public static List<Operation> getOperationHistory(Barcode barcode, AuthInfo auth)
        {
            //Тело запроса
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

            //Конвертация запроса в необходимую кодировку
            byte[] byteUnicode = Encoding.Unicode.GetBytes(textRequest);
            byte[] byteUTF8 = Encoding.Convert(Encoding.Unicode, Encoding.UTF8, byteUnicode);
            textRequest = Encoding.UTF8.GetString(byteUTF8);

            //Заголовок запроса
            string _url = @"https://tracking.russianpost.ru/rtm34"; //url сервиса
            _url = _url.Trim('/').Trim('\\'); //удаление слеша в конце адреса
            WebRequest _request = HttpWebRequest.Create(_url);
            _request.Method = "POST";
            _request.ContentType = "application/soap+xml;charset=UTF-8";
            _request.ContentLength = textRequest.Length;
            
            //Отправка запроса
            StreamWriter _streamWriter = new StreamWriter(_request.GetRequestStream());
            _streamWriter.Write(textRequest);
            _streamWriter.Close();

            //Получение ответа
            WebResponse _response = _request.GetResponse();
            StreamReader _streamReader = new StreamReader(_response.GetResponseStream());
            XmlDocument document = new XmlDocument();
            document.LoadXml(_streamReader.ReadToEnd());

            //Разбор XML
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
        public static Ticket getTicket(List<Barcode> barcodes, AuthInfo auth)
        {
            //Формирование xml-списка с ШПИ
            string xmlBarcodes = "";
            foreach (Barcode barcode in barcodes)
            {
                xmlBarcodes += $@"<fcl:Item Barcode = ""{barcode.Code}""/>";
            }

            //Тело запроса
            string textRequest =
                $@"<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" 
                    xmlns:pos=""http://fclient.russianpost.org/postserver""
                    xmlns:fcl=""http://fclient.russianpost.org"">
                    <soapenv:Header/>
                    <soapenv:Body>
                        <pos:ticketRequest>
                            <request>
                                {xmlBarcodes}
                            </request>
                            <login>{auth.Login}</login>
                            <password>{auth.Password}</password>
                            <language>RUS</language>
                        </pos:ticketRequest>
                    </soapenv:Body>
                </soapenv:Envelope>";

            //Конвертация запроса в необходимую кодировку
            byte[] byteUnicode = Encoding.Unicode.GetBytes(textRequest);
            byte[] byteUTF8 = Encoding.Convert(Encoding.Unicode, Encoding.UTF8, byteUnicode);
            textRequest = Encoding.UTF8.GetString(byteUTF8);

            //Заголовок запроса
            string _url = @"https://tracking.russianpost.ru/fc"; //url сервиса
            _url = _url.Trim('/').Trim('\\'); //удаление слеша в конце адреса
            WebRequest _request = HttpWebRequest.Create(_url);
            _request.Method = "POST";
            _request.ContentType = "text/xml;charset=UTF-8";
            _request.ContentLength = textRequest.Length;

            //Отправка запроса
            StreamWriter _streamWriter = new StreamWriter(_request.GetRequestStream());
            _streamWriter.Write(textRequest);
            _streamWriter.Close();

            //Получение ответа
            WebResponse _response = _request.GetResponse();
            StreamReader _streamReader = new StreamReader(_response.GetResponseStream());
            XmlDocument document = new XmlDocument();
            document.LoadXml(_streamReader.ReadToEnd());

            //Создание билета, хранящего данные из XML
            Ticket ticket = new Ticket(document.DocumentElement.InnerText);
            return ticket;
        }

        /// <summary>
        /// Получение информации об отправлениях по ранее полученному билету
        /// </summary>
        /// <param name="ticket">Билет на подготовку информации</param>
        /// <param name="auth">Данные для авторизации</param>
        /// <returns>Информация об отправлениях из списка</returns>
        public static List<Item> getResponseByTicket(Ticket ticket, AuthInfo auth)
        {
            //Тело запроса
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

            //Конвертация запроса в необходимую кодировку
            byte[] byteUnicode = Encoding.Unicode.GetBytes(textRequest);
            byte[] byteUTF8 = Encoding.Convert(Encoding.Unicode, Encoding.UTF8, byteUnicode);
            textRequest = Encoding.UTF8.GetString(byteUTF8);

            //Заголовок запроса
            string _url = @"https://tracking.russianpost.ru/fc"; //url сервиса
            _url = _url.Trim('/').Trim('\\'); //удаление слеша в конце адреса
            WebRequest _request = HttpWebRequest.Create(_url);
            _request.Method = "POST";
            _request.ContentType = "text/xml;charset=UTF-8";
            _request.ContentLength = textRequest.Length;

            //Отправка запроса
            StreamWriter _streamWriter = new StreamWriter(_request.GetRequestStream());
            _streamWriter.Write(textRequest);
            _streamWriter.Close();

            //Получение ответа
            WebResponse _response = _request.GetResponse();
            StreamReader _streamReader = new StreamReader(_response.GetResponseStream());
            XmlDocument document = new XmlDocument();
            document.LoadXml(_streamReader.ReadToEnd());

            //Разбор XML
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
    }
}
