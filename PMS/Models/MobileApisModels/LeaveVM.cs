using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMS.Models.MobileApisModels
{
    public class LeaveVM
    {
        public int pk_leave_taken_id { get; set; }
        public string fk_user_id { get; set; }
        public string fk_leave_type { get; set; }
        public DateTime? leave_days { get; set; }
        public string day_type { get; set; }
        public string is_halfday { get; set; }
        public string is_afternoon { get; set; }
        public string leave_status { get; set; }
        public string fk_approved_user_id { get; set; }
        public DateTime? LeaveDateFrom { get; set; }
        public DateTime? LeaveDateTo { get; set; }
        public DateTime? LeaveTimeFrom { get; set; }
        public DateTime? LeaveTimeTo { get; set; }
        public string TotalLeave { get; set; }
        public string Remark { get; set; }
    }
    public class LeaveListVM
    {
        public int pk_leave_taken_id { get; set; }
        public string fk_user_id { get; set; }
        public int fk_leave_type { get; set; }
        public string leave_type { get; set; }
        public string is_halfday { get; set; }
        public string LeaveDateFrom { get; set; }
        public string LeaveDateTo { get; set; }
        public string LeaveTimeFrom { get; set; }
        public string LeaveTimeTo { get; set; }
        public int TotalLeave { get; set; }
        public int leave_status { get; set; }
        public string leave_status1 { get; set; }
        public string Name { get; set; }
        public string UserName { get; set; }
        public string ManagerName { get; set; }
        public string Remark { get; set; }
    }
    public class findLeave
    {
        public int LeaveId { get; set; }
    }
    public class LaveDetlVm
    {
        public int pk_leave_taken_id { get; set; }
        public int leave_status { get; set; }
    }
}
