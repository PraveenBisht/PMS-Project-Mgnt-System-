using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMS.Models.MobileApisModels
{
    public class EmpTaskListVM
    {
        public string TaskId { get; set; } = "";
        public string ProjectName { get; set; } = "";
        public string TaskName { get; set; } = "";
        public string Createdby { get; set; } = "";
        public string CreatedTo { get; set; } = "";
        public string Status { get; set; } = "";
        public string Description { get; set; } = "";
        public string Createdon { get; set; } = "";
    }
    public class Img
    {
        public int imageid { get; set; }
        public string imageName { get; set; } = "";
    }
}