using System;
using System.Data;
using hospital_booking.Data.DTOs.User;
using hospital_booking.Data.Helpers;
using Microsoft.Data.SqlClient;

namespace hospital_booking.Data.Repositories.User.Helpers
{
    public static class UserMapper
    {
        public static Models.User MapUserFromReader(SqlDataReader reader)
        {
            return new Models.User
            {
                UserId = reader.GetSafeInt32("user_id"),
                Email = reader.GetSafeString("email"),
                Password = reader.GetSafeString("password"),
                FullName = reader.GetSafeString("fullname"),

            };
        }
    }
}