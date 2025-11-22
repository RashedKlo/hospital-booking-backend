using Microsoft.Extensions.Configuration;

namespace hospital_booking.Data.Settings
{
    public static class DatabaseSettings
    {
        private static readonly string _connectionString;

        static DatabaseSettings()
        {
            try
            {
                var builder = new ConfigurationBuilder()
                    .SetBasePath(AppContext.BaseDirectory)
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

                var config = builder.Build();
                _connectionString = config.GetConnectionString("DefaultConnection") ?? string.Empty;
            }
            catch
            {
                _connectionString = string.Empty;
            }
        }

        public static string ConnectionString => _connectionString;
    }
}