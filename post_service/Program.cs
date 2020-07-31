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
        /// <param name="_url">URL SOAP API-сервиса</param>
        /// <param name="_method">Метод, который вызывается на API сервисе</param>
        /// <param name="_soapEnvelope">SOAP-конверт (запрос), который будет отправлен к API</param>
        /// <returns>Метод возвращает ответ SOAP сервиса в виде XML</returns>
        private static string GetResponseSoap(string _url, string _method, string _soapEnvelope)
        {
            _url = _url.Trim('/').Trim('\\'); // в конце адреса удалить слэш, если он имеется
            WebRequest _request = HttpWebRequest.Create(_url);
            //все эти настройки можно взять со страницы описания веб-сервиса
            _request.Method = "POST";
            _request.ContentType = "application/soap+xml;charset=UTF-8";
            _request.ContentLength = _soapEnvelope.Length;
            //_request.Headers.Add("SOAPAction", _url + @"/" + _method);
            // пишем тело
            StreamWriter _streamWriter = new StreamWriter(_request.GetRequestStream());
            _streamWriter.Write(_soapEnvelope);
            _streamWriter.Close();
            // читаем тело
            WebResponse _response = _request.GetResponse();
            StreamReader _streamReader = new StreamReader(_response.GetResponseStream());
            string _result = _streamReader.ReadToEnd(); // переменная в которую пишется результат (ответ) сервиса
            return _result;
        }

        static void Main()
        {
            string _url = @"https://tracking.russianpost.ru/rtm34"; // URL SOAP API-сервиса
            string _method = @"getOperationHistory​"; // Метод, который вызывается на API сервисе
            string _soapEnvelope = File.ReadAllText(@"D:\GetInfo​.xml"); // SOAP-конверт (запрос), который будет отправлен к API
            string _response = GetResponseSoap(_url, _method, _soapEnvelope); // получаем ответ SOAP сервиса в виде XML
            Console.WriteLine(_response);
            File.WriteAllText(@"D:\Info " + DateTime.Now.ToString().Replace('.', '_').Replace(':', '_') + "​.xml", _response);

            Console.WriteLine("Для завершения работы нажмите Enter...");
            Console.ReadLine();
        }

        


        ////Переменные для логирования
        //public static string pathToLog = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Log");
        //private static object sync = new object();
        //private static string fullText;

        //static void Main()
        //{
        //    Functions myFun = new Functions();

        //    //Проверяем и формируем файл для логирования
        //    if (!Directory.Exists(pathToLog))
        //        Directory.CreateDirectory(pathToLog); // Создаем директорию, если нужно
        //    string filename = Path.Combine(pathToLog, string.Format("{0}_{1:dd.MM.yyy}_{2:HH.mm.ss}.log", AppDomain.CurrentDomain.FriendlyName, DateTime.Now, DateTime.Now));

        //    //Строка запроса SQL
        //    string querypost = "SELECT main_uin.spi,uin from main_uin where (date_ruling between '01.01.2019' and GETDATE() and date_delivery_addressee is null) or (date_ruling between '01.01.2019' and GETDATE() and date_delivery_addressee = '01.01.0001')";
        //    DataTable postDT = myFun.readDataSQL(querypost);

        //    int count = postDT.Rows.Count;
        //    int j = 0;

        //    //Вывод в лог
        //    fullText = string.Format("[{0:dd.MM.yyy}] {1} \r\n", DateTime.Now, " Количество ШПИ: " + count);
        //    lock (sync)
        //    {
        //        File.AppendAllText(filename, fullText, Encoding.GetEncoding("Windows-1251"));
        //    }


        //    try
        //    {

        //        foreach (DataRow row in postDT.Rows)
        //        {
        //            //Вывод в лог
        //            fullText = string.Format("[{0:dd.MM.yyy HH:mm:ss.fff}] {1} \r\n", DateTime.Now, "Запрос по ШПИ: " + row[0].ToString() + " (" + j + " из " + postDT.Rows.Count + ")");
        //            lock (sync)
        //            {
        //                File.AppendAllText(filename, fullText, Encoding.GetEncoding("Windows-1251"));
        //            }
        //            Console.Write(fullText);


        //            string SPI = row[0].ToString();
        //            string UIN = row[1].ToString();
        //            myFun.ParsePost(SPI, UIN, filename, count);

        //            j++;
        //        }
        //        //Вывод в лог
        //        fullText = string.Format("[{0:dd.MM.yyy HH:mm:ss.fff}] {1} \r\n", DateTime.Now, "Количетсво обработанных ШПИ: " + j);
        //        lock (sync)
        //        {
        //            File.AppendAllText(filename, fullText, Encoding.GetEncoding("Windows-1251"));
        //        }
        //        Console.Write(fullText);
        //    }
        //    catch
        //    {
        //    }

        //}
    }
}
