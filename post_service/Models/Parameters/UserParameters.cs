using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
