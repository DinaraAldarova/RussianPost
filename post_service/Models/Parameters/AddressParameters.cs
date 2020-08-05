using post_service.Models.Parameters.Address;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace post_service.Models.Parameters
{
    /// <summary>
    /// Используется для хранения адресных данных
    /// </summary>
    public class AddressParameters
    {
        /// <summary>
        /// Адресные данные места назначения отправления
        /// </summary>
        public Place DestinationAddress { get; private set; }

        /// <summary>
        /// Адресные данные места проведения операции над отправлением
        /// </summary>
        public Place OperationAddress { get; private set; }

        /// <summary>
        /// Данные о стране места назначения пересылки отправления
        /// </summary>
        public Country MailDirect { get; private set; }

        /// <summary>
        /// Данные о стране приема почтового отправления
        /// </summary>
        public Country CountryFrom { get; private set; }

        /// <summary>
        /// Данные о стране проведения операции над почтовым отправлением
        /// </summary>
        public Country CountryOper { get; private set; }

        /// <summary>
        /// Задает значения по-умолчанию для пустого объекта
        /// </summary>
        public AddressParameters()
        {
            MailDirect = new Country();
            CountryFrom = new Country();
            CountryOper = new Country();
            DestinationAddress = new Place();
            OperationAddress = new Place();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="destinationAddress">Адресные данные места назначения отправления</param>
        /// <param name="operationAddress">Адресные данные места проведения операции над отправлением</param>
        /// <param name="mailDirect">Данные о стране места назначения пересылки отправления</param>
        /// <param name="countryFrom">Данные о стране приема почтового отправления</param>
        /// <param name="countryOper">Данные о стране проведения операции над почтовым отправлением</param>
        public AddressParameters(Place destinationAddress, Place operationAddress, Country mailDirect, Country countryFrom, Country countryOper)
        {
            MailDirect = mailDirect;
            CountryFrom = countryFrom;
            CountryOper = countryOper;
            DestinationAddress = destinationAddress;
            OperationAddress = operationAddress;
        }

        /// <summary>
        /// Создание параметров из XML-структуры AddressParameters
        /// </summary>
        /// <param name="AddressParameters">XML-структура AddressParameters</param>
        public AddressParameters(XmlNode AddressParameters)
        {
            MailDirect = new Country();
            CountryFrom = new Country();
            CountryOper = new Country();
            DestinationAddress = new Place();
            OperationAddress = new Place();
            foreach (XmlNode parameter in AddressParameters)
            {
                switch (parameter.Name)
                {
                    case "ns3:MailDirect":
                        MailDirect = new Country(parameter);
                        break;
                    case "ns3:CountryFrom":
                        CountryFrom = new Country(parameter);
                        break;
                    case "ns3:CountryOper":
                        CountryOper = new Country(parameter);
                        break;
                    case "ns3:DestinationAddress":
                        DestinationAddress = new Place(parameter);
                        break;
                    case "ns3:OperationAddress":
                        OperationAddress = new Place(parameter);
                        break;
                    default:
                        throw new Exception();
                }
            }
        }

        /// <summary>
        /// Создание параметров по информации, предоставленной в пакетном запросе
        /// </summary>
        /// <param name="IndexOper">Почтовый индекс места проведения операции</param>
        public AddressParameters(string IndexOper)
        {
            MailDirect = new Country();
            CountryFrom = new Country();
            CountryOper = new Country();
            DestinationAddress = new Place();
            OperationAddress = new Place(IndexOper);
        }
    }
}
