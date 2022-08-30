using log4net;
using log4net.Config;

namespace post_service.Code
{
    /// <summary>
    /// Класс для работы логирования
    /// </summary>
    public static class Logger
    {
        //Примеры использования:
        //Logger.InitLoger(); - инициализация логирования, один раз за работу программы
        //Logger.Log.Info("Информационный текст"); - добавление записиси с пометкой INFO
        //Logger.Log.Error("Текст ошибки"); - добавление записиси с пометкой ERROR

        private static ILog log = LogManager.GetLogger("LOGGER");

        public static ILog Log
        {
            get { return log; }
        }

        public static void InitLogger()
        {
            XmlConfigurator.Configure();
        }
    }
}
