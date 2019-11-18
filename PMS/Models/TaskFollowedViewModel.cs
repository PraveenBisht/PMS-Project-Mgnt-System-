using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMS.Models
{
    public class TaskFollowedViewModel
    {
        public int FollwID { get; set; }
        public string Statusid { get; set; }
        public string Remark { get; set; }
        public string Createdby { get; set; }
        public string CreatedOn { get; set; }
        public string TaskId { get; set; }
    }
}