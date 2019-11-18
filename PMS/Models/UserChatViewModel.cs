using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMS.Models
{
    public class UserChatViewModel
    {
        public string userId { get; set; }
        public string EmailID { get; set; }
        public string Name { get; set; }
        public string LastMassage { get; set; }
        public string Picture { get; set; }
        public string LastSeen { get; set; }
        public string OnlineStatus { get; set; }
        public string designation { get; set; }
        public int InComingmessagecount { get; set; }

        public DateTime ChatDateTime { get; set; }

    }
}