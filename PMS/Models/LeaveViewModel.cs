using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMS.Models
{
    public class LeaveViewModel
    {
        public int Leave_Taken_Id { get; set; }
        public string Leave_days { get; set; }
        public string ApproverName { get; set; }
        public string UserName { get; set; }
        public string Leavestatus { get; set; }


    }
}