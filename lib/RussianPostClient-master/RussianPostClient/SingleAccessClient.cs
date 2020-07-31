namespace RussianPost.Tracking
{
    using RussianPost.Tracking.Service;
    using System;
    using Properties;

    /// <summary>
    /// Клиент Единичного доступа позволяет за одно обращение к Сервису отслеживания получить информацию по одному регистрируемому почтовому отправлению (РПО)
    /// </summary>
    public sealed class SingleAccessClient : ISingleAccessTrackingClient
    {
        public const string RussianLanguage = "RUS";
        public const string EnglishLanguage = "ENG";

        public const string UserLoginKey = "Login";
        public const string UserPasswordKey = "Password";

        private readonly OperationHistory12Client _russianPostService;

        private readonly AuthorizationData _authorizationData;

        public SingleAccessClient()
        {
            _authorizationData = new AuthorizationData();

            InitAuthData(_authorizationData);

            _russianPostService = new OperationHistory12Client();
        }

        private void InitAuthData(AuthorizationData authdata)
        {
            authdata.Login = !string.IsNullOrEmpty(Settings.Default.Login) ? Settings.Default.Login : string.Empty;
            authdata.Password = !string.IsNullOrEmpty(Settings.Default.Password) ? Settings.Default.Password : string.Empty;
        }

        private AuthorizationHeader ConvertAuthDataToAuthHeader(AuthorizationData authdata)
        {
            if (authdata == null)
            {
                throw new ArgumentNullException(nameof(authdata), "The authorization data parameter is null");
            }

            return new AuthorizationHeader
            {
                login = authdata.Login,
                password = authdata.Password,
                mustUnderstand = "1"
            };
        }

        public CustomDutyEvent[] GetCustomDutyEventsForMail(Rpo rpo)
        {
            throw new System.NotImplementedException();
        }

        public LanguageDataLanguage[] GetLanguages()
        {
            throw new System.NotImplementedException();
        }

        public OperationHistoryRecord[] GetOperationHistory(Rpo rpo)
        {
            if (rpo == null)
            {
                throw new ArgumentNullException(nameof(rpo), "The RPO parameter is null");
            }

            var operaationHistoryRequest = new OperationHistoryRequest
            {
                Barcode = rpo.Barcode,
                MessageType = (int)rpo.MessageType
            };

            var authHeader = ConvertAuthDataToAuthHeader(_authorizationData);
            return _russianPostService.getOperationHistory(operaationHistoryRequest, authHeader);
        }

        public SmsHistoryRecord[] GetSmsHistory(Rpo rpo)
        {
            throw new System.NotImplementedException();
        }

        public PostalOrderEvent[] PostalOrderEventsForMail(Rpo rpo)
        {
            throw new System.NotImplementedException();
        }
    }
}
