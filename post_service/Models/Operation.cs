using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using post_service.Models.Parameters;

namespace post_service.Models
{
    /// <summary>
    /// Используется для хранения информации об операции над отправлением
    /// </summary>
    public class Operation
    {
        /// <summary>
        /// Содержит адресные данные с операцией над отправлением
        /// </summary>
        public AddressParameters AddressParameters { get; private set; }

        /// <summary>
        /// Содержит финансовые данные, связанные с операцией над почтовым отправлением
        /// </summary>
        public FinanceParameters FinanceParameters { get; private set; }

        /// <summary>
        /// Содержит данные о почтовом отправлении
        /// </summary>
        public ItemParameters ItemParameters { get; private set; }

        /// <summary>
        /// Cодержит параметры операции над отправлением
        /// </summary>
        public OperationParameters OperationParameters { get; private set; }

        /// <summary>
        /// Содержит данные субъектов, связанных с операцией над почтовым отправлением
        /// </summary>
        public UserParameters UserParameters { get; private set; }

        /// <summary>
        /// Задает значения по-умолчанию для пустого объекта
        /// </summary>
        public Operation()
        {
            AddressParameters = new AddressParameters();
            FinanceParameters = new FinanceParameters();
            ItemParameters = new ItemParameters();
            OperationParameters = new OperationParameters();
            UserParameters = new UserParameters();
        }

        /// <summary>
        /// Создание операции
        /// </summary>
        /// <param name="addressParameters">Содержит адресные данные с операцией над отправлением</param>
        /// <param name="financeParameters">Содержит финансовые данные, связанные с операцией над почтовым отправлением</param>
        /// <param name="itemParameters">Содержит данные о почтовом отправлении</param>
        /// <param name="operationParameters">Cодержит параметры операции над отправлением</param>
        /// <param name="userParameters">Содержит данные субъектов, связанных с операцией над почтовым отправлением</param>
        public Operation(AddressParameters addressParameters, FinanceParameters financeParameters, ItemParameters itemParameters, OperationParameters operationParameters, UserParameters userParameters)
        {
            AddressParameters = addressParameters;
            FinanceParameters = financeParameters;
            ItemParameters = itemParameters;
            OperationParameters = operationParameters;
            UserParameters = userParameters;
        }

        /// <summary>
        /// Создание операции из XML-структуры historyRecord
        /// </summary>
        /// <param name="historyRecord">XML-структура historyRecord</param>
        public Operation(XmlNode historyRecord)
        {
            AddressParameters = new AddressParameters();
            FinanceParameters = new FinanceParameters();
            ItemParameters = new ItemParameters();
            OperationParameters = new OperationParameters();
            UserParameters = new UserParameters();
            foreach (XmlNode parameters in historyRecord)
            {
                switch (parameters.Name)
                {
                    case "ns3:AddressParameters":
                        AddressParameters = new AddressParameters(parameters);
                        break;
                    case "ns3:FinanceParameters":
                        FinanceParameters = new FinanceParameters(parameters);
                        break;
                    case "ns3:ItemParameters":
                        ItemParameters = new ItemParameters(parameters);
                        break;
                    case "ns3:OperationParameters":
                        OperationParameters = new OperationParameters(parameters);
                        break;
                    case "ns3:UserParameters":
                        UserParameters = new UserParameters(parameters);
                        break;
                    default:
                        throw new Exception();
                }
            }
        }

        /// <summary>
        /// Создание операции по информации, предоставленной в пакетном запросе
        /// </summary>
        /// <param name="OperTypeID">Код операции</param>
        /// <param name="OperCtgID">Код атрибута</param>
        /// <param name="OperName">Название операции</param>
        /// <param name="DateOper">Дата и время операции (локальное)</param>
        /// <param name="IndexOper">Почтовый индекс места проведения операции</param>
        public Operation(string OperTypeID, string OperCtgID, string OperName, string DateOper, string IndexOper)
        {
            OperationParameters = new OperationParameters(OperTypeID, OperCtgID, OperName, DateOper);
            AddressParameters = new AddressParameters(IndexOper);
            FinanceParameters = new FinanceParameters();
            ItemParameters = new ItemParameters();
            UserParameters = new UserParameters();
        }
    }
}
