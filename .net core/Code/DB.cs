using post_service.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace post_service.Code
{
    /// <summary>
    /// Методы для работы с базой данных
    /// </summary>
    public static class DB
    {
        /// <summary>
        /// Строка подключения к БД
        /// </summary>
        private const string connString = "Data Source=172.16.80.13,1433;Initial Catalog=test;User ID=test;Password=test334";

        /// <summary>
        /// Формирование текста запроса к базе данных
        /// </summary>
        /// <param name="items">Информация об отправлении</param>
        /// <returns></returns>
        public static string getQuery(Item item)
        {
            string query = "";
            if (item.operations.Count > 0)
            {
                //Операция вручения или возврата будет последняя
                Operation lastOperation = item.operations[item.operations.Count - 1];
                
                if (lastOperation.OperationParameters.OperType.Id == "2")
                //Вручение
                {
                    int operAttr = Convert.ToInt32(lastOperation.OperationParameters.OperAttr.Id);
                    if (operAttr >= 1 && operAttr <= 14)
                    //Атрибут вручения с 1 по 14 (исключая 15-18)
                    {
                        //Преобразование текста даты
                        DateTime dateTime = DateTime.ParseExact(lastOperation.OperationParameters.OperDate, "dd.MM.yyyy HH:mm:ss", null);
                        query = "update main_uin set date_delivery_addressee = '"
                                           + dateTime.ToString("yyyy.MM.dd")
                                           + "' where spi = '"
                                           + item.Barcode
                                           + "'";
                    }
                }
                else if (lastOperation.OperationParameters.OperType.Id == "3" && lastOperation.OperationParameters.OperAttr.Id == "1")
                //Возврат, истек срок хранения
                {
                    //Преобразование текста даты
                    DateTime dateTime = DateTime.ParseExact(lastOperation.OperationParameters.OperDate, "dd.MM.yyyy HH:mm:ss", null);
                    query = "update main_uin set date_delivery_addressee = '"
                                       + dateTime.ToString("yyyy.MM.dd")
                                       + "' where spi = '"
                                       + item.Barcode
                                       + "'";
                }
            }
            return query;
        }

        /// <summary>
        /// Один запрос к БД на запись данных
        /// </summary>
        /// <param name="queryString">Текст запроса</param>
        public static void inputDataSQL(string queryString)
        {
            //Подключаемся к БД
            SqlConnection mySQLConnection = new SqlConnection(connString);
            mySQLConnection.Open();

            //Пишем данные в БД
            SqlCommand mySQLCommand = new SqlCommand(queryString, mySQLConnection);
            mySQLCommand.ExecuteNonQuery();

            //Закрываем соединение с БД
            mySQLConnection.Close();
        }

        /// <summary>
        /// Несколько запросов к БД на запись данных
        /// </summary>
        /// <param name="queriesString">Текст запросов</param>
        public static void inputDataSQL(List<string> queriesString)
        {
            //Подключаемся к БД
            SqlConnection mySQLConnection = new SqlConnection(connString);
            mySQLConnection.Open();

            //Пишем данные в БД
            foreach (string queryString in queriesString)
            {
                SqlCommand mySQLCommand = new SqlCommand(queryString, mySQLConnection);
                mySQLCommand.ExecuteNonQuery();
            }

            //Закрываем соединение с БД
            mySQLConnection.Close();
        }

        /// <summary>
        /// Запрос к БД на чтение данных
        /// </summary>
        /// <param name="queryString">Текст запроса</param>
        /// <returns>Полученные данные и БД</returns>
        public static System.Data.DataTable readDataSQL(string queryString)
        {
            //Переменная для результата запроса
            System.Data.DataTable result = new System.Data.DataTable();

            //Подключаемся к БД
            SqlConnection mySQLConnection = new SqlConnection(connString);
            SqlCommand mySQLCommand = new SqlCommand(queryString, mySQLConnection);
            mySQLConnection.Open();

            //Получаем данные из БД 
            SqlDataAdapter adapterMy = new SqlDataAdapter(mySQLCommand);
            adapterMy.Fill(result);

            //Закрываем соединение с БД
            mySQLConnection.Close();
            adapterMy.Dispose();
            return result;
        }
    }
}
