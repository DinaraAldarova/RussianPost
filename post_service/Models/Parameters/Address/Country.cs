using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace post_service.Models.Parameters.Address
{
    /// <summary>
    /// Используется для атрибутов MailDirect, CountryFrom, CountryOper класса AddressParameters
    /// </summary>
    public class Country
    {
        /// <summary>
        /// Код страны
        /// </summary>
        public string Id { get; private set; }

        /// <summary>
        /// Двухбуквенный идентификатор страны
        /// </summary>
        public string Code2A { get; private set; }

        /// <summary>
        /// Трехбуквенный идентификатор страны
        /// </summary>
        public string Code3A { get; private set; }

        /// <summary>
        /// Российское название страны
        /// </summary>
        public string NameRU { get; private set; }

        /// <summary>
        /// Международное название страны
        /// </summary>
        public string NameEN { get; private set; }

        /// <summary>
        /// Задает значения по-умолчанию для пустого объекта
        /// </summary>
        public Country()
        {
            Id = "";
            Code2A = "";
            Code3A = "";
            NameRU = "";
            NameEN = "";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">Код страны</param>
        /// <param name="code2A">Двухбуквенный идентификатор страны</param>
        /// <param name="code3A">Трехбуквенный идентификатор страны</param>
        /// <param name="nameRU">Российское название страны</param>
        /// <param name="nameEN">Международное название страны</param>
        public Country(string id, string code2A, string code3A, string nameRU, string nameEN)
        {
            Id = id;
            Code2A = code2A;
            Code3A = code3A;
            NameRU = nameRU;
            NameEN = nameEN;
        }

        /// <summary>
        /// Создание параметров из XML-структуры страны
        /// </summary>
        /// <param name="Country">XML-структура страны</param>
        public Country(XmlNode Country)
        {
            Id = "";
            Code2A = "";
            Code3A = "";
            NameRU = "";
            NameEN = "";
            foreach (XmlNode parameter in Country)
            {
                switch (parameter.Name)
                {
                    case "ns3:Id":
                        Id = parameter.InnerText;
                        break;
                    case "ns3:Code2A":
                        Code2A = parameter.InnerText;
                        break;
                    case "ns3:Code3A":
                        Code3A = parameter.InnerText;
                        break;
                    case "ns3:NameRU":
                        NameRU = parameter.InnerText;
                        break;
                    case "ns3:NameEN":
                        NameEN = parameter.InnerText;
                        break;
                    default:
                        throw new Exception();
                }
            }
        }
    }
}
