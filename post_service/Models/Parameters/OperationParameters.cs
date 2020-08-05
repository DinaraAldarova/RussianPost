using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace post_service.Models.Parameters
{
    /// <summary>
    /// Используется для хранения данных об операциях по отправлению
    /// </summary>
    public class OperationParameters
    {
        /// <summary>
        /// Содержит информацию об операции над отправлением
        /// </summary>
        public Category OperType { get; private set; }

        /// <summary>
        /// Содержит информацию об атрибуте операции над отправлением
        /// </summary>
        public Category OperAttr { get; private set; }

        /// <summary>
        /// Содержит данные о дате и времени проведения операции над отправлением
        /// </summary>
        public string OperDate { get; private set; }

        /// <summary>
        /// Задает значения по-умолчанию для пустого объекта
        /// </summary>
        public OperationParameters()
        {
            OperType = new Category();
            OperAttr = new Category();
            OperDate = "";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="operType">Содержит информацию об операции над отправлением</param>
        /// <param name="operAttr">Содержит информацию об атрибуте операции над отправлением</param>
        /// <param name="operDate">Содержит данные о дате и времени проведения операции над отправлением</param>
        public OperationParameters(Category operType, Category operAttr, string operDate)
        {
            OperType = operType;
            OperAttr = operAttr;
            OperDate = operDate;
        }

        /// <summary>
        /// Создание параметров из XML-структуры OperationParameters
        /// </summary>
        /// <param name="OperationParameters">XML-структура OperationParameters</param>
        public OperationParameters(XmlNode OperationParameters)
        {
            OperType = new Category();
            OperAttr = new Category();
            OperDate = "";
            foreach (XmlNode parameter in OperationParameters)
            {
                switch (parameter.Name)
                {
                    case "ns3:OperType":
                        OperType = new Category(parameter);
                        break;
                    case "ns3:OperAttr":
                        OperAttr = new Category(parameter);
                        break;
                    case "ns3:OperDate":
                        OperDate = parameter.InnerText;
                        break;
                    default:
                        throw new Exception();
                }
            }
        }

        /// <summary>
        /// Создание параметров по информации, предоставленной в пакетном запросе
        /// </summary>
        /// <param name="OperTypeID">Код операции</param>
        /// <param name="OperCtgID">Код атрибута</param>
        /// <param name="OperName">Название операции</param>
        /// <param name="DateOper">Дата и время операции (локальное)</param>
        public OperationParameters(string OperTypeID, string OperCtgID, string OperName, string DateOper)
        {
            OperType = new Category(OperTypeID, OperName);
            OperAttr = new Category(OperCtgID, "");
            OperDate = DateOper;
        }
    }
}
