namespace RussianPost.Tracking
{
    using RussianPost.Tracking.Service;

    public interface ISingleAccessTrackingClient
    {
        /// <summary>
        /// Получение подробной информации обо всех операциях, совершенных над отправлением
        /// </summary>
        /// <param name="rpo">РПО</param>
        /// <returns>Массив записей, где каждая запись содержит информацию об одной операции, совершенной над отправлением</returns>
        OperationHistoryRecord[] GetOperationHistory(Rpo rpo);

        /// <summary>
        /// Получить историю sms-уведомлений
        /// </summary>
        /// <param name="rpo">РПО</param>
        /// <returns>Массив записей, где каждая запись содержит информацию по одному уведлмлению</returns>
        SmsHistoryRecord[] GetSmsHistory(Rpo rpo);

        /// <summary>
        /// Получить список используемых языков
        /// </summary>
        /// <returns>Массив записей, где каждая запись содержит информацию об используемом языке</returns>
        LanguageDataLanguage[] GetLanguages();

        /// <summary>
        /// Получить информацию об операциях с таможенной пошлиной, которая связана с конкретным почтовым отправлением
        /// </summary>
        /// <param name="rpo">РПО</param>
        /// <returns>Массив событий таможенной пошлины</returns>
        CustomDutyEvent[] GetCustomDutyEventsForMail(Rpo rpo);

        /// <summary>
        /// Получить информацию об операциях с наложенным платежом, который связан с конкретным почтовым отправлением
        /// </summary>
        /// <param name="rpo">РПО</param>
        /// <returns>Массив событий почтового заказа</returns>
        PostalOrderEvent[] PostalOrderEventsForMail(Rpo rpo);
    }
}
