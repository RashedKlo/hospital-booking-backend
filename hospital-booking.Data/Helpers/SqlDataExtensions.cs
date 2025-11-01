using System;
using System.Data;

namespace hospital_booking.Data.Helpers
{
    public static class SqlDataExtensions
    {
        /// <summary>
        /// Safely gets a string value from IDataReader, returns empty string if NULL
        /// </summary>
        public static string GetSafeString(this IDataReader reader, string columnName)
        {
            int ordinal = reader.GetOrdinal(columnName);
            return reader.IsDBNull(ordinal) ? string.Empty : reader.GetString(ordinal);
        }

        /// <summary>
        /// Safely gets an integer value from IDataReader, returns default value if NULL
        /// </summary>
        public static int GetSafeInt32(this IDataReader reader, string columnName, int defaultValue = 0)
        {
            int ordinal = reader.GetOrdinal(columnName);
            return reader.IsDBNull(ordinal) ? defaultValue : reader.GetInt32(ordinal);
        }

        /// <summary>
        /// Safely gets a nullable integer value from IDataReader
        /// </summary>
        public static int? GetNullableInt32(this IDataReader reader, string columnName)
        {
            int ordinal = reader.GetOrdinal(columnName);
            return reader.IsDBNull(ordinal) ? null : reader.GetInt32(ordinal);
        }

        /// <summary>
        /// Safely gets a boolean value from IDataReader, returns default value if NULL
        /// </summary>
        public static bool GetSafeBoolean(this IDataReader reader, string columnName, bool defaultValue = false)
        {
            int ordinal = reader.GetOrdinal(columnName);
            return reader.IsDBNull(ordinal) ? defaultValue : reader.GetBoolean(ordinal);
        }

        /// <summary>
        /// Safely gets a DateTime value from IDataReader, returns default value if NULL
        /// </summary>
        public static DateTime GetSafeDateTime(this IDataReader reader, string columnName, DateTime? defaultValue = null)
        {
            int ordinal = reader.GetOrdinal(columnName);
            return reader.IsDBNull(ordinal) ? (defaultValue ?? DateTime.MinValue) : reader.GetDateTime(ordinal);
        }

        /// <summary>
        /// Safely gets a nullable DateTime value from IDataReader
        /// </summary>
        public static DateTime? GetNullableDateTime(this IDataReader reader, string columnName)
        {
            int ordinal = reader.GetOrdinal(columnName);
            return reader.IsDBNull(ordinal) ? null : reader.GetDateTime(ordinal);
        }

        /// <summary>
        /// Safely gets a decimal value from IDataReader, returns default value if NULL
        /// </summary>
        public static decimal GetSafeDecimal(this IDataReader reader, string columnName, decimal defaultValue = 0)
        {
            int ordinal = reader.GetOrdinal(columnName);
            return reader.IsDBNull(ordinal) ? defaultValue : reader.GetDecimal(ordinal);
        }

        /// <summary>
        /// Safely gets a nullable decimal value from IDataReader
        /// </summary>
        public static decimal? GetNullableDecimal(this IDataReader reader, string columnName)
        {
            int ordinal = reader.GetOrdinal(columnName);
            return reader.IsDBNull(ordinal) ? null : reader.GetDecimal(ordinal);
        }

        /// <summary>
        /// Safely gets a long value from IDataReader, returns default value if NULL
        /// </summary>
        public static long GetSafeInt64(this IDataReader reader, string columnName, long defaultValue = 0)
        {
            int ordinal = reader.GetOrdinal(columnName);
            return reader.IsDBNull(ordinal) ? defaultValue : reader.GetInt64(ordinal);
        }

        /// <summary>
        /// Safely gets a GUID value from IDataReader, returns Guid.Empty if NULL
        /// </summary>
        public static Guid GetSafeGuid(this IDataReader reader, string columnName)
        {
            int ordinal = reader.GetOrdinal(columnName);
            return reader.IsDBNull(ordinal) ? Guid.Empty : reader.GetGuid(ordinal);
        }

        /// <summary>
        /// Safely gets a nullable GUID value from IDataReader
        /// </summary>
        public static Guid? GetNullableGuid(this IDataReader reader, string columnName)
        {
            int ordinal = reader.GetOrdinal(columnName);
            return reader.IsDBNull(ordinal) ? null : reader.GetGuid(ordinal);
        }
    }
}