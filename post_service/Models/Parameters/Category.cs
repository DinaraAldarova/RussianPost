using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace post_service.Models.Parameters
{
    /// <summary>
    /// 
    /// </summary>
    public class Category
    {
        /// <summary>
        /// Код
        /// </summary>
        public string Id { get; private set; }

        /// <summary>
        /// Название
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">Код</param>
        /// <param name="name">Название</param>
        public Category(string id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
