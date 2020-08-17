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
using Newtonsoft.Json;
using System.Threading;

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
            foreach (Barcode barcode in barcodes)
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        private static string getQuery(Item item)
        {
            string query = "";
            if (item.operations.Count > 0)
            {
                Operation lastOperation = item.operations[item.operations.Count - 1];
                if (lastOperation.OperationParameters.OperType.Id == "2")
                //Вручение
                {
                    int operAttr = Convert.ToInt32(lastOperation.OperationParameters.OperAttr.Id);
                    if (operAttr >= 1 && operAttr <= 14)
                    {
                        //преобразовать дату
                        DateTime dateTime = DateTime.ParseExact(lastOperation.OperationParameters.OperDate, "dd.MM.yyyy HH:mm:ss", null);
                        query = "update main_uin set date_delivery_addressee = '"
                                           + dateTime.ToString("yyyy.MM.dd")
                                           + "' where spi = '"
                                           + item.Barcode
                                           + "'";
                    }
                }
                else if (lastOperation.OperationParameters.OperType.Id == "3" && lastOperation.OperationParameters.OperAttr.Id == "1")
                //Возврат, истек срок хранения
                {
                    //преобразовать дату
                    DateTime dateTime = DateTime.ParseExact(lastOperation.OperationParameters.OperDate, "dd.MM.yyyy HH:mm:ss", null);
                    //запрос что, одинаковый???
                    query = "update main_uin set date_delivery_addressee = '"
                                       + dateTime.ToString("yyyy.MM.dd")
                                       + "' where spi = '"
                                       + item.Barcode
                                       + "'";
                }
            }
            return query;
        }

        public static string BarcodeToString(Barcode barcode)
        {
            return barcode.Code;
        }

        public static Barcode StringToBarcode(string code)
        {
            return new Barcode(code);
        }

        public static string TicketToString(Ticket ticket)
        {
            return ticket.Value;
        }

        public static Ticket StringToTicket(string code)
        {
            return new Ticket(code);
        }

        public static int Min(int a, int b)
        {
            return (a < b) ? a : b;
        }

        public static List<Ticket> WriteTickets(AuthInfo auth)
        {
            List<string> strBarcodes = new List<string>(File.ReadAllLines(@"D:\Barcodes.txt"));
            List<Barcode> barcodes = strBarcodes.ConvertAll(new Converter<string, Barcode>(StringToBarcode));
            Logger.Log.Info(string.Format("Получено {0} ШПИ", barcodes.Count));
            List <Ticket> tickets = new List<Ticket>();
            for (int i = 0; i < barcodes.Count; i += 3000)
            {
                tickets.Add(getTicket(
                    barcodes.GetRange(i, Min(barcodes.Count - i, 3000)),
                    auth));
            }
            Logger.Log.Info(string.Format("Получено {0} билетов", tickets.Count));
            //File.WriteAllLines(@"D:\Tickets.txt", tickets.ConvertAll(new Converter<Ticket, string>(TicketToString)));
            return tickets;
        }

        public static List<Item> ReadTickets(List<Ticket> tickets, AuthInfo auth)
        {
            //List<string> strTickets = new List<string>(File.ReadAllLines(@"D:\Tickets.txt"));
            //List<Ticket> tickets = strTickets.ConvertAll(new Converter<string, Ticket>(StringToTicket));
            List<Item> items = new List<Item>();
            foreach (Ticket ticket in tickets)
            {
                List<Item> response = getResponseByTicket(ticket, auth);
                while (response.Exists(x => x.isReady == false))
                {
                    Thread.Sleep(5000);
                    response = getResponseByTicket(ticket, auth);
                }
                items.AddRange(response);
            }
            Logger.Log.Info(string.Format("Получена информация о {0} ШПИ", items.Count));
            return items;
            //File.WriteAllLines(@"D:\Tickets.txt", tickets.ConvertAll(new Converter<Ticket, string>(TicketToString)));
        }

        public static void WriteQueries(List<Item> items)
        {
            List<string> queries = new List<string>();
            foreach (Item item in items)
            {
                string query = getQuery(item);
                if (!string.IsNullOrEmpty(query))
                {
                    queries.Add(query);
                }
            }
            File.WriteAllLines(@"D:\Queries.txt", queries);
        }

        static void Main()
        {
            //Barcode barcode1 = new Barcode("LO754799163CN");
            //Barcode barcode2 = new Barcode("RA644000001RU");
            //List<Barcode> barcodes = new List<Barcode>() { barcode1, barcode2 };

            AuthInfo admin = new AuthInfo("EJyiDhijTZDvND", "D12mQ61jAJBS");
            AuthInfo user = new AuthInfo("RbQGQzMkvBLUCc", "GWeCJeA0Cw7s");
            Logger.InitLogger();
            //var operations1 = getOperationHistory(barcode1, user);
            //var operations2 = getOperationHistory(barcode2, user);

            //Ticket ticket = getTicket(strBarcodes, admin);
            //var items = getResponseByTicket(ticket, admin);

            //WriteTickets(admin);
            //List<Item> items = ReadTickets(admin);
            //WriteQueries(items);

            Console.WriteLine("Для завершения работы нажмите Enter...");
            Console.ReadLine();
        }
    }
}
