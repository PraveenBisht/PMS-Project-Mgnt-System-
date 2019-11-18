using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMS.Models
{
    public class AssignViewModel
    {
        public int assignId { get; set; }
        public string taskId { get; set; }
        public string toassignId { get; set; }
        public string fromAssignId { get; set; }
        public string assignDateTime { get; set; }
        public string Userid { get; set; }

    }
}