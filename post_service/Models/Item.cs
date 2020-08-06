using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using post_service.Models.Parameters;
using post_service.Models.Parameters.Address;

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
        /// Задает значения по-умолчанию для пустого объекта
        /// </summary>
        public Item()
        {
            Barcode = "";
            operations = new List<Operation>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="barcode">Идентификатор отправления</param>
        /// <param name="operations">Информация по операциям над отправлением</param>
        public Item(string barcode, List<Operation> operations)
        {
            Barcode = barcode;
            this.operations = operations;
        }

        /// <summary>
        /// Создание параметров из XML-структуры категории
        /// </summary>
        /// <param name="Category">XML-структура категории</param>
        public Item(XmlNode Item)
        {
            Barcode = Item.Attributes["Barcode"].Value;

            if (Item.FirstChild.Name == "ns3:Error")
            {
                switch (Item.FirstChild.Attributes["ErrorTypeID"].Value)
                {
                    case "2":
                        //Формат данных запроса не соответствует установленному настоящим протоколом
                        throw new Exception(Item.FirstChild.Attributes["ErrorName"].Value);
                        break;
                    case "3":
                        //Неуспешная авторизация клиента при вызове метода
                        throw new Exception(Item.FirstChild.Attributes["ErrorName"].Value);
                        break;
                    case "6":
                        //Ответ по билету ещё не готов
                        //throw new Exception(Item.FirstChild.Attributes["ErrorName"].Value);
                        break;
                    case "12":
                        //Информация о заданном идентификаторе отправления отсутствует
                        //throw new Exception(Item.FirstChild.Attributes["ErrorName"].Value);
                        break;
                    case "16":
                        //Внутренняя ошибка работы Сервиса отслеживания
                        throw new Exception(Item.FirstChild.Attributes["ErrorName"].Value);
                        break;
                    case "17":
                        //Время хранения ответа по билету истекло, ответ был удален с сервера
                        //throw new Exception(Item.FirstChild.Attributes["ErrorName"].Value);
                        break;
                    default:
                        //throw new Exception();
                        break;
                }
                operations = new List<Operation>();
                return;
            }

            operations = new List<Operation>();
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
