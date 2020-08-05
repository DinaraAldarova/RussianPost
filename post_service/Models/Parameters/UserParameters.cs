using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace post_service.Models.Parameters
{
    /// <summary>
    /// Используется для хранения данных субъектов, связанных с операцией над отправлением
    /// </summary>
    public class UserParameters
    {
        /// <summary>
        /// Содержит информацию о категории отправителя
        /// </summary>
        public Category SendCtg { get; private set; }

        /// <summary>
        /// Содержит данные об отправителе
        /// </summary>
        public string Sndr { get; private set; }

        /// <summary>
        /// Содержит данные о получателе отправления
        /// </summary>
        public string Rcpn { get; private set; }

        /// <summary>
        /// Задает значения по-умолчанию для пустого объекта
        /// </summary>
        public UserParameters()
        {
            SendCtg = new Category();
            Sndr = "";
            Rcpn = "";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sendCtg">Содержит информацию о категории отправителя</param>
        /// <param name="sndr">Содержит данные об отправителе</param>
        /// <param name="rcpn">Содержит данные о получателе отправления</param>
        public UserParameters(Category sendCtg, string sndr, string rcpn)
        {
            SendCtg = sendCtg;
            Sndr = sndr;
            Rcpn = rcpn;
        }

        /// <summary>
        /// Создание параметров из XML-структуры UserParameters
        /// </summary>
        /// <param name="UserParameters">XML-структура UserParameters</param>
        public UserParameters(XmlNode UserParameters)
        {
            SendCtg = new Category();
            Sndr = "";
            Rcpn = "";
            foreach (XmlNode parameter in UserParameters)
            {
                switch (parameter.Name)
                {
                    case "ns3:SendCtg":
                        SendCtg = new Category(parameter);
                        break;
                    case "ns3:Sndr":
                        Sndr = parameter.InnerText;
                        break;
                    case "ns3:Rcpn":
                        Rcpn = parameter.InnerText;
                        break;
                    default:
                        throw new Exception();
                }
            }
        }
    }
}
