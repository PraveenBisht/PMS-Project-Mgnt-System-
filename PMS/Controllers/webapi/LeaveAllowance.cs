using PMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data.Entity;

namespace PMS.Controllers.webapi
{
    public class LeaveAllowanceController : ApiController
    {
        // GET api/<controller>
        public IEnumerable<object> Get()
        {
            PMSEntities DbLMS = new PMSEntities();
            var LeaveType = DbLMS.Leave_Management_Leave_Allowance
                .Select(x => new { x.pk_leave_allowance_id, x.Innovation_Management_User.user_firstname, 
                    x.allowance_year, x.Leave_Management_Leave_Type.leave_type, 
                    x.allowance_entitlement, x.allowance_taken })
                    .Where(x => x.pk_leave_allowance_id != null);
            
            return LeaveType;
        }//user_firstname user_lastname user_picture user_email user_isadmin user_isdeleted pk_user_id

        // GET api/<controller>/5
        public RemainingLeaveInfo Get(int id)
        {
            PMSEntities DC = new PMSEntities();
            var leave_request = DC.Leave_Management_Leave_Taken.Where(x => x.pk_leave_taken_id == id).First();
            RemainingLeaveInfo t = new RemainingLeaveInfo();
            try
            {
                var all_leaves = DC.Leave_Management_Leave_Allowance.Where(x => x.fk_user_id == leave_request.fk_user_id
                    && x.allowance_year == leave_request.leave_days.Value.Year.ToString());

                int all_leaves_count = all_leaves.Select(x => (int)x.allowance_entitlement).Sum();
                int specific_leave_count = all_leaves.Where(x => x.fk_leave_type_id == leave_request.fk_leave_type).Select(x => (int)x.allowance_entitlement).Sum();

                int all_taken_leave = DC.Leave_Management_Leave_Taken.Where(x => x.fk_user_id == leave_request.fk_user_id
                    && x.leave_status == 1).Count();
                int specific_taken_leave = DC.Leave_Management_Leave_Taken.Where(x => x.fk_user_id == leave_request.fk_user_id
                    && x.fk_leave_type == leave_request.fk_leave_type
                    && x.leave_status == 1).Count();

                t.AL = all_leaves_count; t.AT = all_taken_leave;
                t.SL = specific_leave_count; t.ST = specific_taken_leave;

            }
            catch (Exception)
            {
                returnToAjax("Failed to get remaining leave allowance. Please check that " + leave_request.Leave_Management_Leave_Type.leave_type + " leaves are allocated for "+ leave_request.leave_days.Value.Year);
            }
            return t;
        }

        public class RemainingLeaveInfo
        {
            public int AL { get; set; }//all leaves
            public int SL { get; set; }//specific leaves
            public int AT { get; set; }//all taken
            public int ST { get; set; }//specific taken
        }

        private static void returnToAjax(string Message)
        {
            var resp = new HttpResponseMessage(HttpStatusCode.InternalServerError)
            {
                ReasonPhrase = Message
            };
            throw new HttpResponseException(resp);
        }

        // POST api/<controller>
        public bool Post(Leave_Management_Leave_Allowance NewMember)
        {
            try
            {
                PMSEntities DbLMS = new PMSEntities();

                DbLMS.Leave_Management_Leave_Allowance.Add(NewMember);
                DbLMS.SaveChanges();

                return true;
            }
            catch (Exception)
            {
                return true;
            }

        }

        // PUT api/<controller>/5
        public bool Post(int id, Leave_Management_Leave_Allowance newType)
        {
            try
            {
                PMSEntities DbLMS = new PMSEntities();
                var SelectedRow = DbLMS.Leave_Management_Leave_Allowance.FirstOrDefault(x => x.pk_leave_allowance_id == id);
                SelectedRow.allowance_entitlement = newType.allowance_entitlement;
                DbLMS.Entry(SelectedRow).State = EntityState.Modified;
                DbLMS.SaveChanges();

                return true;
            }
            catch (Exception)
            {
                return true;
            }
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }
}