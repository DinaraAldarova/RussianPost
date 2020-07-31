using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace post_service
{
    public class Functions
    {
        //Переменные для логирования
        private static object sync = new object();
        private static string fullText;

        //Интеграционный модуль ПОЧТА РОССИИ
        public void ParsePost(string SPI)
        {
            var client = new RussianPost.Tracking.SingleAccessClient();
            var rpo = RussianPost.Tracking.Rpo.Default;
            //rpo.MessageType = RussianPost.Tracking.MessageType.HistoryOperations;
            //try
            //{
            rpo.Barcode = SPI;

            var records = client.GetOperationHistory(rpo);


            foreach (var record in records)
            {

                //if (record.OperationParameters.OperType.Name == "Вручение" & (record.OperationParameters.OperAttr.Id == 1 | record.OperationParameters.OperAttr.Id == 2 | record.OperationParameters.OperAttr.Id == 3 | record.OperationParameters.OperAttr.Id == 4 | record.OperationParameters.OperAttr.Id == 5 | record.OperationParameters.OperAttr.Id == 6 | record.OperationParameters.OperAttr.Id == 7 | record.OperationParameters.OperAttr.Id == 8 | record.OperationParameters.OperAttr.Id == 9 | record.OperationParameters.OperAttr.Id == 10 | record.OperationParameters.OperAttr.Id == 11 | record.OperationParameters.OperAttr.Id == 12 | record.OperationParameters.OperAttr.Id == 13 | record.OperationParameters.OperAttr.Id == 14))
                //{
                //    try
                //    {
                //        //Формируем строку и обновляем данные в БД
                //        queryInput = "update main_uin set date_delivery_addressee = '" + record.OperationParameters.OperDate + "' where spi = '" + rpo.Barcode + "'";
                //        inputDataSQL(queryInput);



                //        Console.Write(fullText);

                //        //Прогоняем формулы
                //        //Update_effective_date(inputUIN);
                //        //Console.Write("Формула 1 - ОК; \r\n");
                //        //Update_amount_recover(inputUIN);
                //        //Console.Write("Формула 2 - ОК; \r\n");
                //        //Update_lastday_complaint(inputUIN);
                //        //Console.Write("Формула 3 - ОК; \r\n");
                //    }
                //    catch
                //    {
                //        //Вывод в лог
                //        //fullText = string.Format("[{0:dd.MM.yyy HH:mm:ss.fff}] {1} \r\n", DateTime.Now, "Проблема с ШПИ: " + rpo.Barcode);
                //        //lock (sync)
                //        //{
                //        //    File.AppendAllText(filename, fullText, Encoding.GetEncoding("Windows-1251"));
                //        //}
                //        //Console.Write(fullText);
                //    }
                //}
                //else if (record.OperationParameters.OperType.Name == "Возврат" & record.OperationParameters.OperAttr.Id == 1)
                //    {
                //    try
                //        {
                //            //Формируем строку и обновляем данные в БД
                //            queryInput = "update main_uin set date_delivery_addressee = '" + record.OperationParameters.OperDate + "' where spi = '" + rpo.Barcode + "'";
                //            inputDataSQL(queryInput);


                //            //Вывод в лог
                //            //fullText = string.Format("[{0:dd.MM.yyy HH:mm:ss.fff}] {1} {2} {3}\r\n",
                //            //DateTime.Now, "Успешно обновлено - УИН " + UIN, "ШПИ " + rpo.Barcode, "Дата " + record.OperationParameters.OperDate);
                //            //lock (sync)
                //            //{
                //            //    File.AppendAllText(filename, fullText, Encoding.GetEncoding("Windows-1251"));
                //            //}
                //            //Console.Write(fullText);

                //            //Прогоняем формулы
                //            //Update_effective_date(inputUIN);
                //            //Console.Write("Формула 1 - ОК; \r\n");
                //           // Update_amount_recover(inputUIN);
                //            //Console.Write("Формула 2 - ОК; \r\n");
                //            //Update_lastday_complaint(inputUIN);
                //           // Console.Write("Формула 3 - ОК; \r\n");
                //        }
                //        catch
                //        {
                //            //Вывод в лог
                //            //fullText = string.Format("[{0:dd.MM.yyy HH:mm:ss.fff}] {1} \r\n", DateTime.Now, "Проблема с ШПИ: " + rpo.Barcode);
                //            //lock (sync)
                //            //{
                //            //    File.AppendAllText(filename, fullText, Encoding.GetEncoding("Windows-1251"));
                //           // }
                //           // Console.Write(fullText);
                //        }
                //    }
            }
            //}
            //catch
            //{
            //Вывод в лог
            //fullText = string.Format("[{0:dd.MM.yyy HH:mm:ss.fff}] {1} \r\n", DateTime.Now, "Проблема с ШПИ: " + rpo.Barcode);
            //lock (sync)
            //{
            //    File.AppendAllText(filename, fullText, Encoding.GetEncoding("Windows-1251"));
            //}

            //}
        }
    }

}
