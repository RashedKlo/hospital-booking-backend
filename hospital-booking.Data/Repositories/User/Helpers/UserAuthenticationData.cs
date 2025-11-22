using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using hospital_booking.Data.Models;

namespace hospital_booking.Data.Repositories.User.Helpers
{
    public class UserAuthenticationData
    {
        public Models.User? User { get; set; }
        public string AccessToken { get; set; }
        public UserAuthenticationData(Models.User user, string AccessToken)
        {
            this.User = user;
            this.AccessToken = AccessToken;
        }
    }
}