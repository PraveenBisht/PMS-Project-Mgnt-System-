using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
//using Microsoft.AspNetCore.Cors;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PMS;
using PMS.Models;
using PMS.Models.MobileApisModels;
using PMS_API.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Results;

namespace PMS_API.Controllers
{
    [Authorize]
    [RoutePrefix("api/MobileDashboard")]
    public class MobileDashboardController : ApiController
    {
        PMSEntities db = new PMSEntities();
        public string RoleName { get; set; }

        [HttpPost]
        [Route("Dashboardata")]
        public JsonResult<object> Dashboardata([FromBody]JObject json)
        {
            Attendanceclock attandanceClock = JsonConvert.DeserializeObject<Attendanceclock>(json.ToString());
            var obj = new object();
            Dictionary<string, object> dic = new Dictionary<string, object>();

            UserDashboardVM userDash = new UserDashboardVM();
            AttendanceManagement Attendance_Management = new AttendanceManagement();
            TaskManagement Task_Management = new TaskManagement();
            LeaveManagement Leave_Management = new LeaveManagement();

            try
            {
                if (User.Identity.IsAuthenticated)
                {
                    int opentask = 0, reopentask = 0, completedtask = 0, pendingleave = 0, ApproveLeave = 0, Rejectedleave = 0;
                    double Employeependingleave = 0, EmployeeApproveLeave = 0, EmployeeRejectedleave = 0;
                    var userid = User.Identity.GetUserId();
                    var user = User.Identity;
                    ApplicationDbContext context = new ApplicationDbContext();
                    var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

                    var s = UserManager.GetRoles(userid);
                    RoleName = s[0].ToString();
                    DateTime todayDate = DateTime.Now.Date;
                    bool AttendanceValid = db.Attendances.Any(c => c.DateOfDay == todayDate && c.EmployeeID == userid && c.ComingTime!=null);
                    var UserDetails = db.AspNetUsers.Where(x => x.Id == userid).FirstOrDefault();
                    string username = (UserDetails.Name == null || UserDetails.Name == "NA") ? "NA" : UserDetails.Name;
                    string deginations = (UserDetails.Designation == null || UserDetails.Designation == "NA") ? "NA" : UserDetails.Designation; 
            
                    if (attandanceClock.IsPresent == "p")
                    {
                        DateTime myDateTime = DateTime.Now;
                        DateTime CheckTime = DateTime.Now.Date.AddHours(12);
                        if (AttendanceValid)
                        {
                            var attendanceRow = db.Attendances.Where(c => c.DateOfDay == todayDate && c.EmployeeID == userid).Single();
                            dic.Add("iscoming", "true");
                            dic.Add("isLeave", "true");
                            attendanceRow.LeaveTime = myDateTime;
                            db.Entry(attendanceRow).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                        else
                        {
                            DateTime now = DateTime.Now;
                            Attendance Attendance = new Attendance
                            {
                                EmployeeID = userid,
                                ComingTime = myDateTime,
                                // DateOfDay = Convert.ToDateTime(DateTime.Now.ToString("dd-MM-yyyy")),
                                DateOfDay = DateTime.Now.Date,

                            };

                            db.Attendances.Add(Attendance);
                            db.SaveChanges();
                            dic.Add("isLeave", "false");
                            dic.Add("iscoming", "true");
                            if (myDateTime > CheckTime)
                            {
                                var leavedetails = db.Leave_Management_Leave_Taken.Where(x => x.fk_user_id == userid && x.leave_days == todayDate).FirstOrDefault();
                                if (leavedetails == null)
                                {
                                    var Rolename = db.AspNetRoles.Where(x => x.Name == "Admin").FirstOrDefault();
                                    var AdminID = db.UserRoles.Where(x => x.RoleId == Rolename.Id).FirstOrDefault();
                                    DateTime d = todayDate;
                                    Leave_Management_Leave_Taken lt = new Leave_Management_Leave_Taken();
                                    lt.fk_leave_type = 2;
                                    lt.leave_days = d;
                                    lt.fk_user_id = userid;
                                    lt.leave_status = 1;
                                    lt.is_halfday = true;
                                    lt.TotalLeave = 1;
                                    lt.Remark = "Atomatic Leave";
                                    lt.LeaveDateFrom = todayDate;
                                    lt.LeaveDateTo = todayDate.AddDays(1);
                                    lt.fk_approved_user_id = AdminID.UserID;
                                    db.Leave_Management_Leave_Taken.Add(lt);
                                    db.SaveChanges();
                                }
                            }
                        }
                    }
                    else
                    {
                        if (AttendanceValid == true)
                        {

                            dic.Add("iscoming", "true");

                            bool AttendanceAllValid = db.Attendances.Any(c => c.DateOfDay == todayDate && c.EmployeeID == userid && c.LeaveTime != null );
                            
                         
                            if (AttendanceAllValid)
                            {
                                dic.Add("isLeave", "true");
                            }
                            else
                            {
                                dic.Add("isLeave", "false");
                            }
                        }
                        else
                        {
                            DateTime CheckTime = DateTime.Now.Date.AddHours(14);
                            if (DateTime.Now > CheckTime)
                            {
                                dic.Add("iscoming", "true");
                                dic.Add("isLeave", "true");
                            }
                            else
                           {
                               dic.Add("iscoming", "false");
                               dic.Add("isLeave", "false");
                           }

                        }

                    }
                    if (RoleName == "Admin")
                    {
                        var TotalTask = (from a in db.AssignTasks join t in db.tbl_Task on a.TaskId equals t.TaskId select t).ToList();
                        Task_Management.TotalTasks = Convert.ToString(TotalTask.Count);
                        if (TotalTask.Count > 0)
                        {
                            for (int i = 0; i < TotalTask.Count; i++)
                            {
                                if (TotalTask[i].Status == 1)
                                {
                                    opentask++;
                                }
                                else if (TotalTask[i].Status == 2)
                                {
                                    reopentask++;
                                }
                                else
                                {
                                    completedtask++;
                                }
                            }
                            dic.Add("TotalTask", Convert.ToString(TotalTask.Count));
                            dic.Add("OpenTask", Convert.ToString(opentask));
                            dic.Add("ReOpenTask", Convert.ToString(reopentask));
                            dic.Add("CompletedTask", Convert.ToString(completedtask));
                        }
                    }

                    if (RoleName == "User")
                    {

                        var TotalTask = (from a in  db.tbl_Task  where a.CreatedTo == userid select a).ToList();
                        Task_Management.TotalTasks = Convert.ToString(TotalTask.Count);
                        if (TotalTask.Count > 0)
                        {
                            for (int i = 0; i < TotalTask.Count; i++)
                            {
                                if (TotalTask[i].Status == 1)
                                {
                                    opentask++;
                                }
                                else if (TotalTask[i].Status == 2)
                                {
                                    reopentask++;
                                }
                                else
                                {
                                    completedtask++;
                                }
                            }
                        }
                            DateTime dt = DateTime.Now;
                            string totalHrs1="";
                            int Month = DateTime.Now.Month;
                            var TotalAttendances = db.Attendances.Where(x => x.EmployeeID == userid && (x.ComingTime != null)).ToList().Where(x => x.DateOfDay.Value.Month == Month).OrderBy(x => x.DateOfDay).ToList();
                            var AttandanceToday = db.Attendances.Where(x => x.EmployeeID == userid && (x.DateOfDay == todayDate)).FirstOrDefault();
                            if (AttandanceToday!=null)
                            {
                                totalHrs1 = AttandanceToday.ComingTime == null ? "" : Convert.ToString(DateTime.Now - AttandanceToday.ComingTime).Substring(0, 5);
                            }
                            var TotalAbsent = db.Attendances.Where(x => x.EmployeeID == userid && (x.ComingTime == null)).ToList().Where(x => x.DateOfDay.Value.Month == Month).OrderBy(x => x.DateOfDay).ToList();
                            int days = dt.AddDays(1 - dt.Day).AddMonths(1).AddDays(-1).Day - dt.Day;
                       

                            userDash.TotalAttendance = Convert.ToString(TotalAttendances.Count);
                            userDash.TotalAbsent = Convert.ToString(TotalAbsent.Count);
                            userDash.ClockInTime = totalHrs1;
                            userDash.MonthRemainingDays = Convert.ToString(days);
                            userDash.TotalTask = Convert.ToString(TotalTask.Count);
                            userDash.OpenTask = Convert.ToString(opentask);
                            userDash.ReOpenTask = Convert.ToString(reopentask);
                            userDash.CompletedTask = Convert.ToString(completedtask);
                      
                       
                        var leavedetails = (from l in db.Leave_Management_Leave_Taken
                                            where l.fk_user_id == userid
                                            select new
                                            {
                                                l.fk_approved_user_id,
                                                l.fk_leave_type,
                                                l.fk_user_id,
                                                l.is_afternoon,
                                                l.is_halfday,
                                                l.leave_status,
                                                l.TotalLeave

                                            }).ToList();

                        if (leavedetails.Count > 0)
                        {
                            for (int i = 0; i < leavedetails.Count; i++)
                            {

                                if (leavedetails[i].fk_leave_type == 2)//Morning
                                {

                                    if (leavedetails[i].leave_status == 1)
                                    {
                                        EmployeeApproveLeave = EmployeeApproveLeave + 0.5;
                                    }
                                    else if (leavedetails[i].leave_status == 2)
                                    {
                                        EmployeeRejectedleave = EmployeeRejectedleave + 0.5;
                                    }
                                    else
                                    {
                                        Employeependingleave = Employeependingleave + 0.5;
                                    }
                                }
                                else if (leavedetails[i].fk_leave_type >2)//full day
                                {
                                    if (leavedetails[i].leave_status == 1)
                                    {
                                        EmployeeApproveLeave = EmployeeApproveLeave + Convert.ToDouble(leavedetails[i].TotalLeave == null ? 1 : leavedetails[i].TotalLeave);
                                    }
                                    else if (leavedetails[i].leave_status == 2)
                                    {
                                        EmployeeRejectedleave = EmployeeRejectedleave + Convert.ToDouble(leavedetails[i].TotalLeave == null ? 1 : leavedetails[i].TotalLeave);
                                    }
                                    else
                                    {
                                        Employeependingleave = Employeependingleave + Convert.ToDouble(leavedetails[i].TotalLeave == null ? 1 : leavedetails[i].TotalLeave);
                                    }
                                }

                            }

                            userDash.ApprovedLeave = Convert.ToString(EmployeeApproveLeave);
                            userDash.RejectLeave = Convert.ToString(EmployeeRejectedleave);
                            userDash.PendingLeave = Convert.ToString(Employeependingleave);
                        }
                        dic.Add("Status", "1");
                        dic.Add("Message", "User Deshboard Data.");
                        dic.Add("LogedInDetail", userDash);
                        dic.Add("UserName", username);
                        dic.Add("designation", deginations);

                    }
                    if (RoleName == "Manager")
                    {

                        var TotalTask = (from a in db.tbl_Task where a.CreatedTo == userid select a).ToList();
                        Task_Management.TotalTasks = Convert.ToString(TotalTask.Count);
                        if (TotalTask.Count > 0)
                        {
                            for (int i = 0; i < TotalTask.Count; i++)
                            {
                                if (TotalTask[i].Status == 1)
                                {
                                    opentask++;
                                }
                                else if (TotalTask[i].Status == 2)
                                {
                                    reopentask++;
                                }
                                else
                                {
                                    completedtask++;
                                }
                            }
                        }
                        DateTime dt = DateTime.Now;
                        string totalHrs1 = "";
                        int Month = DateTime.Now.Month;
                        var TotalAttendances = db.Attendances.Where(x => x.EmployeeID == userid && (x.ComingTime != null)).ToList().Where(x => x.DateOfDay.Value.Month == Month).OrderBy(x => x.DateOfDay).ToList();
                        var AttandanceToday = db.Attendances.Where(x => x.EmployeeID == userid && (x.DateOfDay == todayDate)).FirstOrDefault();
                        if (AttandanceToday != null)
                        {
                            totalHrs1 = AttandanceToday.ComingTime == null ? "" : Convert.ToString(DateTime.Now - AttandanceToday.ComingTime).Substring(0, 5);
                        }
                        var TotalAbsent = db.Attendances.Where(x => x.EmployeeID == userid && (x.ComingTime == null)).ToList().Where(x => x.DateOfDay.Value.Month == Month).OrderBy(x => x.DateOfDay).ToList();
                        int days = dt.AddDays(1 - dt.Day).AddMonths(1).AddDays(-1).Day - dt.Day;


                        userDash.TotalAttendance = Convert.ToString(TotalAttendances.Count);
                        userDash.TotalAbsent = Convert.ToString(TotalAbsent.Count);
                        userDash.ClockInTime = totalHrs1;
                        userDash.MonthRemainingDays = Convert.ToString(days);
                        userDash.TotalTask = Convert.ToString(TotalTask.Count);
                        userDash.OpenTask = Convert.ToString(opentask);
                        userDash.ReOpenTask = Convert.ToString(reopentask);
                        userDash.CompletedTask = Convert.ToString(completedtask);


                        var leavedetails = (from l in db.Leave_Management_Leave_Taken
                                            where l.fk_user_id == userid
                                            select new
                                            {
                                                l.fk_approved_user_id,
                                                l.fk_leave_type,
                                                l.fk_user_id,
                                                l.is_afternoon,
                                                l.is_halfday,
                                                l.leave_status,
                                                l.TotalLeave

                                            }).ToList();

                        if (leavedetails.Count > 0)
                        {
                            for (int i = 0; i < leavedetails.Count; i++)
                            {

                                if (leavedetails[i].fk_leave_type == 2)//Morning
                                {

                                    if (leavedetails[i].leave_status == 1)
                                    {
                                        EmployeeApproveLeave = EmployeeApproveLeave + 0.5;
                                    }
                                    else if (leavedetails[i].leave_status == 2)
                                    {
                                        EmployeeRejectedleave = EmployeeRejectedleave + 0.5;
                                    }
                                    else
                                    {
                                        Employeependingleave = Employeependingleave + 0.5;
                                    }
                                }
                                else if (leavedetails[i].fk_leave_type > 2)//full day
                                {
                                    if (leavedetails[i].leave_status == 1)
                                    {
                                        EmployeeApproveLeave = EmployeeApproveLeave + Convert.ToDouble(leavedetails[i].TotalLeave == null ? 1 : leavedetails[i].TotalLeave);
                                    }
                                    else if (leavedetails[i].leave_status == 2)
                                    {
                                        EmployeeRejectedleave = EmployeeRejectedleave + Convert.ToDouble(leavedetails[i].TotalLeave == null ? 1 : leavedetails[i].TotalLeave);
                                    }
                                    else
                                    {
                                        Employeependingleave = Employeependingleave + Convert.ToDouble(leavedetails[i].TotalLeave == null ? 1 : leavedetails[i].TotalLeave);
                                    }
                                }

                            }

                            userDash.ApprovedLeave = Convert.ToString(EmployeeApproveLeave);
                            userDash.RejectLeave = Convert.ToString(EmployeeRejectedleave);
                            userDash.PendingLeave = Convert.ToString(Employeependingleave);
                        }
                        dic.Add("Status", "1");
                        dic.Add("Message", "User Deshboard Data.");
                        dic.Add("LogedInDetail", userDash);
                        dic.Add("UserName", username);
                        dic.Add("designation", deginations);


                        //var TotalTask = (from a in db.AssignTasks join t in db.tbl_Task on a.TaskId equals t.TaskId select t).ToList();
                        //Task_Management.TotalTasks = Convert.ToString(TotalTask.Count);
                        //if (TotalTask.Count > 0)
                        //{
                        //    for (int i = 0; i < TotalTask.Count; i++)
                        //    {
                        //        if (TotalTask[i].Status == 1)
                        //        {
                        //            opentask++;
                        //        }
                        //        else if (TotalTask[i].Status == 2)
                        //        {
                        //            reopentask++;
                        //        }
                        //        else
                        //        {
                        //            completedtask++;

                        //        }
                        //    }
                        //    dic.Add("TotalTask", Convert.ToString(TotalTask.Count));
                        //    dic.Add("OpenTask", Convert.ToString(opentask));
                        //    dic.Add("ReOpenTask", Convert.ToString(reopentask));
                        //    dic.Add("CompletedTask", Convert.ToString(completedtask));
                        //    dic.Add("error", "1");
                        //}



                    }
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
                dic.Add("Status", "0");

            }
            obj = dic;
            return Json(obj);
        }

        [HttpPost]
        [Route("MarkAutomaticAttandance")]
        public JsonResult<object> MarkAutomaticAttandance()
        {
            var obj = new object();
            Dictionary<string, string> dic = new Dictionary<string, string>();
            DateTime CheckTime = DateTime.Now.Date.AddHours(16);
            if (DateTime.Now > CheckTime)
            {
                List<AspNetUser> Employees = db.AspNetUsers.ToList();
                for (int i = 0; i < Employees.Count; i++)
                {
                    string UserID = Employees[i].Id;
                    var userdeRole = db.UserRoles.Where(x => x.UserID == UserID).FirstOrDefault();

                    DateTime todayDate = DateTime.Now.Date;
                    var attendanceDetails = db.Attendances.Where(x => x.DateOfDay >= todayDate && x.EmployeeID == UserID).FirstOrDefault();

                    if (attendanceDetails == null)
                    {
                        bool AttendanceValid = db.Attendances.Any(c => c.DateOfDay == todayDate && c.EmployeeID == UserID);
                        DateTime now = DateTime.Now;
                        Attendance Attendance = new Attendance
                        {
                            EmployeeID = UserID,
                            DateOfDay = DateTime.Now.Date,

                        };
                        db.Attendances.Add(Attendance);
                        db.SaveChanges();

                        var leavedetails = db.Leave_Management_Leave_Taken.Where(x => x.fk_user_id == UserID && x.leave_days == todayDate).FirstOrDefault();
                        if (leavedetails == null)
                        {

                            var Rolename = db.AspNetRoles.Where(x => x.Name == "Admin").FirstOrDefault();
                            var AdminID = db.UserRoles.Where(x => x.RoleId == Rolename.Id).FirstOrDefault();
                            DateTime d = todayDate;
                            Leave_Management_Leave_Taken lt = new Leave_Management_Leave_Taken();
                            lt.fk_leave_type = 4;
                            lt.leave_days = d;
                            lt.fk_user_id = UserID;
                            lt.leave_status = 1;
                            lt.is_halfday = false;
                            lt.TotalLeave = 1;
                            lt.fk_leave_type = 6;
                            lt.Remark = "Atomatic Leave";
                            lt.LeaveDateFrom = todayDate;
                            lt.LeaveDateTo = todayDate.AddDays(1);
                            lt.fk_approved_user_id = AdminID.UserID;
                            db.Leave_Management_Leave_Taken.Add(lt);
                            db.SaveChanges();
                        }

                    
                    }
                   
                }
                dic.Add("Status", "1");
                dic.Add("Message", "Updated Mark Automatic Attandance ");

            }

            obj = dic;
            return Json(obj);

        }

    }
}
