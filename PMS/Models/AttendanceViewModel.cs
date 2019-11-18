using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace PMS.Models
{
	public class AttendanceModel
    {
        public int ID { get; set; }
        public string EmpName { get; set; }
        public Nullable<System.DateTime> ComingTime { get; set; }
        public Nullable<System.DateTime> DateOfDay { get; set; }
        public Nullable<System.DateTime> LeaveTime { get; set; }
        public string comingtime { get; set; }
        public string dateofday { get; set; }
        public string leavetime { get; set; }
        public string EmployeeID { get; set; }
        public string LateTime { get; set; }
        public string WorkingHours { get; set; }
        public string AttandanceStatus { get; set; }


    }
    public class AttendanceFilter
    {
        public string Employee { get; set; }
        public string start { get; set; }
        public string end { get; set; }
     

    }
    public class Attendanceclock
    {
        public string IsPresent { get; set; }


    }

    public class AttendanceExportViewModel
    {
     
        public string Name { get; set; }
        public string ComingTime { get; set; }
        public string Date { get; set; }
        public string LeaveTime { get; set; }
        public string LateTime { get; set; }
        public string WorkingHours { get; set; }
        public string AttandanceStatus { get; set; }


    }

}