using PMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using System.Data.Entity;

namespace PMS.Controllers.webapi
{
    public class ApproveLeaveController : ApiController
    {
       
        // GET api/<controller>
        public List<ApproveLeave> Get()
        {
            List<ApproveLeave> L = new List<ApproveLeave>();
            using (PMSEntities DC = new PMSEntities())
            {
                var Userid = User.Identity.GetUserId();
                var def = (from l in DC.Leave_Management_Leave_Taken select l);
                var  userid = Userid;
                List<string> requestors = (from r in DC.Leave_Management_Approval_Matrix where r.fk_requestor_id == userid select r.fk_requestor_id.ToString()).ToList<string>();
                if (requestors.Count>0)
                {
                    def = (from l in DC.Leave_Management_Leave_Taken
                           where l.leave_days.Value.Year >= DateTime.Now.Year
                           && l.leave_days.Value.Month >= DateTime.Now.Month - 3
                           select l);
                }

                foreach (var i in def)
                {
                    ApproveLeave l = new ApproveLeave();
                    l.ID = i.pk_leave_taken_id;
                    l.D = i.leave_days.Value.ToString("yyyy-MM-dd");
                    l.T = i.Leave_Management_Leave_Type.leave_type;
                    l.U = i.Innovation_Management_User.user_firstname;

                    switch (i.leave_status)
                    {
                        case 0:
                            l.S = "Applied";
                            break;
                        case 1:
                            l.S = "Approved (" + ((i.Innovation_Management_User1 != null) ? i.Innovation_Management_User1.user_firstname : "-") + ")";
                            break;
                        case 2:
                            l.S = "Rejected";
                            break;
                    }
                    if ((bool)i.is_halfday)
                    {
                        if ((bool)i.is_afternoon)
                        {
                            l.D += " afternoon";
                        }
                        else
                        {
                            l.D += " morning";
                        }
                    }

                    L.Add(l);
                }
            }
            return L;
        }

        public void Post(int id, int status)
        {
            using (PMSEntities DC = new PMSEntities())
            {
                var Userid = User.Identity.GetUserId();
                var def = (from l in DC.Leave_Management_Leave_Taken where l.pk_leave_taken_id == id select l).FirstOrDefault();
                def.leave_status = status;
                if (status == 0)
                {
                    def.fk_approved_user_id = null;
                }
                else
                {
                    def.fk_approved_user_id = Userid;
                }
                DC.Entry(def).State = EntityState.Modified;
                DC.SaveChanges();
            

                #region email
                if (status == 1)
                {
                    string emailadd = def.Innovation_Management_User.user_email;
                    string date = def.leave_days.Value.ToShortDateString();
                    System.Threading.Thread email = new System.Threading.Thread(delegate()
                    {
                        CommonMethods.SendMail(emailadd, "info@globibo.com", "Leave Approved - " + date, "", false);
                    });
                    email.IsBackground = true;
                    email.Start();
                }
                #endregion
            }
        }

        private static void returnToAjax(string Message)
        {
            var resp = new HttpResponseMessage(HttpStatusCode.InternalServerError)
            {
                ReasonPhrase = Message
            };
            throw new HttpResponseException(resp);
        }
    }

    public class ApproveLeave
    {
        public int ID { get; set; }
        public string U { get; set; }//applied user
        public string T { get; set; }//leave type
        public string D { get; set; }//leave date
        public string S { get; set; }//leave status
    }
}