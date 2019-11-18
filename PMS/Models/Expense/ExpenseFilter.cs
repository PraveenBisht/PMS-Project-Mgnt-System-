using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PMS.Models.Expense
{
    public class ExpenseFilter
    {
        public ExpenseFilter()
        {
            FromDate = DateTime.Now;
            ToDate = DateTime.Now;
        }

        [DataType(DataType.Date)]
        public DateTime FromDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime ToDate { get; set; }
    }
}