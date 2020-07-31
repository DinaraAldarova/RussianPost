using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace post_service
{
    class Program
    {
        //Переменные для логирования
        public static string pathToLog = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Log");
        private static object sync = new object();
        private static string fullText;

        static void Main()
        {
            Functions myFun = new Functions();

            string SPI = "LO754799163CN";//"62007501000225";//"UA914953735HK";//row[0].ToString();
            myFun.ParsePost(SPI);
            Console.Write(fullText);
        }
    }
}
