using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
