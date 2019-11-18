using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PMS.Models;
using PMS.Models.MobileApisModels;
using PMS_API.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Results;
using static PMS_API.Models.CommonModal;

namespace PMS.Controllers.MobileApis
{
    [RoutePrefix("api/Leave")]
    [Authorize]
    public class LeaveMgntController : ApiController
    {
        PMSEntities db = new PMSEntities();
        ApplicationDbContext db1 = new ApplicationDbContext();
        [HttpGet]
        [Route("LeaveType")]
        [AllowAnonymous]
        public JsonResult<object> LeaveType()
        {
            var obj = new object();
            Dictionary<string, object> dic = new Dictionary<string, object>();
            ApplicationDbContext context = new ApplicationDbContext();
            try
            {
                var ddlLeaveType = context.Database.SqlQuery<SelectListItem1>("TaskListForDDL").ToList();
                if (ddlLeaveType != null && ddlLeaveType.Count > 0)
                {
                    dic.Add("result", ddlLeaveType);
                    dic.Add("Message", "ddlLeaveType");
                    dic.Add("Status", "1");
                }
                else
                {
                    dic.Add("Message", "No Data available");
                    dic.Add("Status", "2");
                }
            }
            catch (Exception E)
            {
                dic.Add("Message", E.Message);
                dic.Add("Status", "0");
            }
            obj = dic;
            return Json(obj);
        }

        [HttpGet]
        [Route("ManagerList")]
        [AllowAnonymous]
        public JsonResult<object> ManagerList()
        {
            var obj = new object();
            Dictionary<string, object> dic = new Dictionary<string, object>();
            ApplicationDbContext context = new ApplicationDbContext();
            try
            {
                var ddlManagerList = context.Database.SqlQuery<SelectListItem1>("ManagerListForDDL").ToList();
                if (ddlManagerList != null && ddlManagerList.Count > 0)
                {
                    dic.Add("result", ddlManagerList);
                    dic.Add("Message", "ddlManagerList");
                    dic.Add("Status", "1");
                }
                else
                {
                    dic.Add("Message", "No Data available");
                    dic.Add("Status", "2");
                }
            }
            catch (Exception E)
            {
                dic.Add("Message", E.Message);
                dic.Add("Status", "0");
            }
            obj = dic;
            return Json(obj);
        }

        [HttpPost]
        [Route("ApplyLeave")]
        public JsonResult<object> ApplyLeave([FromBody]JObject json)
        {
            var obj = new object();
            Dictionary<string, object> dic = new Dictionary<string, object>();

            try
            {
                LeaveVM re = JsonConvert.DeserializeObject<LeaveVM>(json.ToString());
                Leave_Management_Leave_Taken Leave = new Leave_Management_Leave_Taken();
                string UserId = User.Identity.GetUserId();
                // string taskid = re.details.TaskId;
                Leave.fk_user_id = UserId;
                if (!string.IsNullOrWhiteSpace(re.TotalLeave))
                {
                    string[] tokens = re.TotalLeave.Split(' ', '\t');
                    Leave.TotalLeave = Convert.ToInt32(tokens[0]);
                }
                if (!string.IsNullOrWhiteSpace(re.day_type.Trim()))
                {
                    if (Convert.ToInt32(re.day_type.Trim()) == 1)//Morning
                    {
                        Leave.is_halfday = true;
                        Leave.is_afternoon = false;
                    }
                    else if (Convert.ToInt32(re.day_type.Trim()) == 2)//Afternoon
                    {
                        Leave.is_halfday = true;
                        Leave.is_afternoon = true;
                    }
                    else //full day
                    {
                        Leave.is_halfday = false;
                    }
                }
                Leave.is_halfday = false;
                Leave.fk_leave_type = Convert.ToInt32(re.fk_leave_type.Trim());
                if (Convert.ToInt32(re.fk_leave_type.Trim()) == 1)
                {
                    Leave.LeaveDateFrom = Convert.ToDateTime(Convert.ToDateTime(re.LeaveDateFrom).ToString("yyyy-MM-dd") + " " + Convert.ToDateTime(re.LeaveTimeFrom).ToString("hh:mm tt"));
                    Leave.LeaveDateTo = Convert.ToDateTime(Convert.ToDateTime(re.LeaveDateFrom).ToString("yyyy-MM-dd") + " " + Convert.ToDateTime(re.LeaveTimeTo).ToString("hh:mm tt"));
                }
                else
                {
                    Leave.LeaveDateFrom = Convert.ToDateTime(re.LeaveDateFrom);
                    Leave.LeaveDateTo = Convert.ToDateTime(re.LeaveDateTo);
                }
                Leave.leave_status = 0;
                Leave.fk_approved_user_id = re.fk_approved_user_id;
                Leave.Remark = re.Remark;
                Leave.leave_days = DateTime.Now;
                db.Leave_Management_Leave_Taken.Add(Leave);
                db.SaveChanges();
                dic.Add("Message", "Leave Applied Successfully.");
                dic.Add("Status", "1");
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
            {
                Exception raise = dbEx;
                string message;
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        message = string.Format("{0}:{1}",
                           validationErrors.Entry.Entity.ToString(),
                           validationError.ErrorMessage);
                        raise = new InvalidOperationException(message, raise);
                    }
                }
                dic.Add("Message", raise);
                dic.Add("Status", "0");
            }
            obj = dic;
            return Json(obj);
        }


        [HttpPost]
        [Route("FindLeaveById")]
        public JsonResult<object> FindLeaveById([FromBody]JObject json)
        {
            findLeave request = JsonConvert.DeserializeObject<findLeave>(json.ToString());
            var obj = new object();
            Dictionary<string, object> dic = new Dictionary<string, object>();
            try
            {
                int LeaveId = request.LeaveId;
                string userId = User.Identity.GetUserId();
                SqlParameter param1 = new SqlParameter("@LeaveId", LeaveId);
                var LeaveById = db1.Database.SqlQuery<LeaveListVM>("EmpLeaveDetailById @LeaveId", param1).ToList();
                if (LeaveById != null && LeaveById.Count > 0)
                {
                    dic.Add("result", LeaveById);
                    dic.Add("Message", "LeaveDetailById");
                    dic.Add("Status", "1");
                }
                else
                {
                    dic.Add("Message", "No Data available");
                    dic.Add("Status", "2");
                }
            }
            catch (Exception E)
            {
                dic.Add("Message", E.Message);
                dic.Add("Status", "0");
            }
            obj = dic;
            return Json(obj);
        }

        [HttpPost]
        [Route("UpdateLeave")]
        public JsonResult<object> UpdateLeave([FromBody]JObject json)
        {
            var obj = new object();
            Dictionary<string, object> dic = new Dictionary<string, object>();

            try
            {
                LeaveVM re = JsonConvert.DeserializeObject<LeaveVM>(json.ToString());
                Leave_Management_Leave_Taken Leave = new Leave_Management_Leave_Taken();
                string UserId = User.Identity.GetUserId();
                int pk_leave_taken_id = re.pk_leave_taken_id;
                var data = db.Leave_Management_Leave_Taken.Find(pk_leave_taken_id);
                if (data != null)
                {
                    data.fk_user_id = UserId;
                    if (!string.IsNullOrWhiteSpace(re.TotalLeave))
                    {
                        string[] tokens = re.TotalLeave.Split(' ', '\t');
                        data.TotalLeave = Convert.ToInt32(tokens[0]);
                    }
                    if (!string.IsNullOrWhiteSpace(re.day_type))
                    {
                        if (Convert.ToInt32(re.day_type.Trim()) == 1)//Morning
                        {
                            data.is_afternoon = false;
                        }
                        else if (Convert.ToInt32(re.day_type.Trim()) == 2)//Afternoon
                        {
                            data.is_afternoon = true;
                        }
                    }
                    if (!string.IsNullOrWhiteSpace(re.fk_leave_type)) data.fk_leave_type = Convert.ToInt32(re.fk_leave_type.Trim());
                    if (Convert.ToInt32(re.fk_leave_type.Trim()) == 1)
                    {
                        if (!string.IsNullOrWhiteSpace(Convert.ToString(re.LeaveTimeFrom)))
                        {
                            data.LeaveDateFrom = Convert.ToDateTime(Convert.ToDateTime(re.LeaveDateFrom).ToString("yyyy-MM-dd") + " " + Convert.ToDateTime(re.LeaveTimeFrom).ToString("hh:mm tt"));
                            data.LeaveDateTo = Convert.ToDateTime(Convert.ToDateTime(re.LeaveDateFrom).ToString("yyyy-MM-dd") + " " + Convert.ToDateTime(re.LeaveTimeTo).ToString("hh:mm tt"));
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrWhiteSpace(Convert.ToString(re.LeaveDateFrom)))
                        {
                            data.LeaveDateFrom = Convert.ToDateTime(re.LeaveDateFrom);
                            data.LeaveDateTo = Convert.ToDateTime(re.LeaveDateTo);
                        }
                    }
                    data.fk_approved_user_id = re.fk_approved_user_id;
                    data.Remark = re.Remark;
                    db.Leave_Management_Leave_Taken.Add(Leave);
                    db.SaveChanges();
                    dic.Add("Message", "Leave Updated Successfully.");
                    dic.Add("Status", "1");

                }
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
            {
                Exception raise = dbEx;
                string message;
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        message = string.Format("{0}:{1}",
                           validationErrors.Entry.Entity.ToString(),
                           validationError.ErrorMessage);
                        raise = new InvalidOperationException(message, raise);
                    }
                }
                dic.Add("Message", raise);
                dic.Add("Status", "0");
            }
            obj = dic;
            return Json(obj);
        }

        [HttpPost]
        [Route("LeaveList")]
        public JsonResult<object> LeaveList([FromBody]JObject json)
        {
            leaveList request = JsonConvert.DeserializeObject<leaveList>(json.ToString());
            var obj = new object();
            Dictionary<string, object> dic = new Dictionary<string, object>();
            try
            {
                int FromRow = request.pageNumber;
                string userId = User.Identity.GetUserId();
                int leave_status = Convert.ToInt32(request.leave_status);
                if (request.FromDate == "" && request.Todate == "")
                {
                    SqlParameter param1 = new SqlParameter("@FromRow", FromRow);
                    SqlParameter param2 = new SqlParameter("@userId", userId);
                    SqlParameter param3 = new SqlParameter("@leave_status", leave_status);
                    var LeaveList = db1.Database.SqlQuery<LeaveListVM>("EmpLeaveList @FromRow, @userId,@leave_status", param1, param2, param3).ToList();
                    if (LeaveList != null && LeaveList.Count > 0)
                    {
                        dic.Add("result", LeaveList);
                        dic.Add("Message", "LeaveList");
                        dic.Add("Status", "1");
                    }
                    else
                    {
                        dic.Add("Message", "No Data available");
                        dic.Add("Status", "2");
                    }
                }
                else
                {
                    SqlParameter param1 = new SqlParameter("@FromRow", FromRow);
                    SqlParameter param2 = new SqlParameter("@userId", userId);
                    SqlParameter param3 = new SqlParameter("@leave_status", leave_status);
                    SqlParameter param4 = new SqlParameter("@Fromdate", request.FromDate);
                    SqlParameter param5 = new SqlParameter("@Todate", request.Todate);
                    var LeaveList = db1.Database.SqlQuery<LeaveListVM>("EmpLeaveList_ByTwoDate @FromRow, @userId,@leave_status,@Fromdate,@Todate", param1, param2, param3, param4,param5).ToList();
                    if (LeaveList != null && LeaveList.Count > 0)
                    {
                        dic.Add("result", LeaveList);
                        dic.Add("Message", "LeaveList");
                        dic.Add("Status", "1");
                    }
                    else
                    {
                        dic.Add("Message", "No Data available");
                        dic.Add("Status", "2");
                    }

                }

            }
            catch (Exception E)
            {
                dic.Add("Message", E.Message);
                dic.Add("Status", "0");
            }
            obj = dic;
            return Json(obj);
        }

        [HttpPost]
        [Route("DeleteLeave")]
        public JsonResult<object> DeleteLeave([FromBody]JObject json)
        {
            leaveList request = JsonConvert.DeserializeObject<leaveList>(json.ToString());
            var obj = new object();
            Dictionary<string, object> dic = new Dictionary<string, object>();
            if (!string.IsNullOrWhiteSpace(request.Id))
            {
                int id = Convert.ToInt32(request.Id);
                var def = (from l in db.Leave_Management_Leave_Taken where l.pk_leave_taken_id == id select l).FirstOrDefault();
                db.Leave_Management_Leave_Taken.Remove(def);
                db.SaveChanges();
                dic.Add("Message", "Data Deleted Successfully.");
                dic.Add("Status", "1");

            }
            else
            {
                dic.Add("Message", "Provide LeaveId.");
                dic.Add("Status", "1");
            }
            obj = dic;
            return Json(obj);
        }

        [HttpPost]
        [Route("LeaveListWeb")]
        public JsonResult<object> LeaveListWeb([FromBody]JObject json)
        {
            leaveList request = JsonConvert.DeserializeObject<leaveList>(json.ToString());
            var obj = new object();
            Dictionary<string, object> dic = new Dictionary<string, object>();
            try
            {
                string RoleType = null;
                string userId = User.Identity.GetUserId();

                if (string.IsNullOrEmpty(request.RoleType))
                {
                    if (User.IsInRole("Admin") || User.IsInRole("Manager")) RoleType = "AdminUser";
                    if (User.IsInRole("User")) RoleType = "User";
                }
                else
                {
                    RoleType = request.RoleType;
                }
                int leave_type = Convert.ToInt32(request.leave_status);
                SqlParameter param2 = new SqlParameter("@userId", userId);
                SqlParameter param3 = new SqlParameter("@leave_type", leave_type);
                SqlParameter param4 = new SqlParameter("@RoleType", RoleType);
                var LeaveList = db1.Database.SqlQuery<LeaveListVM>("EmpLeaveListWeb @userId,@leave_type,@RoleType", param2, param3, param4).ToList();
                if (LeaveList != null && LeaveList.Count > 0)
                {
                    dic.Add("result", LeaveList);
                    dic.Add("Message", "LeaveList");
                    dic.Add("Status", "1");
                }
                else
                {
                    dic.Add("Message", "No Data available");
                    dic.Add("Status", "2");
                }
            }
            catch (Exception E)
            {
                dic.Add("Message", E.Message);
                dic.Add("Status", "0");
            }
            obj = dic;
            return Json(obj);
        }

        [HttpPost]
        [Route("FindLeaveByIdWeb")]
        public JsonResult<object> FindLeaveByIdWeb([FromBody]JObject json)
        {
            findLeave request = JsonConvert.DeserializeObject<findLeave>(json.ToString());
            var obj = new object();
            Dictionary<string, object> dic = new Dictionary<string, object>();
            try
            {
                int LeaveId = request.LeaveId;
                //string userId = User.Identity.GetUserId();
                if (LeaveId > 0)
                {
                    SqlParameter param1 = new SqlParameter("@LeaveId", LeaveId);
                    var LeaveById = db1.Database.SqlQuery<LeaveListVM>("EmpLeaveDetailByIdWeb @LeaveId", param1).ToList();
                    if (LeaveById != null && LeaveById.Count > 0)
                    {
                        dic.Add("result", LeaveById);
                        dic.Add("Message", "LeaveDetailById");
                        dic.Add("Status", "1");
                    }
                    else
                    {
                        dic.Add("Message", "No Data available");
                        dic.Add("Status", "2");
                    }
                }
                else
                {
                    dic.Add("Message", "Please Provide LeaveId");
                    dic.Add("Status", "3");
                }
            }
            catch (Exception E)
            {
                dic.Add("Message", E.Message);
                dic.Add("Status", "0");
            }
            obj = dic;
            return Json(obj);
        }

        [HttpPost]
        [Route("LeaveApproveReject")]
        public JsonResult<object> LeaveApproveReject([FromBody]JObject json)
        {
            LaveDetlVm request = JsonConvert.DeserializeObject<LaveDetlVm>(json.ToString());
            var obj = new object();
            Dictionary<string, object> dic = new Dictionary<string, object>();
            try
            {
                int LeaveId = request.pk_leave_taken_id;
                int leave_status = request.leave_status;
                //string userId = User.Identity.GetUserId();
                if (LeaveId > 0)
                { 
                    SqlParameter param1 = new SqlParameter("@LeaveId", LeaveId);
                    SqlParameter param2 = new SqlParameter("@leave_status", leave_status);
                    db1.Database.SqlQuery<LeaveListVM>("EmpLeaveApproveReject @LeaveId,@leave_status", param1,param2).ToList();
                    if (leave_status == 1)
                    {
                        dic.Add("Message", "Leave Approved Successfully.");
                        dic.Add("Status", "1");
                    }
                    else
                    {
                        dic.Add("Message", "Leave Rejected Successfully.");
                        dic.Add("Status", "1");
                    }
                }
                else
                {
                    dic.Add("Message", "Please Provide LeaveId");
                    dic.Add("Status", "2");
                }
            }
            catch (Exception E)
            {
                dic.Add("Message", E.Message);
                dic.Add("Status", "0");
            }
            obj = dic;
            return Json(obj);
        }

    }
}