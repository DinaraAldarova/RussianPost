using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace post_service.Models
{
    public class Barcode
    {
        public string Code { get; private set; }
        public string UIN { get; private set; }

        public Barcode(string code)
        {
            Code = code;
            UIN = "";
        }

        public Barcode(string code, string uin)
        {
            Code = code;
            UIN = uin;
        }

        public static string BarcodeToString(Barcode barcode)
        {
            return barcode.Code + " " + barcode.UIN;
        }

        public static Barcode StringToBarcode(string codebarcode)
        {
            if (Regex.IsMatch(codebarcode, "один из двух типов ШПИ, пробел, запись UIN")) { }
            string[] splits = codebarcode.Split(' ');
            return new Barcode(splits[0], splits.ElementAt(1));
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
