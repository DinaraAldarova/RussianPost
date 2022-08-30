using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using post_service.Models;
using post_service.Code;

namespace post_service
{
    class Program
    {
        /// <summary>
        /// Получение ШПИ из базы данных
        /// </summary>
        /// <returns>Список ШПИ из БД</returns>
        public static List<Barcode> GetBarcodes()
        {
            //Чтение информации из базы данных
            string querypost = "SELECT main_uin.spi,uin from main_uin where (date_ruling between '01.01.2019' and GETDATE() and date_delivery_addressee is null) or (date_ruling between '01.01.2019' and GETDATE() and date_delivery_addressee = '01.01.0001')";
            DataTable postDT = DB.readDataSQL(querypost);

            //Разбор полученной информации
            List<Barcode> barcodes = new List<Barcode>();
            foreach (DataRow row in postDT.Rows)
            {
                string SPI = row[0].ToString();
                string UIN = row[1].ToString();
                Barcode barcode = new Barcode(SPI, UIN);
                barcodes.Add(barcode);
            }

            Logger.Log.Info(string.Format("Получено {0} ШПИ", barcodes.Count));
            return barcodes;
        }

        /// <summary>
        /// Получение билетов на получение информации по отправлениям
        /// </summary>
        /// <param name="barcodes">Список ШПИ</param>
        /// <param name="auth">Информация для авторизации по API Почты России</param>
        /// <returns>Список билетов на получение информации</returns>
        public static List<Ticket> GetTickets(List<Barcode> barcodes, AuthInfo auth)
        {
            List<Ticket> tickets = new List<Ticket>();

            //За каждый запрос можно отправить не более 3000 ШПИ
            for (int i = 0; i < barcodes.Count; i += 3000)
            {
                tickets.Add(Request.getTicket(
                    barcodes.GetRange(i, Math.Min(barcodes.Count - i, 3000)),
                    auth));
            }

            Logger.Log.Info(string.Format("Получено {0} билетов", tickets.Count));
            return tickets;
        }

        /// <summary>
        /// Получение информации об отправлениях по полученным ранее билетам
        /// </summary>
        /// <param name="tickets">Список билетов на получение информации</param>
        /// <param name="auth">Информация для авторизации по API Почты России</param>
        /// <returns>Информация об отправлениях</returns>
        public static List<Item> GetItems(List<Ticket> tickets, AuthInfo auth)
        {
            List<Item> items = new List<Item>();
            foreach (Ticket ticket in tickets)
            {
                //Рочта России рекомендует запрашивать данные через 15 минут после получения билета и далее с шагом 15 минут
                Thread.Sleep(900000);
                List<Item> response = Request.getResponseByTicket(ticket, auth);
                while (response.Exists(x => x.isReady == false))
                {
                    Thread.Sleep(900000);
                    response = Request.getResponseByTicket(ticket, auth);
                }
                items.AddRange(response);
            }

            Logger.Log.Info(string.Format("Получена информация о {0} ШПИ", items.Count));
            Logger.Log.Info(string.Format("Из них некорректно обработано {0} ШПИ", items.Count(x => x.isCorrect == false)));
            Logger.Log.Info(string.Format("Из них не найдена информация о {0} ШПИ", items.Count(x => x.isExist == false)));
            Logger.Log.Info(string.Format("Корректно обработано {0} ШПИ", items.Count(x => x.isCorrect && x.isExist && x.isReady)));

            //Функция вернет только корректно полученную информацию об отправлениях
            items.RemoveAll(x => (x.isCorrect && x.isExist && x.isReady) == false);
            return items;
        }

        /// <summary>
        /// Получение списка запросов к базе данных на основе полученной информации об отправлениях
        /// </summary>
        /// <param name="items">Информация от отправлениях</param>
        /// <returns>Список запросов к базе данных</returns>
        public static List<string> GetQueries(List<Item> items)
        {
            List<string> queries = new List<string>();
            foreach (Item item in items)
            {
                string query = DB.getQuery(item);
                if (!string.IsNullOrEmpty(query))
                {
                    queries.Add(query);
                }
            }
            return queries;
        }

        static void Main()
        {
            //Данные для авторизации пользователя при работе с API Почты России
            AuthInfo admin = new AuthInfo("EJyiDhijTZDvND", "D12mQ61jAJBS");

            //Запуск логирования
            Logger.InitLogger();
            Logger.Log.Info("Запуск службы");

            //Получение ШПИ из базы данных 
            List<Barcode> barcodes;
            barcodes = GetBarcodes();
            //Barcode.WriteBarcodesToFile(barcodes, @"D:\Barcodes.txt");
            //barcodes = Barcode.ReadBarcodesFromFile(@"D:\Barcodes.txt");

            //Отправка ШПИ Почте России и получение билетов на получение информации
            List<Ticket> tickets;
            tickets = GetTickets(barcodes, admin);
            //Ticket.WriteTicketsToFile(tickets, @"D:\Tickets.txt");
            //tickets = Ticket.ReadTicketsFromFile(@"D:\Tickets.txt");

            //Получение информации об отправлениях по полученным билетам
            List<Item> items = GetItems(tickets, admin);
            //запись в файл не предусмотрена

            //Формирование запросов к базе данных на основе полученной информации об отправлениях
            List<string> queries;
            queries = GetQueries(items);
            //WriteQueriesToFile(queries, @"D:\Queries.txt");
            //queries = ReadQueriesFromFile(@"D:\Queries.txt");

            //Отправка запросов в базу данных
            DB.inputDataSQL(queries);
            Logger.Log.Info($"В базе данных обновлена информация о {queries.Count} ШПИ");
            Logger.Log.Info("Завершение работы службы");

            //Console.WriteLine("Для завершения работы нажмите Enter...");
            //Console.ReadLine();
        }
    }
}
