using System;
using System.Collections.Generic;
using System.Xml;
using post_service.Code;

namespace post_service.Models
{
    /// <summary>
    /// Используется для хранения информации об одном отправлении
    /// </summary>
    public class Item
    {
        /// <summary>
        /// Идентификатор отправления
        /// </summary>
        public string Barcode { get; private set; }

        /// <summary>
        /// Информация по операциям над отправлением
        /// </summary>
        public List<Operation> operations { get; private set; }

        /// <summary>
        /// Готовность информации по билету
        /// </summary>
        public bool isReady { get; private set; }

        /// <summary>
        /// Информация об идентификаторе отправления найдена
        /// </summary>
        public bool isExist { get; private set; }

        /// <summary>
        /// Запрос на получение информации корректно обработан
        /// </summary>
        public bool isCorrect { get; private set; }

        /// <summary>
        /// Задает значения по-умолчанию для пустого объекта
        /// </summary>
        public Item()
        {
            Barcode = "";
            operations = new List<Operation>();
            isReady = true;
            isExist = true;
            isCorrect = true;
        }

        /// <summary>
        /// Создание объекта по ШПИ и списку операций над отправлением
        /// </summary>
        /// <param name="barcode">Идентификатор отправления</param>
        /// <param name="operations">Список операций над отправлением</param>
        public Item(string barcode, List<Operation> operations)
        {
            Barcode = barcode;
            this.operations = operations;
            isReady = true;
            isExist = true;
            isCorrect = true;
        }

        /// <summary>
        /// Создание объекта по XML-структуре
        /// </summary>
        /// <param name="Item">XML-структура</param>
        public Item(XmlNode Item)
        {
            isReady = true;
            isExist = true;
            isCorrect = true;
            Barcode = Item.Attributes["Barcode"].Value;
            operations = new List<Operation>();

            //Обработка ошибок
            if (Item.FirstChild.Name == "ns3:Error")
            {
                switch (Item.FirstChild.Attributes["ErrorTypeID"].Value)
                {
                    case "2":
                        //Формат данных запроса не соответствует установленному настоящим протоколом
                        //throw new Exception(Item.FirstChild.Attributes["ErrorName"].Value);
                        isCorrect = false;
                        Logger.Log.Error($"Формат данных запроса не соответствует протоколу {Barcode} | {Item.FirstChild.Attributes["ErrorName"].Value}");
                        break;
                    case "3":
                        //Неуспешная авторизация клиента при вызове метода
                        //throw new Exception(Item.FirstChild.Attributes["ErrorName"].Value);
                        isCorrect = false;
                        Logger.Log.Error($"Неуспешная авторизация клиента при вызове метода {Barcode} | {Item.FirstChild.Attributes["ErrorName"].Value}");
                        break;
                    case "6":
                        //Ответ по билету ещё не готов
                        //throw new Exception(Item.FirstChild.Attributes["ErrorName"].Value);
                        isReady = false; 
                        break;
                    case "12":
                        //Информация о заданном идентификаторе отправления отсутствует
                        //throw new Exception(Item.FirstChild.Attributes["ErrorName"].Value);
                        isExist = false;
                        Logger.Log.Error($"Информация об идентификаторе отправления отсутствует {Barcode} | {Item.FirstChild.Attributes["ErrorName"].Value}");
                        break;
                    case "16":
                        //Внутренняя ошибка работы Сервиса отслеживания
                        //throw new Exception(Item.FirstChild.Attributes["ErrorName"].Value);
                        isCorrect = false;
                        Logger.Log.Error($"Внутренняя ошибка работы Сервиса отслеживания {Barcode} | {Item.FirstChild.Attributes["ErrorName"].Value}");
                        break;
                    case "17":
                        //Время хранения ответа по билету истекло, ответ был удален с сервера
                        //throw new Exception(Item.FirstChild.Attributes["ErrorName"].Value);
                        isCorrect = false;
                        Logger.Log.Error($"Время хранения ответа по билету истекло, ответ был удален с сервера {Barcode} | {Item.FirstChild.Attributes["ErrorName"].Value}");
                        break;
                    default:
                        //throw new Exception();
                        isCorrect = false;
                        Logger.Log.Error($"Неизвестный номер ошибки {Barcode} | {Item.FirstChild.Attributes["ErrorName"].Value}");
                        break;
                }
                return;
            }

            //Запись информации об операциях
            foreach (XmlNode xmlOperation in Item)
            {
                string OperTypeID = "";
                string OperCtgID = "";
                string OperName = "";
                string DateOper = "";
                string IndexOper = "";
                foreach (XmlAttribute attribute in xmlOperation.Attributes)
                {
                    switch (attribute.Name)
                    {
                        case "OperTypeID":
                            OperTypeID = attribute.Value;
                            break;
                        case "OperCtgID":
                            OperCtgID = attribute.Value;
                            break;
                        case "OperName":
                            OperName = attribute.Value;
                            break;
                        case "DateOper":
                            DateOper = attribute.Value;
                            break;
                        case "IndexOper":
                            IndexOper = attribute.Value;
                            break;
                        default:
                            throw new Exception();
                    }
                }
                Operation operation = new Operation(OperTypeID, OperCtgID, OperName, DateOper, IndexOper);
                operations.Add(operation);
            }
        }
    }
}
