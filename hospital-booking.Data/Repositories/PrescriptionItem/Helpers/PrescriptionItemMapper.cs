using System;
using Microsoft.Data.SqlClient;
using hospital_booking.Data.DTOs.PrescriptionItem;
using hospital_booking.Data.DTOs.Prescription;

namespace hospital_booking.Data.Repositories.PrescriptionItem.Helpers
{
    public static class PrescriptionItemMapper
    {
        public static PrescriptionItemDto MapFromReader(SqlDataReader reader)
        {
            var item = new PrescriptionItemDto
            {
                PrescriptionItemId = reader.GetInt32(0),
                PrescriptionId = reader.GetInt32(1),
                MedicationName = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                Dosage = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                Duration = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
                Frequency = reader.IsDBNull(5) ? string.Empty : reader.GetString(5)
            };

            // Check if Prescription columns are present (start index 6)
            if (reader.FieldCount > 6)
            {
                item.Prescription = new PrescriptionDto
                {
                    PrescriptionId = reader.GetInt32(6),
                    AppointmentId = reader.GetInt32(7),
                    Instructions = reader.IsDBNull(8) ? string.Empty : reader.GetString(8)
                };
            }

            return item;
        }
    }
}
