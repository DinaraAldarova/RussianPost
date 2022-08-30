namespace post_service.Code
{
    /// <summary>
    /// Хранит информацию для авторизации при работе с API Почты России
    /// </summary>
    public class AuthInfo
    {
        /// <summary>
        /// Логин пользователя
        /// </summary>
        public string Login { get; private set; }

        /// <summary>
        /// Пароль пользователя
        /// </summary>
        public string Password { get; private set; }

        /// <summary>
        /// Создание объекта, хранящего логин и пароль пользователя
        /// </summary>
        /// <param name="login">Логин пользователя</param>
        /// <param name="password">Пароль пользователя</param>
        public AuthInfo(string login, string password)
        {
            Login = login;
            Password = password;
        }
    }
}
