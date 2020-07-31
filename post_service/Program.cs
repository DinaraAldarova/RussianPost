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

            //Проверяем и формируем файл для логирования
            if (!Directory.Exists(pathToLog))
                Directory.CreateDirectory(pathToLog); // Создаем директорию, если нужно
            string filename = Path.Combine(pathToLog, string.Format("{0}_{1:dd.MM.yyy}_{2:HH.mm.ss}.log", AppDomain.CurrentDomain.FriendlyName, DateTime.Now, DateTime.Now));

            //Строка запроса SQL
            string querypost = "SELECT main_uin.spi,uin from main_uin where (date_ruling between '01.01.2019' and GETDATE() and date_delivery_addressee is null) or (date_ruling between '01.01.2019' and GETDATE() and date_delivery_addressee = '01.01.0001')";
            DataTable postDT = myFun.readDataSQL(querypost);

            int count = postDT.Rows.Count;
            int j = 0;

            //Вывод в лог
            fullText = string.Format("[{0:dd.MM.yyy}] {1} \r\n", DateTime.Now, " Количество ШПИ: " + count);
            lock (sync)
            {
                File.AppendAllText(filename, fullText, Encoding.GetEncoding("Windows-1251"));
            }


            try
            {

                foreach (DataRow row in postDT.Rows)
                {
                    //Вывод в лог
                    fullText = string.Format("[{0:dd.MM.yyy HH:mm:ss.fff}] {1} \r\n", DateTime.Now, "Запрос по ШПИ: " + row[0].ToString() + " (" + j + " из " + postDT.Rows.Count + ")");
                    lock (sync)
                    {
                        File.AppendAllText(filename, fullText, Encoding.GetEncoding("Windows-1251"));
                    }
                    Console.Write(fullText);


                    string SPI = row[0].ToString();
                    string UIN = row[1].ToString();
                    myFun.ParsePost(SPI, UIN, filename, count);

                    j++;
                }
                //Вывод в лог
                fullText = string.Format("[{0:dd.MM.yyy HH:mm:ss.fff}] {1} \r\n", DateTime.Now, "Количетсво обработанных ШПИ: " + j);
                lock (sync)
                {
                    File.AppendAllText(filename, fullText, Encoding.GetEncoding("Windows-1251"));
                }
                Console.Write(fullText);
            }
            catch
            {
            }

        }
    }
}
