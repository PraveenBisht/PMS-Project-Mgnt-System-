using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PMS.Models
{
    public class ProjectViewModel
    {
        public int ProjectID { get; set; }

        //[Required(ErrorMessage = "Enter Project Code")]
        //public string ProjectCode { get; set; }

        [Required(ErrorMessage = "Enter ProjectName")]
        public string ProjectName { get; set; }
        public string Projectdescription { get; set; }
        public string status { get; set; }
        public string createDate { get; set; }
    }
}