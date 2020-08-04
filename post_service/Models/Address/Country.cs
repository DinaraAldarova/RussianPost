using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace post_service.Models.Address
{
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
        public string NameRu { get; private set; }

        /// <summary>
        /// Международное название страны
        /// </summary>
        public string NameEN { get; private set; }

        public Country(string id, string code2A, string code3A, string nameRu, string nameEN)
        {
            Id = id;
            Code2A = code2A;
            Code3A = code3A;
            NameRu = nameRu;
            NameEN = nameEN;
        }
    }
}
