using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PFM.Models
{
    public partial class User
    {
        public User()
        {
            Categories = new HashSet<Categories>();
            Transactions = new HashSet<Transactions>();
        }

        [Required]
        public int UserId { get; set; }
        [Required]
        public string UserFirstName { get; set; }
        [Required]
        public string UserLastName { get; set; }
        [Required]
        public string FullName { get { return UserFirstName + " " + UserLastName; } }
        [Required]
        public string Email { get; set; }
        [NotMapped]
        [Compare("Email")]
        public string ConfirmEmail { get; set; }
        [Required]
        public string Password { get; set; }
        [NotMapped]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }

        public ICollection<Categories> Categories { get; set; }
        public ICollection<Transactions> Transactions { get; set; }
    }

}
