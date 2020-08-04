using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace post_service.Models.Address
{
    public class AddressParameters
    {
        /// <summary>
        /// Содержит данные о стране места назначения пересылки отправления.
        /// </summary>
        public Country MailDirect { get; private set; }

        /// <summary>
        /// Содержит данные о стране приема почтового отправления.
        /// </summary>
        public Country CountryFrom { get; private set; }

        /// <summary>
        /// Содержит данные о стране проведения операции над почтовым отправлением.
        /// </summary>
        public Country CountryOper { get; private set; }
    }
}
