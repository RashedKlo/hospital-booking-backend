using System;
using Microsoft.Data.SqlClient;
using hospital_booking.Data.DTOs.PrescriptionItem;

namespace hospital_booking.Data.Repositories.PrescriptionItem.Helpers
{
    public static class PrescriptionItemMapper
    {
        public static PrescriptionItemDto MapFromReader(SqlDataReader reader)
        {
            return new PrescriptionItemDto
            {
                ItemId = reader.GetInt32(0),
                PrescriptionId = reader.GetInt32(1),
                Name = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                Dosage = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                Duration = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
                Frequency = reader.IsDBNull(5) ? string.Empty : reader.GetString(5)
            };
        }
    }
}
