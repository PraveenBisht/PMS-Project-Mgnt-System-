//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PMS
{
    using System;
    using System.Collections.Generic;
    
    public partial class Attendance
    {
        public int ID { get; set; }
        public Nullable<System.DateTime> ComingTime { get; set; }
        public Nullable<System.DateTime> DateOfDay { get; set; }
        public Nullable<System.DateTime> LeaveTime { get; set; }
        public string EmployeeID { get; set; }
    }
}