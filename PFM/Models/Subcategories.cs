using System;
using System.Collections.Generic;

namespace PFM.Models
{
    public partial class Subcategories
    {
        public Subcategories()
        {
            Transactions = new HashSet<Transactions>();
        }

        public int SubcategoryId { get; set; }
        public int CategoryId { get; set; }
        public int Value { get; set; }
        public string SubcategoryName { get; set; }

        public Categories Category { get; set; }
        public ICollection<Transactions> Transactions { get; set; }
    }
}
