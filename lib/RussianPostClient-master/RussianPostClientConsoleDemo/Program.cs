using RussianPost.Tracking;
using System;
using System.Text;

namespace RussianPostClientConsoleDemo
{
    public class Program
    {
        private static void Main(string[] args)
        {
            var client = new SingleAccessClient();

            var rpo = Rpo.Default;

            rpo.Barcode = "61499534037414";


            var records = client.GetOperationHistory(rpo);

            var stringBuilder = new StringBuilder("Посылка прошла этапы:\n");
            foreach (var record in records)
            {
                stringBuilder.AppendFormat("---> {0} : {1}\n", record.OperationParameters.OperAttr.Name, record.OperationParameters.OperDate);
            }

            Console.Write(stringBuilder);

          
            Console.ReadKey();
        }
    }
}
