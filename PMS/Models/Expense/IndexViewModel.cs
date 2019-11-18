using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace PMS.Models.Expense
{
    public class IndexViewsModel: BaseModel
    {
        [Required]
        [Display(Name = "Tag As")]
        public IEnumerable<SelectListItem> TagList { get; set; }
        public IEnumerable<SelectListItem> MonthsList { get; set; }
      
        public IHtmlString ExpenseTagData { get; set; }
        public List<ExpenseModel> Expenses { get; set; }
        public ExpenseModel ExpenseModelEntry { get; set; }
    }
}