using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMS.Models
{

        public class TaskViewModel
       {
        public int id { get; set; }
        public string taskId { get; set; }
        [Required(ErrorMessage = "Please Enter description")]
        [Display(Name = "description")]
        [MaxLength(500)]
        public string description { get; set; }

        public string createdby { get; set; }
        public string CreatedTo { get; set; }

        [Required(ErrorMessage = "Please Enter YourTask  Name")]
        [Display(Name = "summaryName")]
        [MaxLength(100)]
        public string TaskName { get; set; }
        public string Logintype { get; set; }
        public string Devicetype { get; set; }
        public string imageurl { get; set; }
        public List<TaskViewModel> FileDetails { get; internal set; }
        public string CountAttechedFile { get; set; }
        [Required(ErrorMessage = "Please select Project Name")]
        [Display(Name = "projectList")]
        [MaxLength(100)]
        public string projectList { get; set; }
        public string projectid { get; set; }
        public string projectname { get; set; }
        public string Createdon { get; set; }
        public string status { get; set; }

        public List<HttpPostedFileBase> postedFiles { get; set; }

    }
}