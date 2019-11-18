using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMS.Models
{
    public class UserViewModel
    {
        public string id { get; set; }
        public int EmplyoeeId { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string Createddate { get; set; }
        public string Leave_status { get; set; }
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public string designation { get; set; }
        public string address { get; set; }
        public string mobilenumber { get; set; }
        public string qualification { get; set; }
        public string joiningDate { get; set; }
        public string profilePic { get; set; }


    }
    public class AttendanceViewModel
    {
        public bool iscoming { get; set; }
        public bool isLeave { get; set; }
        public bool AttBtnHideShow { get; set; }
        public string IpAddress { get; set; }

    }
}