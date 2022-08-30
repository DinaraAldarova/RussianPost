using System.Text.RegularExpressions;
using post_service.Code;

namespace post_service.Models
{
    /// <summary>
    /// Хранение ШПИ и соответствующего ему УИН
    /// </summary>
    public class Barcode
    {
        /// <summary>
        /// ШПИ, номер отправления
        /// </summary>
        public string Code { get; private set; }

        /// <summary>
        /// УИН
        /// </summary>
        public string UIN { get; private set; }

        /// <summary>
        /// Создание объекта по ШПИ
        /// </summary>
        /// <param name="code">ШПИ</param>
        public Barcode(string code)
        {
            Code = code;
            UIN = "";
        }

        /// <summary>
        /// Создание объекта по ШПИ и УИН
        /// </summary>
        /// <param name="code">ШПИ</param>
        /// <param name="uin">УИН</param>
        public Barcode(string code, string uin)
        {
            Code = code;
            UIN = uin;
        }

        /// <summary>
        /// Конвертация в строку
        /// </summary>
        /// <param name="barcode">Объект, содержащий ШПИ и УИН</param>
        /// <returns>Строка, содержащая ШПИ и УИН</returns>
        public static string ConvertToString(Barcode barcode)
        {
            return barcode.Code + " " + barcode.UIN;
        }

        /// <summary>
        /// Конертация из строки
        /// </summary>
        /// <param name="barcode">Строка, содержащая ШПИ и УИН</param>
        /// <returns>Объект, содержащий ШПИ и УИН</returns>
        public static Barcode ConvertFromString(string barcode)
        {
            if (Regex.IsMatch(barcode, "[0-9]{14} [0-9]{24,25}( )*")) 
            {
                string[] splits = barcode.Split(' ');
                return new Barcode(splits[0], splits[1]);
            }
            Logger.Log.Error($"При чтении файла с ШПИ встречена строка, не соответствующая формату: {barcode}");
            return new Barcode("", "");
        }
    }
}
