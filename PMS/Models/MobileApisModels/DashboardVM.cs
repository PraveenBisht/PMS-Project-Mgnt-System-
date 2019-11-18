using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMS.Models.MobileApisModels
{
    public class UserDashboardVM
    {
        public string TotalAttendance { get; set; } = "0";
        public string TotalAbsent { get; set; } = "0";
        public string ClockInTime { get; set; } = "0";
        public string MonthRemainingDays { get; set; } = "0";
        public string TotalTask { get; set; } = "0";
        public string OpenTask { get; set; } = "0";
        public string ReOpenTask { get; set; } = "0";
        public string CompletedTask { get; set; } = "0";
        public string ApprovedLeave { get; set; } = "0";
        public string RejectLeave { get; set; } = "0";
        public string PendingLeave { get; set; } = "0";
   
    }
}