using System;
using System.Collections.Generic;

namespace PFM.Models
{
    public partial class Categories
    {
        public Categories()
        {
            Subcategories = new HashSet<Subcategories>();
        }

        public int CategoryId { get; set; }
        public int UserId { get; set; }
        public string CategoryName { get; set; }

        public User User { get; set; }
        public ICollection<Subcategories> Subcategories { get; set; }
    }
}
