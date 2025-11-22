using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace hospital_booking.Data.Settings
{
    public class JwtSettings
    {
        public string SigningKey { get; set; } = string.Empty;
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        public int ExpirationInMinutes { get; set; } = 60;

        public static JwtSettings LoadFromConfiguration(IConfiguration configuration)
        {
            var settings = new JwtSettings();
            var section = configuration.GetSection("Jwt");
            if (section.Exists())
            {
                settings.SigningKey = section["SigningKey"] ?? string.Empty;
                settings.Issuer = section["Issuer"] ?? string.Empty;
                settings.Audience = section["Audience"] ?? string.Empty;
                if (int.TryParse(section["ExpirationInMinutes"], out int expiration))
                    settings.ExpirationInMinutes = expiration;
            }
            return settings;
        }
    }
}

