using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace post_service.Models
{
    public class Barcode
    {
        public string Code { get; private set; }

        public Barcode(string code)
        {
            Code = code;
        }
    }
}
