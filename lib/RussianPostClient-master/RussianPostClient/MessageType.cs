namespace RussianPost.Tracking
{
    /// <summary>
    /// Тип сообщения
    /// </summary>
    public enum MessageType
    {
        /// <summary>
        /// История операций для отправления
        /// </summary>
        HistoryOperations = 0,

        /// <summary>
        /// История операций для заказного уведомления по данному отправлению
        /// </summary>
        HistoryNotifications = 1
    }
}