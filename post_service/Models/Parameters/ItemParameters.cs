using System;
using System.Xml;

namespace post_service.Models.Parameters
{
    /// <summary>
    /// Используется для хранения данных о почтовом отправлении
    /// </summary>
    public class ItemParameters
    {
        /// <summary>
        /// Идентификатор почтового отправления, текущий для данной операции
        /// </summary>
        public string Barcode { get; private set; }

        /// <summary>
        /// Служебная информация, идентифицирующая отправление
        /// </summary>
        public string Internum { get; private set; }

        /// <summary>
        /// Признак корректности вида и категории отправления для внутренней пересылки
        /// </summary>
        public string ValidRuType { get; private set; }

        /// <summary>
        /// Признак корректности вида и категории отправления для международной пересылки
        /// </summary>
        public string ValidEnType { get; private set; }

        /// <summary>
        /// Содержит текстовое описание вида и категории отправления
        /// </summary>
        public string ComplexItemName { get; private set; }

        /// <summary>
        /// Содержит информацию о разряде почтового отправления
        /// </summary>
        public Category MailRank { get; private set; }

        /// <summary>
        /// Содержит информацию об отметках почтовых отправлений
        /// </summary>
        public Category PostMark { get; private set; }

        /// <summary>
        /// Содержит данные о виде почтового отправления
        /// </summary>
        public Category MailType { get; private set; }

        /// <summary>
        /// Содержит данные о категории почтового отправления
        /// </summary>
        public Category MailCtg { get; private set; }

        /// <summary>
        /// Вес отправления в граммах
        /// </summary>
        public string Mass { get; private set; }

        /// <summary>
        /// Значение максимально возможного веса для данного вида и категории отправления для внутренней пересылки
        /// </summary>
        public string MaxMassRu { get; private set; }

        /// <summary>
        /// Значение максимально возможного веса для данного вида и категории отправления для международной пересылки
        /// </summary>
        public string MaxMassEn { get; private set; }

        /// <summary>
        /// Задает значения по-умолчанию для пустого объекта
        /// </summary>
        public ItemParameters()
        {
            Barcode = "";
            Internum = "";
            ValidRuType = "";
            ValidEnType = "";
            ComplexItemName = "";
            MailRank = new Category();
            PostMark = new Category();
            MailType = new Category();
            MailCtg = new Category();
            Mass = "";
            MaxMassRu = "";
            MaxMassEn = "";
        }

        /// <summary>
        /// Создание параметров
        /// </summary>
        /// <param name="barcode">Идентификатор почтового отправления, текущий для данной операции</param>
        /// <param name="internum">Служебная информация, идентифицирующая отправление</param>
        /// <param name="validRuType">Признак корректности вида и категории отправления для внутренней пересылки</param>
        /// <param name="validEnType">Признак корректности вида и категории отправления для международной пересылки</param>
        /// <param name="complexItemName">Содержит текстовое описание вида и категории отправления</param>
        /// <param name="mailRank">Содержит информацию о разряде почтового отправления</param>
        /// <param name="postMark">Содержит информацию об отметках почтовых отправлений</param>
        /// <param name="mailType">Содержит данные о виде почтового отправления</param>
        /// <param name="mailCtg">Содержит данные о категории почтового отправления</param>
        /// <param name="mass">Вес отправления в граммах</param>
        /// <param name="maxMassRu">Значение максимально возможного веса для данного вида и категории отправления для внутренней пересылки</param>
        /// <param name="maxMassEn">Значение максимально возможного веса для данного вида и категории отправления для международной пересылки</param>
        public ItemParameters(string barcode, string internum, string validRuType, string validEnType, string complexItemName, Category mailRank, Category postMark, Category mailType, Category mailCtg, string mass, string maxMassRu, string maxMassEn)
        {
            Barcode = barcode;
            Internum = internum;
            ValidRuType = validRuType;
            ValidEnType = validEnType;
            ComplexItemName = complexItemName;
            MailRank = mailRank;
            PostMark = postMark;
            MailType = mailType;
            MailCtg = mailCtg;
            Mass = mass;
            MaxMassRu = maxMassRu;
            MaxMassEn = maxMassEn;
        }

        /// <summary>
        /// Создание параметров из XML-структуры ItemParameters
        /// </summary>
        /// <param name="ItemParameters">XML-структура ItemParameters</param>
        public ItemParameters(XmlNode ItemParameters)
        {
            Barcode = "";
            Internum = "";
            ValidRuType = "";
            ValidEnType = "";
            ComplexItemName = "";
            MailRank = new Category();
            PostMark = new Category();
            MailType = new Category();
            MailCtg = new Category();
            Mass = "";
            MaxMassRu = "";
            MaxMassEn = "";
            foreach (XmlNode parameter in ItemParameters)
            {
                switch (parameter.Name)
                {
                    case "ns3:Barcode":
                        Barcode = parameter.InnerText;
                        break;
                    case "ns3:Internum":
                        Internum = parameter.InnerText;
                        break;
                    case "ns3:ValidRuType":
                        ValidRuType = parameter.InnerText;
                        break;
                    case "ns3:ValidEnType":
                        ValidEnType = parameter.InnerText;
                        break;
                    case "ns3:ComplexItemName":
                        ComplexItemName = parameter.InnerText;
                        break;
                    case "ns3:MailRank":
                        MailRank = new Category(parameter);
                        break;
                    case "ns3:PostMark":
                        PostMark = new Category(parameter);
                        break;
                    case "ns3:MailType":
                        MailType = new Category(parameter);
                        break;
                    case "ns3:MailCtg":
                        MailCtg = new Category(parameter);
                        break;
                    case "ns3:Mass":
                        Mass = parameter.InnerText;
                        break;
                    case "ns3:MaxMassRu":
                        MaxMassRu = parameter.InnerText;
                        break;
                    case "ns3:MaxMassEn":
                        MaxMassEn = parameter.InnerText;
                        break;
                    default:
                        throw new Exception();
                }
            }
        }
    }
}
