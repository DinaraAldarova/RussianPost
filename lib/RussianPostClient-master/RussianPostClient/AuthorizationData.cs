namespace RussianPost.Tracking
{
    /// <summary>
    /// Авторизационные данные
    /// </summary>
    public sealed class AuthorizationData
    {
        /// <summary>
        /// Логин пользователя
        /// </summary>
        public string Login { get; set; }

        /// <summary>
        /// Пароль пользователя
        /// </summary>
        public string Password { get; set; }
    }
}