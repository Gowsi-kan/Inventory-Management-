using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagement.Models
{
    // Response model for login
    public class LoginResponse
    {
        public string Token { get; set; }
        public User User { get; set; }
    }

    // User model matching the API response
    public class User
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public Int16 UserPrivilege { get; set; }
    }
}
