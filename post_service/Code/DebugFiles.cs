using post_service.Models;
using System;
using System.Collections.Generic;
using System.IO;

namespace post_service.Code
{
    /// <summary>
    /// Функции для записи и чтения файлов, хранящих отладочную информацию
    /// </summary>
    public static class DebugFiles
    {
        /// <summary>
        /// Запись списка ШПИ в файл
        /// </summary>
        /// <param name="barcodes">Список ШПИ</param>
        /// <param name="path">Путь к файлу</param>
        public static void WriteBarcodesToFile(List<Barcode> barcodes, string path)
        {
            File.WriteAllLines(path, barcodes.ConvertAll(new Converter<Barcode, string>(Barcode.ConvertToString)));
        }

        /// <summary>
        /// Чтение списка ШПИ из файла
        /// </summary>
        /// <param name="path">Путь к файлу</param>
        /// <returns>Список ШПИ</returns>
        public static List<Barcode> ReadBarcodesFromFile(string path)
        {
            List<string> strBarcodes = new List<string>(File.ReadAllLines(path));
            List<Barcode> barcodes = strBarcodes.ConvertAll(new Converter<string, Barcode>(Barcode.ConvertFromString));
            return barcodes;
        }

        /// <summary>
        /// Запись списка билетов в файл
        /// </summary>
        /// <param name="tickets">Список билетов</param>
        /// <param name="path">Путь к файлу</param>
        public static void WriteTicketsToFile(List<Ticket> tickets, string path)
        {
            File.WriteAllLines(path, tickets.ConvertAll(new Converter<Ticket, string>(Ticket.ConvertToString)));
        }

        /// <summary>
        /// Чтение списка билетов из файла
        /// </summary>
        /// <param name="path">Путь к файлу</param>
        /// <returns>Список билетов</returns>
        public static List<Ticket> ReadTicketsFromFile(string path)
        {
            List<string> strTickets = new List<string>(File.ReadAllLines(path));
            List<Ticket> tickets = strTickets.ConvertAll(new Converter<string, Ticket>(Ticket.ConvertFromString));
            return tickets;
        }

        /// <summary>
        /// Запись списка запросов к БД в файл
        /// </summary>
        /// <param name="queries">Список запросов к БД</param>
        /// <param name="path">Путь к файлу</param>
        public static void WriteQueriesToFile(List<string> queries, string path)
        {
            File.WriteAllLines(path, queries);
        }

        /// <summary>
        /// Чтение списка запросов к БД из файла
        /// </summary>
        /// <param name="path">Путь к файлу</param>
        /// <returns>Список запросов к БД</returns>
        public static List<string> ReadQueriesFromFile(string path)
        {
            List<string> queries = new List<string>(File.ReadAllLines(path));
            return queries;
        }
    }
}
