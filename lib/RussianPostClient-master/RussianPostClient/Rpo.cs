namespace RussianPost.Tracking
{
    /// <summary>
    /// Зарегистрированное почтовое отправление (РПО)
    /// </summary>
    public sealed class Rpo
    {
        /// <summary>
        /// Идентификатор РПО в одном из форматов: внутрироссийский [14 цифр] / международный [13 символов в формате S10]
        /// </summary>
        public string Barcode { get; set; }

        /// <summary>
        /// Тип сообщения
        /// </summary>
        public MessageType MessageType { get; set; }

        /// <summary>
        /// Язык
        /// </summary>
        public string Language { get; set; }

        /// <summary>
        /// Дефолтный РПО: требуется заполнить Barcode
        /// </summary>
        public static Rpo Default => new Rpo { MessageType = MessageType.HistoryOperations, Language = "RUS" };
        }
}