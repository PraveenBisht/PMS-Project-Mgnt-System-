using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace PMS.Models.Expense
{
    public class WithdrawalViewModel 
    {
        public int Id { get; set; }
        public Nullable<decimal> amount { get; set; }
        public string UserID { get; set; }
    
        public Nullable<System.DateTime> createddate { get; set; }
    }
}