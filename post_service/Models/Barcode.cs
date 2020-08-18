using System;
using System.Collections.Generic;
using System.IO;
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

        public static string BarcodeToString(Barcode barcode)
        {
            return barcode.Code;
        }

        public static Barcode StringToBarcode(string code)
        {
            return new Barcode(code);
        }

        public static void WriteBarcodesToFile(List<Barcode> barcodes, string path)
        {
            File.WriteAllLines(path, barcodes.ConvertAll(new Converter<Barcode, string>(BarcodeToString)));
        }

        public static List<Barcode> ReadBarcodesFromFile(string path)
        {
            List<string> strBarcodes = new List<string>(File.ReadAllLines(path));
            List<Barcode> barcodes = strBarcodes.ConvertAll(new Converter<string, Barcode>(StringToBarcode));
            return barcodes;
        }
    }
}
