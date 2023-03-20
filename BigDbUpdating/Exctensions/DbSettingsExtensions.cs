using BigDbUpdating.Database.Settings;
using Npgsql;

namespace BigDbUpdating.Exctensions
{
    /// <summary>
    ///     Методы расширения DbSettings
    /// </summary>
    public static class DbSettingsExtensions
    {
        /// <summary>
        ///     Возвращает строку подключения к БД
        /// </summary>
        /// <param name="settings"></param>
        public static string GetConnectionString(this DbSettings settings)
        {
            var connectionStringBuilder = new NpgsqlConnectionStringBuilder
            {
                Host = settings.Host,
                Port = settings.Port,
                Database = settings.DbName,
                Username = settings.User,
                Password = settings.Password,
                MaxPoolSize = settings.MaxPoolSize,
                NoResetOnClose = settings.NoResetOnClose
            };

            return connectionStringBuilder.ConnectionString;
        }
    }
}