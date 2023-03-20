namespace BigDbUpdating.Database.Settings
{
    /// <summary>
    ///     Конфигурация базы данных
    /// </summary>
    public class DbSettings
    {
        /// <summary>
        ///     Имя пользователя
        /// </summary>
        public string User { get; set; }

        /// <summary>
        ///     Пароль
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        ///     Хост
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        ///     Порт
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        ///     Имя базы данных
        /// </summary>
        public string DbName { get; set; }

        /// <summary>
        ///     Максимальное число одновременных соединений
        /// </summary>
        public int MaxPoolSize { get; set; }

        /// <summary>
        ///     В некоторых случаях повышает производительность,
        ///     не сбрасывая состояние соединения при его возврате в пул,
        ///     за счет утечки состояния.
        ///     Используйте только в том случае, если сравнительный анализ показывает улучшение производительности.
        /// </summary>
        public bool NoResetOnClose { get; set; }
    }
}