using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.IO;
using System.Net;
using System.Xml;

namespace post_service
{
    class Program
    {
        /// <summary> Метод отправки запроса к SOAP сервису и получения от него ответа </summary>
        /// <returns>Метод возвращает ответ SOAP сервиса в виде XML</returns>
        private static string getOperationHistory(string barcode, string login, string password)
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
                                <data:Barcode>{barcode}</data:Barcode>
                                <data:MessageType>0</data:MessageType>
                                <data:Language>RUS</data:Language>
                            </data:OperationHistoryRequest>
                            <data:AuthorizationHeader>
                                <data:login>{login}</data:login>
                                <data:password>{password}</data:password>
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
            string _result = _streamReader.ReadToEnd(); // переменная в которую пишется результат (ответ) сервиса
            return _result;
        }

        /// <summary> Метод отправки запроса к SOAP сервису и получения от него ответа </summary>
        /// <returns>Метод возвращает ответ SOAP сервиса в виде XML</returns>
        private static string getTicket(List<string> barcodes, string login, string password)
        {
            //Переименовать переменные barcodes - их легко спутать
            string strBarcodes = @"<fcl:Item Barcode = ""RA644000001RU""/>
                                <fcl:Item Barcode = ""LO754799163CN""/>";
            strBarcodes = "";
            foreach(string barcode in barcodes)
            {
                strBarcodes += $@"<fcl:Item Barcode = ""{barcode}""/>";
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
                            <login>{login}</login>
                            <password>{password}</password>
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
            string _result = _streamReader.ReadToEnd(); // переменная в которую пишется результат (ответ) сервиса
            return _result;
        }

        /// <summary> Метод отправки запроса к SOAP сервису и получения от него ответа </summary>
        /// <returns>Метод возвращает ответ SOAP сервиса в виде XML</returns>
        private static string getResponseByTicket(string ticket, string login, string password)
        {
            string textRequest =
                $@"<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" 
                    xmlns:pos=""http://fclient.russianpost.org/postserver"">
                    <soapenv:Header/>
                    <soapenv:Body>
                        <pos:answerByTicketRequest>
                            <ticket>{ticket}</ticket>
                            <login>{login}</login>
                            <password>{password}</password>
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
            string _result = _streamReader.ReadToEnd(); // переменная в которую пишется результат (ответ) сервиса
            return _result;
        }

        static void Main()
        {
            string barcode = "LO754799163CN";
            List<string> barcodes = new List<string>() { "LO754799163CN", "RA644000001RU" };

            string ticket1 = "20200731152103929EJYIDHIJTZDVND";
            string ticket2 = "20200731104356988EJYIDHIJTZDVND";

            string userLogin = "RbQGQzMkvBLUCc";
            string userPassword = "GWeCJeA0Cw7s";

            string adminLogin = "EJyiDhijTZDvND";
            string adminPassword = "D12mQ61jAJBS";

            string _response;

            _response = getOperationHistory(barcode, userLogin, userPassword); // получаем ответ SOAP сервиса в виде XML
            Console.WriteLine(_response);
            File.WriteAllText(@"D:\Info getOperationHistory " + DateTime.Now.ToString().Replace('.', '_').Replace(':', '_') + "​.xml", _response);

            _response = getTicket(barcodes, adminLogin, adminPassword); // получаем ответ SOAP сервиса в виде XML
            Console.WriteLine(_response);
            File.WriteAllText(@"D:\Info getTicket " + DateTime.Now.ToString().Replace('.', '_').Replace(':', '_') + "​.xml", _response);

            _response = getResponseByTicket(ticket2, adminLogin, adminPassword); // получаем ответ SOAP сервиса в виде XML
            Console.WriteLine(_response);
            File.WriteAllText(@"D:\Info GetResponseByTicket " + DateTime.Now.ToString().Replace('.', '_').Replace(':', '_') + "​.xml", _response);

            Console.WriteLine("Для завершения работы нажмите Enter...");
            Console.ReadLine();
        }
    }
}
