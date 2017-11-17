using System;
using System.Collections.Generic;

namespace PFM.Models
{
    public partial class User
    {
        public User()
        {
            Categories = new HashSet<Categories>();
            Transactions = new HashSet<Transactions>();
        }

        public int UserId { get; set; }
        public string UserFirstName { get; set; }
        public string UserLastName { get; set; }
        public string FullName { get { return UserFirstName + " " + UserLastName; } }
        public string Email { get; set; }
        public string Password { get; set; }

        public ICollection<Categories> Categories { get; set; }
        public ICollection<Transactions> Transactions { get; set; }
    }
}
