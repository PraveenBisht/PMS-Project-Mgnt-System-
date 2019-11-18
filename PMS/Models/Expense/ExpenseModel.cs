using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PMS.Models.Expense
{
    public class ExpenseModel
    {
        public int ID { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Date")]
   
        public DateTime ExpenseDate { get; set; }
        public string expensedatetime { get; set; }
        [Required]
        [DataType(DataType.Currency)]
        [Display(Name = "Amount")]
        public decimal Amount { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "Tag As")]
        public string Tag { get; set; }

    }
}