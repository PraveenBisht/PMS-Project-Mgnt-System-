using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMS.Models
{
    public class NotifModels
    {
        public string UserID { get; set; }
        public string Message { get; set; }
    }
    public class NotificationModels
    {
        public string Message { get; set; }
        public string FromEmailID { get; set; }
        public string Name { get; set; }
        public string ProfilePic { get; set; }
        public string UserEmailID { get; set; }
        public string designation { get; set; }
    }
}