﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace post_service.Code
{
    public static class Functions
    {
        /// <summary>
        /// Формула «Дата вступления в законную силу» Изменяем effective_date в Таблица УИН (main_uin) 
        /// </summary>
        /// <param name="updateUIN">УИН по которому происходят изменения</param>
        public static void Update_effective_date(string updateUIN)
        {
            System.Data.DataTable input_date_delivery_addressee = new System.Data.DataTable();// = result.Tables[0];

            //Получаем из базы Дату вручения адресату по УИНу
            input_date_delivery_addressee = DB.readDataSQL("SELECT date_delivery_addressee FROM main_uin WHERE uin='" + updateUIN + "'");

            //Дата вручения адресату не пустая
            if (input_date_delivery_addressee.Rows[0][0] != System.DBNull.Value)
            {
                //Формат даты
                CultureInfo enUS = CultureInfo.CreateSpecificCulture("en-US");
                DateTimeFormatInfo dtfi = enUS.DateTimeFormat;
                dtfi.ShortDatePattern = "yyyy.MM.dd";

                //Заголовок строки для вставки в базу, Сразу вставляем Последний день подачи жалобы
                string strUPD = "UPDATE main_uin SET lastday_complaint='";
                //Временная переменная для хранения дат
                DateTime temp_date;
                //Переносим Дату вручения адресату во временную переменную
                DateTime.TryParse(input_date_delivery_addressee.Rows[0][0].ToString(), out temp_date);

                //Увеличиваем временную переменную на 10 дней и проверяем что 10 день не выходной или праздник, получаем Последний день подачи жалобы
                temp_date = temp_date.AddDays(9);
                int i = 1;
                while (i > 0)
                {
                    temp_date = temp_date.AddDays(1);
                    if (Holiday(temp_date) == false && temp_date.DayOfWeek != DayOfWeek.Saturday && temp_date.DayOfWeek != DayOfWeek.Sunday) i--;
                }

                //Добавляем к строке дату Последний день подачи жалобы
                strUPD += temp_date.ToString("d", enUS).Replace(".", string.Empty);

                //Добавляем один день к Последний день подачи жалобы и получаем Дата вступления в законную силу
                temp_date = temp_date.AddDays(1);
                strUPD += "', effective_date='" + temp_date.ToString("d", enUS).Replace(".", string.Empty);

                //Добавляем 60 дней от Даты вступления в законную силу и получаем Срок для оплаты штрафа
                temp_date = temp_date.AddDays(60);
                strUPD += "', timelimit_payment='" + temp_date.ToString("d", enUS).Replace(".", string.Empty);

                //Добавляем 10 дней к Срок для оплаты штрафа и получаем Плановая дата направления в ФССП (последний день). Финализируем строку для вставки.
                temp_date = temp_date.AddDays(10);
                strUPD += "', fssp_plane_date='" + temp_date.ToString("d", enUS).Replace(".", string.Empty) + "' WHERE uin='" + updateUIN + "'";

                try
                {
                    DB.inputDataSQL(strUPD);
                }
                catch
                {

                }

            }

            //Рассчитываем оставшуюся сумму для взыскания

            System.Data.DataTable input_amount_recover = new System.Data.DataTable();
            System.Data.DataTable input_fssp_status = new System.Data.DataTable();
            //Получаем из базы Сумма для взыскания
            input_amount_recover = DB.readDataSQL("SELECT amount_recover FROM main_uin WHERE uin='" + updateUIN + "'");
            //Получаем из базы Статусы ФССП по УИНу
            input_fssp_status = DB.readDataSQL("SELECT fssp_status FROM main_uin WHERE uin='" + updateUIN + "'");

            //Проверяем что Сумма для взыская меньше или равна 0
            if (Convert.ToInt32(input_amount_recover.Rows[0][0]) <= 0)
            {
                //Заполняем строку для вставки в базу данных. Устанавливаем Срок для оплаты штрафа и Плановая дата направления в ФССП (последний день) в значение NULL
                string strUPD = "UPDATE main_uin SET timelimit_payment=NULL, fssp_plane_date=NULL";

                //Статус ФССП не равен 2 или 5 или 9
                if (Convert.ToInt32(input_fssp_status.Rows[0][0]) == 2 | Convert.ToInt32(input_fssp_status.Rows[0][0]) == 5 | Convert.ToInt32(input_fssp_status.Rows[0][0]) == 9)
                {
                }
                else
                {
                    strUPD += ", fssp_status=12";
                }

                //Финализируем строку для вставки
                strUPD += " WHERE uin='" + updateUIN + "'";

                try
                {
                    DB.inputDataSQL(strUPD);
                }
                catch
                {

                }
            }
        }

        /// <summary>
        /// Формула «Сумма для взыскания» Изменяем amount_recover в Таблица УИН (main_uin)
        /// </summary>
        /// <param name="updateUIN"></param>
        public static void Update_amount_recover(string updateUIN)
        {
            //Получаем из базы данных все жалобы по УИНу
            System.Data.DataTable input_date = new System.Data.DataTable();
            //Получаем из базы данных Номер жалобы по порядку и Информация о результатах рассмотрения жалобы
            input_date = DB.readDataSQL("SELECT number_claim, results_claim FROM claim WHERE uin='" + updateUIN + "'");
            //Получаем из базы данных Оплаты по УИНу
            System.Data.DataTable input_pays = new System.Data.DataTable();
            //input_pays = DB.readDataSQL("SELECT size_payment FROM payment WHERE uin='" + updateUIN + "'", connString);
            input_pays = DB.readDataSQL("SELECT SUM(cast(replace(size_payment, ',', '.') as money)) FROM payment WHERE uin='" + updateUIN + "'");
            //Получаем из базы данных Размер штрафа по УИНу
            System.Data.DataTable input_fine_size = new System.Data.DataTable();
            input_fine_size = DB.readDataSQL("SELECT fine_size FROM main_uin WHERE uin='" + updateUIN + "'");
            //Получаем статус ФССП
            System.Data.DataTable input_fssp_status = new System.Data.DataTable();
            input_fssp_status = DB.readDataSQL("SELECT fssp_status FROM main_uin WHERE uin='" + updateUIN + "'");
            //считаем сумму оплат по УИНу
            Decimal sum_pay = 0;
            if (input_pays.Rows[0][0] != System.DBNull.Value)
            {
                sum_pay = Convert.ToDecimal(input_pays.Rows[0][0]);
            }


            //for (int i = 0; i < input_pays.Rows.Count; i++)
            // {
            //    sum_pay += Convert.ToDecimal(input_pays.Rows[i][0]);
            //}

            //Количество жалоб по УИНу > 0
            // if (input_date.Rows[0][0] != System.DBNull.Value)
            if (input_date.Rows.Count > 0)
            {
                if (Convert.ToInt32(input_date.Rows[input_date.Rows.Count - 1][1]) == 3)
                {
                    //Устанавливаем Сумма для взыскания = 0 - Сумма оплат по УИНу

                    try
                    {
                        DB.inputDataSQL("UPDATE main_uin SET amount_recover='" + (0 - sum_pay).ToString().Replace(",", ".") + "' FROM main_uin WHERE uin='" + updateUIN + "'");
                    }
                    catch
                    {

                    }

                }
                else
                {
                    //Получаем из базы данных Размер штрафа после обжалования (переквалификация) по УИНу
                    System.Data.DataTable input_amount_appeal = new System.Data.DataTable();
                    input_amount_appeal = DB.readDataSQL("SELECT amount_appeal FROM main_uin WHERE uin='" + updateUIN + "'");
                    //Размер штрафа после обжалования (переквалификации) не равен NULL
                    if (input_amount_appeal.Rows[0][0] != System.DBNull.Value)
                    {
                        //Устанавливаем Сумма для взыскания = Размер штрафа после обжалования (переквалификация) - Сумма оплат по УИНу
                        //strUPD = "UPDATE main_uin SET amount_recover='" + (Convert.ToSingle(input_amount_appeal.Rows[0][0]) - sum_pay) + "' FROM main_uin WHERE uin='" + updateUIN + "'";
                        sum_pay = Convert.ToDecimal(input_amount_appeal.Rows[0][0]) - sum_pay;
                        try
                        {
                            DB.inputDataSQL("UPDATE main_uin SET amount_recover='" + sum_pay.ToString().Replace(",", ".") + "' FROM main_uin WHERE uin='" + updateUIN + "'");
                        }
                        catch
                        {
                        }
                    }
                    else
                    {
                        //Устанавливаем Сумма для взыскания = Размер штрафа - Сумма оплат по УИНу
                        //strUPD = "UPDATE main_uin SET amount_recover='" + (Convert.ToSingle(input_fine_size.Rows[0][0]) - sum_pay) + "' FROM main_uin WHERE uin='" + updateUIN + "'";
                        sum_pay = Convert.ToDecimal(input_fine_size.Rows[0][0]) - sum_pay;
                        try
                        {
                            DB.inputDataSQL("UPDATE main_uin SET amount_recover='" + sum_pay.ToString().Replace(",", ".") + "' FROM main_uin WHERE uin='" + updateUIN + "'");
                        }
                        catch
                        {
                        }

                    }
                }
            }
            else
            {
                //Устанавливаем Сумма для взыскания = Размер штрафа - Сумма оплат по УИНу
                sum_pay = Convert.ToDecimal(input_fine_size.Rows[0][0]) - sum_pay;
                try
                {
                    DB.inputDataSQL("UPDATE main_uin SET amount_recover='" + sum_pay.ToString().Replace(",", ".") + "' FROM main_uin WHERE uin='" + updateUIN + "'");
                }
                catch
                {

                }

            }

            try
            {
                //Сумма для взыскания меньше или равно 0
                if (sum_pay <= 0)
                {
                    //Срок для оплаты штрафа = NULL, Плановая дата направления в ФССП = NULL
                    string strUPD = "UPDATE main_uin SET timelimit_payment=NULL, fssp_plane_date=NULL";
                    //Если Статус ФССП не равен Прекращение исполнения (Смерть должника) или Окончание (Оплата) или Окончание (Срок)
                    if (Convert.ToInt32(input_fssp_status.Rows[0][0]) == 2 | Convert.ToInt32(input_fssp_status.Rows[0][0]) == 5 | Convert.ToInt32(input_fssp_status.Rows[0][0]) == 9 | Convert.ToInt32(input_fssp_status.Rows[0][0]) == 7 | Convert.ToInt32(input_fssp_status.Rows[0][0]) == 8)
                    {
                    }
                    else
                    {
                        strUPD += ", fssp_status=12";
                    }

                    //Финализируем строку для вставки
                    strUPD += " FROM main_uin WHERE uin = '" + updateUIN + "'";
                    try
                    {
                        DB.inputDataSQL(strUPD);
                    }
                    catch
                    {

                    }
                }
            }
            catch
            {

            }
        }

        /// <summary>
        /// Формула «Последний день подачи жалобы» Изменяем lastday_complaint в Таблица УИН (main_uin)
        /// </summary>
        /// <param name="updateUIN"></param>
        public static void Update_lastday_complaint(string updateUIN)
        {
            //Получаем из базы данных все жалобы по УИНу
            System.Data.DataTable input_date = new System.Data.DataTable();
            //Получаем из базы данных Номер жалобы по порядку, Информация о результатах рассмотрения жалобы и Дата вступления в силу после обжалования
            input_date = DB.readDataSQL("SELECT number_claim, results_claim, date_decision_claim FROM claim WHERE uin='" + updateUIN + "'");

            //Получаем из базы данных Статус ФССП по УИНу
            System.Data.DataTable input_fssp_status = new System.Data.DataTable();
            input_fssp_status = DB.readDataSQL("SELECT fssp_status FROM main_uin WHERE uin='" + updateUIN + "'");

            //Количество жалоб по УИНу > 0
            //if (input_date.Rows[0][0] != System.DBNull.Value)
            if (input_date.Rows.Count > 0)
            {
                //Переменная для вставки в базу данных
                string strUPD = string.Empty;

                //Дата вступления в силу после обжалования = NULL
                if (input_date.Rows[input_date.Rows.Count - 1][2] == System.DBNull.Value)
                {
                    //Последний день подачи жалобы = NULL, Плановая дата направления ФССП = NULL, Срок для оплаты штрафа = NULL
                    strUPD = "UPDATE main_uin SET lastday_complaint= NULL, fssp_plane_date = NULL, timelimit_payment = NULL";

                    //Если статус ФССП не равен Возврат или Прекращение исполнения (Смерть должника) или Прекращение исполнения (Иное) или Возбуждение или Окончание (Оплата) или Окончание (Отсутствие должника, имущества) или В работе или Передано в ФССП или Окончание (Срок) или Отмена по жалобе или Оплата
                    if (Convert.ToInt32(input_fssp_status.Rows[0][0]) == 1 | Convert.ToInt32(input_fssp_status.Rows[0][0]) == 2 | Convert.ToInt32(input_fssp_status.Rows[0][0]) == 3 | Convert.ToInt32(input_fssp_status.Rows[0][0]) == 4 | Convert.ToInt32(input_fssp_status.Rows[0][0]) == 5 | Convert.ToInt32(input_fssp_status.Rows[0][0]) == 6 | Convert.ToInt32(input_fssp_status.Rows[0][0]) == 7 | Convert.ToInt32(input_fssp_status.Rows[0][0]) == 8 | Convert.ToInt32(input_fssp_status.Rows[0][0]) == 9 | Convert.ToInt32(input_fssp_status.Rows[0][0]) == 11 | Convert.ToInt32(input_fssp_status.Rows[0][0]) == 12)
                    {

                    }
                    else
                    {
                        strUPD += ", fssp_status=10";
                    }
                }
                else
                {
                    //Информация о результатах рассмотрения жалобы = Отменено
                    if (Convert.ToInt32(input_date.Rows[input_date.Rows.Count - 1][1]) == 3)
                    {
                        //Последний день подачи жалобы = NULL, Плановая дата направления ФССП = NULL, Срок для оплаты штрафа = NULL, Размер штрафа после обжалования = 0, Статус ФССП = Отменена по жалобе
                        strUPD = "UPDATE main_uin SET lastday_complaint= NULL, fssp_plane_date = NULL, timelimit_payment = NULL, amount_appeal = 0, fssp_status = 11";

                        //Срок для оплаты штрафа после обжалования = NULL
                        try
                        {
                            DB.inputDataSQL("UPDATE claim SET last_time_payment_decision = NULL FROM claim WHERE number_claim='" + Convert.ToInt32(input_date.Rows[input_date.Rows.Count - 1][0]) + "'");
                        }
                        catch
                        {

                        }
                    }
                    else
                    {
                        //Информация о результатах рассмортения жалобы = Частично или Оставлено без изменений
                        if (Convert.ToInt32(input_date.Rows[input_date.Rows.Count - 1][1]) == 1 | Convert.ToInt32(input_date.Rows[input_date.Rows.Count - 1][1]) == 2)
                        {
                            //Изменяем в последней Жалобе Срок для оплаты штрафа после обжалования = Дата вступления в силу после обжалования + 60 дней
                            //Временная переменная для хранения дат
                            DateTime temp_date;
                            //Переносим Дата вступления в силу после обжалования во временную переменную
                            DateTime.TryParse(input_date.Rows[input_date.Rows.Count - 1][2].ToString(), out temp_date);
                            //temp_date = temp_date.AddDays(60);
                            try
                            {
                                //Меняем запись в последней Жалобе по УИНу
                                DB.inputDataSQL("UPDATE claim SET last_time_payment_decision = '" + temp_date.AddDays(60) + "' From claim WHERE number_claim='" + Convert.ToInt32(input_date.Rows[input_date.Rows.Count - 1][0]) + "'");

                                //Меняем в УИНе Плановая дата направления в ФССП = Срок для оплаты штрафа после обжалования + 10 дней, Последний день подачи жалобы = Дата вступления в силу после обдалования - 1 день, Дата вступления в законную силу = Дата вступления в силу после обжалования, Срок для оплаты штрафа = Срок для оплаты штрафа после обжалования
                                strUPD = "UPDATE main_uin SET fssp_plane_date = '" + temp_date.AddDays(70) + "', lastday_complaint = '" + temp_date.AddDays(59) + "', effective_date = '" + temp_date + "', timelimit_payment = '" + temp_date.AddDays(60) + "'";
                                //Статус ФССП не равен Возврат или Прекращение исполнения (Смерть должника) или Прекращение исполнения (Иное) или Возбуждение или Окончание (Оплата) или Окончание (Отсутствие должника, имущества) или В работе или Передано в ФССП или Окончание (Срок) или Отмена по жалобе или Оплата
                                if (Convert.ToInt32(input_fssp_status.Rows[0][0]) == 1 | Convert.ToInt32(input_fssp_status.Rows[0][0]) == 2 | Convert.ToInt32(input_fssp_status.Rows[0][0]) == 3 | Convert.ToInt32(input_fssp_status.Rows[0][0]) == 4 | Convert.ToInt32(input_fssp_status.Rows[0][0]) == 5 | Convert.ToInt32(input_fssp_status.Rows[0][0]) == 6 | Convert.ToInt32(input_fssp_status.Rows[0][0]) == 7 | Convert.ToInt32(input_fssp_status.Rows[0][0]) == 8 | Convert.ToInt32(input_fssp_status.Rows[0][0]) == 9 | Convert.ToInt32(input_fssp_status.Rows[0][0]) == 11 | Convert.ToInt32(input_fssp_status.Rows[0][0]) == 12)
                                {

                                }
                                else
                                {
                                    strUPD += ", fssp_status=0";
                                }
                            }
                            catch
                            {

                            }
                        }
                    }
                }

                //Финализируем строку для изменения
                strUPD += " FROM main_uin WHERE uin = '" + updateUIN + "'";

                try
                {
                    DB.inputDataSQL(strUPD);
                }
                catch
                {

                }
            }
        }

        /// <summary>
        /// Поиск праздников
        /// </summary>
        /// <param name="mydate"></param>
        /// <returns></returns>
        internal static bool Holiday(DateTime mydate)
        {
            System.Data.DataTable dataTable = new System.Data.DataTable();
            dataTable = DB.readDataSQL("SELECT holidays_date FROM holidays Where holidays_date = '" + mydate + "'");
            if (dataTable.Rows.Count != 0)
            {
                return (true);
            }
            else
            {
                return (false);
            }
        }
    }
}
