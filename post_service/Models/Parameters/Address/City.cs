using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace post_service.Models.Parameters.Address
{
    /// <summary>
    /// Используется для атрибутов DestinationAddress, OperationAddress класса AddressParameters
    /// </summary>
    public class City
    {
        /// <summary>
        /// Почтовый индекс места
        /// </summary>
        public string Index { get; private set; }

        /// <summary>
        /// Адрес и/или название места
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index">Почтовый индекс места</param>
        /// <param name="description">Адрес и/или название места</param>
        public City(string index, string description)
        {
            Index = index;
            Description = description;
        }
    }
}
