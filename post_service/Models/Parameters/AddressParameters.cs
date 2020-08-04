using post_service.Models.Parameters.Address;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public City DestinationAddress { get; private set; }

        /// <summary>
        /// Адресные данные места проведения операции над отправлением
        /// </summary>
        public City OperationAddress { get; private set; }

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
        /// 
        /// </summary>
        /// <param name="destinationAddress">Адресные данные места назначения отправления</param>
        /// <param name="operationAddress">Адресные данные места проведения операции над отправлением</param>
        /// <param name="mailDirect">Данные о стране места назначения пересылки отправления</param>
        /// <param name="countryFrom">Данные о стране приема почтового отправления</param>
        /// <param name="countryOper">Данные о стране проведения операции над почтовым отправлением</param>
        public AddressParameters(City destinationAddress, City operationAddress, Country mailDirect, Country countryFrom, Country countryOper)
        {
            MailDirect = mailDirect;
            CountryFrom = countryFrom;
            CountryOper = countryOper;
            DestinationAddress = destinationAddress;
            OperationAddress = operationAddress;
        }
    }
}
