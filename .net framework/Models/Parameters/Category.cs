using System;
using System.Xml;

namespace post_service.Models.Parameters
{
    /// <summary>
    /// Используется для атрибутов, содержащих название и id элемента определенной категории
    /// </summary>
    public class Category
    {
        /// <summary>
        /// Код
        /// </summary>
        public string Id { get; private set; }

        /// <summary>
        /// Название
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Задает значения по-умолчанию для пустого объекта
        /// </summary>
        public Category()
        {
            Id = "";
            Name = "";
        }

        /// <summary>
        /// Создание параметра
        /// </summary>
        /// <param name="id">Код</param>
        /// <param name="name">Название</param>
        public Category(string id, string name)
        {
            Id = id;
            Name = name;
        }

        /// <summary>
        /// Создание параметров из XML-структуры категории
        /// </summary>
        /// <param name="Category">XML-структура категории</param>
        public Category(XmlNode Category)
        {
            Id = "";
            Name = "";
            foreach (XmlNode parameter in Category)
            {
                switch(parameter.Name)
                {
                    case "ns3:Id":
                        Id = parameter.InnerText;
                        break;
                    case "ns3:Name":
                        Name = parameter.InnerText;
                        break;
                    default:
                        throw new Exception();
                }
            }
        }
    }
}
