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
        public static List<Ticket> GetTickets(AuthInfo auth)
        {
            List<string> strBarcodes = new List<string>(File.ReadAllLines(@"D:\Barcodes.txt"));
            List<Barcode> barcodes = strBarcodes.ConvertAll(new Converter<string, Barcode>(Barcode.StringToBarcode));
            Logger.Log.Info(string.Format("Получено {0} ШПИ", barcodes.Count));
            List <Ticket> tickets = new List<Ticket>();
            for (int i = 0; i < barcodes.Count; i += 3000)
            {
                tickets.Add(Request.getTicket(
                    barcodes.GetRange(i, Math.Min(barcodes.Count - i, 3000)),
                    auth));
            }
            Logger.Log.Info(string.Format("Получено {0} билетов", tickets.Count));
            return tickets;
        }

        public static List<Item> ReadItems(List<Ticket> tickets, AuthInfo auth)
        {
            List<Item> items = new List<Item>();
            foreach (Ticket ticket in tickets)
            {
                List<Item> response = Request.getResponseByTicket(ticket, auth);
                while (response.Exists(x => x.isReady == false))
                {
                    Thread.Sleep(5000);
                    response = Request.getResponseByTicket(ticket, auth);
                }
                items.AddRange(response);
            }
            Logger.Log.Info(string.Format("Получена информация о {0} ШПИ", items.Count));
            Logger.Log.Info(string.Format("Из них некорректно обработано {0} ШПИ", items.Count(x => x.isCorrect == false)));
            Logger.Log.Info(string.Format("Из них не найдена информация о {0} ШПИ", items.Count(x => x.isExist == false)));
            Logger.Log.Info(string.Format("Корректно обработано {0} ШПИ", items.Count(x => x.isCorrect && x.isExist && x.isReady)));
            items.RemoveAll(x => (x.isCorrect && x.isExist && x.isReady) == false);
            return items;
        }

        public static void GetQueries(List<Item> items)
        {
            List<string> queries = new List<string>();
            foreach (Item item in items)
            {
                string query = Request.getQuery(item);
                if (!string.IsNullOrEmpty(query))
                {
                    queries.Add(query);
                }
            }
            File.WriteAllLines(@"D:\Queries.txt", queries);
            Functions.inputDataSQL(queries);
        }

        static void Main()
        {
            AuthInfo admin = new AuthInfo("EJyiDhijTZDvND", "D12mQ61jAJBS");
            AuthInfo user = new AuthInfo("RbQGQzMkvBLUCc", "GWeCJeA0Cw7s");
            Logger.InitLogger();

            List<Ticket> tickets;
            //tickets = GetTickets(admin);
            //Ticket.WriteTicketsToFile(tickets, @"D:\Tickets.txt");
            tickets = Ticket.ReadTicketsFromFile(@"D:\Tickets.txt");

            List<Item> items = ReadItems(tickets, admin);
            GetQueries(items);

            Console.WriteLine("Для завершения работы нажмите Enter...");
            Console.ReadLine();
        }
    }
}
