using System;
using System.Collections.Generic;

namespace PFM.Models
{
    public partial class Transactions
    {
        public int TransactionsId { get; set; }
        public int UserId { get; set; }
        public int SubcategoryId { get; set; }
        public int Value { get; set; }

        public Subcategories Subcategory { get; set; }
        public User User { get; set; }
    }
}
