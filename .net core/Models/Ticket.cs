using System;
using System.Text.RegularExpressions;
using post_service.Code;
using Serilog;

namespace post_service.Models
{
    /// <summary>
    /// Хранение билета на получение информации об отправлениях
    /// </summary>
    public class Ticket
    {
        /// <summary>
        /// Номер билета
        /// </summary>
        public string Value { get; private set; }

        /// <summary>
        /// Дата получения билета
        /// </summary>
        public DateTime DateTime { get; private set; }

        /// <summary>
        /// Имя пользователя, получившего билет
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Создание билета по его номеру
        /// </summary>
        /// <param name="value">Номер билета</param>
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

        /// <summary>
        /// Конвертация в строку
        /// </summary>
        /// <param name="ticket">Объект, содержащий номер билета</param>
        /// <returns>Строка, содержащая номер билета</returns>
        public static string ConvertToString(Ticket ticket)
        {
            return ticket.Value;
        }

        /// <summary>
        /// Конвертация из строки
        /// </summary>
        /// <param name="value">Строка, содержащая номер билета</param>
        /// <returns>Объект, содержащий номер билета</returns>
        public static Ticket ConvertFromString(string value)
        {
            if (Regex.IsMatch(value, "[0-9]{17}[A-Z]{14}"))
            {
                return new Ticket(value);
            }
            Log.Error($"При чтении файла с билетами встречена строка, не соответствующая формату: {value}");
            return new Ticket("");
        }
    }
}
