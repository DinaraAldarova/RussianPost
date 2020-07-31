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
@"<soap:Envelope xmlns:soap=""http://www.w3.org/2003/05/soap-envelope""
xmlns:oper=""http://russianpost.org/operationhistory""
xmlns:data=""http://russianpost.org/operationhistory/data""
xmlns:ns1=""http://schemas.xmlsoap.org/soap/envelope/"">
<soap:Header/>
<soap:Body>
<oper:getOperationHistory>
<!--Optional:-->
<data:OperationHistoryRequest>
<data:Barcode>LO754799163CN</data:Barcode>
<data:MessageType>0</data:MessageType>
<!--Optional:-->
<data:Language>RUS</data:Language>
</data:OperationHistoryRequest>
<!--Optional:-->
<data:AuthorizationHeader>
<data:login>RbQGQzMkvBLUCc</data:login>
<data:password>GWeCJeA0Cw7s</data:password>
</data:AuthorizationHeader>
</oper:getOperationHistory>
</soap:Body>
</soap:Envelope>";
            byte[] byteUnicode = Encoding.Unicode.GetBytes(textRequest);
            byte[] byteUTF8 = Encoding.Convert(Encoding.Unicode, Encoding.UTF8, byteUnicode);
            textRequest = Encoding.UTF8.GetString(byteUTF8);

            //File.WriteAllText(@"D:\GetInfo2​.xml", textRequest);
            
            string _soapEnvelope = File.ReadAllText(@"D:\GetInfo​.xml"); // SOAP-конверт (запрос), который будет отправлен к API
            _soapEnvelope = _soapEnvelope.Replace("INPUTBARCODE", barcode).Replace("INPUTLOGIN", login).Replace("INPUTPASSWORD", password);
            byte[] byteUTF82 = Encoding.UTF8.GetBytes(_soapEnvelope);

            for (int i = 0; i < byteUTF8.Length && i < byteUTF82.Length; i++)
                if (byteUTF8[i] != byteUTF82[i])
                {
                    Console.WriteLine();
                }


            string _url = @"https://tracking.russianpost.ru/rtm34"; // URL SOAP API-сервиса
            string _method = @"getOperationHistory​"; // Метод, который вызывается на API сервисе

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

        static void Main()
        {
            string barcode = "LO754799163CN";
            string login = "RbQGQzMkvBLUCc";
            string password = "GWeCJeA0Cw7s";

            //string _soapEnvelope = File.ReadAllText(@"D:\GetInfo​.xml"); // SOAP-конверт (запрос), который будет отправлен к API

            string _response = getOperationHistory(barcode, login, password); // получаем ответ SOAP сервиса в виде XML

            Console.WriteLine(_response);
            File.WriteAllText(@"D:\Info " + DateTime.Now.ToString().Replace('.', '_').Replace(':', '_') + "​.xml", _response);

            Console.WriteLine("Для завершения работы нажмите Enter...");
            Console.ReadLine();
        }
    }
}
