using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMS.Models
{
    public class ChangeTaskStatusViewModel
    {
        public string taskId { get; set; }
        public string Status { get; set; }
        public string Remark { get; set; }
    }
}