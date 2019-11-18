using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace PMS_API.Models
{
    [DataContract]
    public class AttendanceManagement
    {
        [DataMember(Name = "RemainingDays")]
        public string RemainingDays { get; set; } = "";
        [DataMember(Name = "TotalPresent")]
        public string TotalPresent { get; set; } = "";
        [DataMember(Name = "TotalAbsent")]
        public string TotalAbsent { get; set; } = "";
        [DataMember(Name = "TodaysLogInTime")]
        public string TodaysLogInTime { get; set; } = "";
    }

    [DataContract]
    public class TaskManagement
    {
        [DataMember(Name = "TotalTasks")]
        public string TotalTasks { get; set; } = "";
        [DataMember(Name = "OpenTask")]
        public string OpenTask { get; set; } = "";
        [DataMember(Name = "ReOpenTask")]
        public string ReOpenTask { get; set; } = "";
        [DataMember(Name = "CompletedTask")]
        public string CompletedTask { get; set; } = "";
    }
    [DataContract]
    public class LeaveManagement
    {
        [DataMember(Name = "ApprovedLeave")]
        public string ApprovedLeave { get; set; } = "";
        [DataMember(Name = "PendingLeave")]
        public string PendingLeave { get; set; } = "";
        [DataMember(Name = "RejectLeave")]
        public string RejectLeave { get; set; } = "";
    }
}