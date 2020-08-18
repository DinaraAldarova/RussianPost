using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace post_service.Models
{
    public class Ticket
    {
        public string Value { get; private set; }
        public DateTime DateTime { get; private set; }
        public string Name { get; private set; }

        public Ticket(string value)
        {
            Value = value;
            Name = value.Substring(17);

            int year = Convert.ToInt32(value.Substring(0, 4));
            int month = Convert.ToInt32(value.Substring(4, 2));
            int day = Convert.ToInt32(value.Substring(6, 2));
            int hour = Convert.ToInt32(value.Substring(8, 2));
            int minute = Convert.ToInt32(value.Substring(10, 2));
            int second = Convert.ToInt32(value.Substring(12, 2));
            int millisecond = Convert.ToInt32(value.Substring(14, 3));
            DateTime = new DateTime(year, month, day, hour, minute, second, millisecond);
        }

        public static string TicketToString(Ticket ticket)
        {
            return ticket.Value;
        }

        public static Ticket StringToTicket(string code)
        {
            return new Ticket(code);
        }

        public static void WriteTicketsToFile(List<Ticket> tickets, string path)
        {
            File.WriteAllLines(path, tickets.ConvertAll(new Converter<Ticket, string>(TicketToString)));
        }

        public static List<Ticket> ReadTicketsFromFile(string path)
        {
            List<string> strTickets = new List<string>(File.ReadAllLines(path));
            List<Ticket> tickets = strTickets.ConvertAll(new Converter<string, Ticket>(StringToTicket));
            return tickets;
        }
    }
}
