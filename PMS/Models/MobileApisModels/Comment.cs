using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMS.Models.MobileApisModels
{
    public class Comment
    {
        public string TaskId { get; set; }
        public string FromassignId { get; set; }
        public string ToAssignId { get; set; }
        public int Status { get; set; }
        public string Description { get; set; }
        public string CreatedOn { get; set; }
    }
}