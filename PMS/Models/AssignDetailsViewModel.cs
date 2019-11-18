using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMS.Models
{
    public class AssignDetailsViewModel
    {
        public int id { get; set; } = 0;
        public string taskId { get; set; } = "";
        public string description { get; set; } = "";
        public string createdby { get; set; } = "";
        public string TaskName { get; set; } = "";
        public string projectname { get; set; } = "";
        public string Createdon { get; set; } = "";
        public string status { get; set; } = "";
        public string AssignTouserName { get; set; } = "";
        public string AssignFromUserName { get; set; } = "";
        public string Devicetype { get; set; } = "";
    }
}