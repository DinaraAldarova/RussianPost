﻿using System;
using System.Xml;

namespace post_service.Models.Parameters
{
    /// <summary>
    /// Используется для хранения финансовых данных
    /// </summary>
    public class FinanceParameters
    {
        /// <summary>
        /// Сумма наложенного платежа в копейках
        /// </summary>
        public string Payment { get; private set; }

        /// <summary>
        /// Сумма объявленной ценности в копейках
        /// </summary>
        public string Value { get; private set; }

        /// <summary>
        /// Общая сумма платы за пересылку наземным и воздушным транспортом в копейках
        /// </summary>
        public string MassRate { get; private set; }

        /// <summary>
        /// Сумма платы за объявленную ценность в копейках
        /// </summary>
        public string InsrRate { get; private set; }

        /// <summary>
        /// Выделенная сумма платы за пересылку воздушным транспортом из общей суммы платы за пересылку в копейках
        /// </summary>
        public string AirRate { get; private set; }

        /// <summary>
        /// Сумма дополнительного тарифного сбора в копейках
        /// </summary>
        public string Rate { get; private set; }

        /// <summary>
        /// Сумма таможенного платежа в копейках
        /// </summary>
        public string CustomDuty { get; private set; }

        /// <summary>
        /// Задает значения по-умолчанию для пустого объекта
        /// </summary>
        public FinanceParameters()
        {
            Payment = "";
            Value = "";
            MassRate = "";
            InsrRate = "";
            AirRate = "";
            Rate = "";
            CustomDuty = "";
        }

        /// <summary>
        /// Создание параметров
        /// </summary>
        /// <param name="payment">Сумма наложенного платежа в копейках</param>
        /// <param name="value">Сумма объявленной ценности в копейках</param>
        /// <param name="massRate">Общая сумма платы за пересылку наземным и воздушным транспортом в копейках</param>
        /// <param name="insrRate">Сумма платы за объявленную ценность в копейках</param>
        /// <param name="airRate">Выделенная сумма платы за пересылку воздушным транспортом из общей суммы платы за пересылку в копейках</param>
        /// <param name="rate">Сумма дополнительного тарифного сбора в копейках</param>
        /// <param name="customDuty">Сумма таможенного платежа в копейках</param>
        public FinanceParameters(string payment, string value, string massRate, string insrRate, string airRate, string rate, string customDuty)
        {
            Payment = payment;
            Value = value;
            MassRate = massRate;
            InsrRate = insrRate;
            AirRate = airRate;
            Rate = rate;
            CustomDuty = customDuty;
        }

        /// <summary>
        /// Создание параметров из XML-структуры FinanceParameters
        /// </summary>
        /// <param name="FinanceParameters">XML-структура FinanceParameters</param>
        public FinanceParameters(XmlNode FinanceParameters)
        {
            Payment = "";
            Value = "";
            MassRate = "";
            InsrRate = "";
            AirRate = "";
            Rate = "";
            CustomDuty = "";
            foreach (XmlNode parameter in FinanceParameters)
            {
                switch (parameter.Name)
                {
                    case "ns3:Payment":
                        Payment = parameter.InnerText;
                        break;
                    case "ns3:Value":
                        Value = parameter.InnerText;
                        break;
                    case "ns3:MassRate":
                        MassRate = parameter.InnerText;
                        break;
                    case "ns3:InsrRate":
                        InsrRate = parameter.InnerText;
                        break;
                    case "ns3:AirRate":
                        AirRate = parameter.InnerText;
                        break;
                    case "ns3:Rate":
                        Rate = parameter.InnerText;
                        break;
                    case "ns3:CustomDuty":
                        CustomDuty = parameter.InnerText;
                        break;
                    default:
                        throw new Exception();
                }
            }
        }
    }
}
