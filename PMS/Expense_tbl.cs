//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PMS
{
    using System;
    using System.Collections.Generic;
    
    public partial class Expense_tbl
    {
        public int ID { get; set; }
        public Nullable<System.DateTime> ExpenseDate { get; set; }
        public string Description { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public Nullable<int> TagID { get; set; }
        public string UserID { get; set; }
    
        public virtual Tag_tbl Tag_tbl { get; set; }
    }
}
