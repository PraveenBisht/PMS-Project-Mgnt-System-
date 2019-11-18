using PMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace PMS.Controllers.webapi
{    
    public class LeaveController : ApiController
    {
        // GET api/<controller>
        public List<string[]> Get()
        {
            List<string[]> dat = new List<string[]>();

            PMSEntities DC = new PMSEntities();
            var data = (from l in DC.Leave_Management_Leave_Taken where l.leave_status == 1 && l.leave_days.Value.Year == DateTime.Now.Year orderby l.leave_days ascending select new { l.leave_days, l.Innovation_Management_User.user_firstname }).ToList();
            var days = (from d in data select d.leave_days).Distinct();
            foreach (var item in days)
            {
                var users = (from u in data where u.leave_days == item select u.user_firstname);
                string users2 = string.Join(", ", users);

                string color = "#ff340c";//red
                switch (users.Count())
                {
                    case 1:
                        color = "#c5cf4c";
                        break;
                    case 2:
                        color = "#f1dd30";
                        break;
                    case 3:
                        color = "#ff900c";
                        break;
                }

                string[] test = new string[] { item.Value.ToString("d/M/yyyy"), users2, "", color };
                dat.Add(test);
            }
            return dat;
        }

    }
}