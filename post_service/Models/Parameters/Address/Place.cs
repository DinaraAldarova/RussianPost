using System;
using System.Xml;

namespace post_service.Models.Parameters.Address
{
    /// <summary>
    /// Используется для атрибутов DestinationAddress, OperationAddress класса AddressParameters
    /// </summary>
    public class Place
    {
        /// <summary>
        /// Почтовый индекс места
        /// </summary>
        public string Index { get; private set; }

        /// <summary>
        /// Адрес и/или название места
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// Задает значения по-умолчанию для пустого объекта
        /// </summary>
        public Place()
        {
            Index = "";
            Description = "";
        }

        /// <summary>
        /// Создание параметра
        /// </summary>
        /// <param name="index">Почтовый индекс места</param>
        /// <param name="description">Адрес и/или название места</param>
        public Place(string index, string description)
        {
            Index = index;
            Description = description;
        }

        /// <summary>
        /// Создание параметра из XML-структуры места
        /// </summary>
        /// <param name="Place">XML-структура места</param>
        public Place(XmlNode Place)
        {
            Index = "";
            Description = "";
            foreach (XmlNode parameter in Place)
            {
                switch (parameter.Name)
                {
                    case "ns3:Index":
                        Index = parameter.InnerText;
                        break;
                    case "ns3:Description":
                        Description = parameter.InnerText;
                        break;
                    default:
                        throw new Exception();
                }
            }
        }

        /// <summary>
        /// Создание параметра по информации, предоставленной в пакетном запросе
        /// </summary>
        /// <param name="IndexOper">Почтовый индекс места проведения операции</param>
        public Place(string IndexOper)
        {
            Index = IndexOper;
            Description = "";
        }
    }
}
