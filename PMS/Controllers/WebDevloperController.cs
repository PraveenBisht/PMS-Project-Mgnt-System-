using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Security.Claims;
using System.Web.Security;
using Microsoft.AspNet.Identity;
using PMS.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;

namespace PMS.Controllers
{
    public class WebDevloperController : Controller
    {
        PMSEntities db = new PMSEntities();
        public string RoleName { get; set; }
        // GET: Users
  
        public ActionResult Dashboard(AttendanceViewModel vm)
        {
            try
            {
                if (User.Identity.IsAuthenticated)
                {
                    int opentask = 0, reopentask = 0, completedtask = 0, pendingleave = 0, ApproveLeave = 0, Rejectedleave = 0;
                    var userid = User.Identity.GetUserId();
                    var user = User.Identity;
                    ViewBag.Name = user.Name;
                    ApplicationDbContext context = new ApplicationDbContext();
                    var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

                    var s = UserManager.GetRoles(user.GetUserId());
                    RoleName = s[0].ToString();
                    ViewBag.displayMenu = "No";
                    ViewBag.UserRole = RoleName;

                    if (RoleName == "Admin")
                    {
                        Session["displayMenu"] = "Admin";

                        var TotalTask = (from a in db.AssignTasks join t in db.tbl_Task on a.TaskId equals t.TaskId select t).ToList();
                        ViewBag.TotalTask = TotalTask.Count;

                        if (TotalTask.Count > 0)
                        {
                            ViewBag.TotalTask = TotalTask.Count;

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
                            ViewBag.TotalOpenTask = opentask;
                            ViewBag.TotalReOpenTask = reopentask;
                            ViewBag.TotalcompletTask = completedtask;
                        }
                    }
                    if (RoleName == "User")
                    {
                        // ViewBag.displayMenu = "User"
                        Session["displayMenu"] = "User";

                        var TotalTask = (from a in db.AssignTasks join t in db.tbl_Task on a.TaskId equals t.TaskId where a.ToAssignId == userid select t).ToList();
                        ViewBag.TotalTask = TotalTask.Count;

                        if (TotalTask.Count > 0)
                        {
                            ViewBag.TotalTask = TotalTask.Count;

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
                            ViewBag.TotalOpenTask = opentask;
                            ViewBag.TotalReOpenTask = reopentask;
                            ViewBag.TotalcompletTask = completedtask;
                        }

                        var leavedetails = (from l in db.Leave_Management_Leave_Taken
                                            where l.leave_days.Value.Year >= DateTime.Now.Year
                                            && l.leave_days.Value.Month >= DateTime.Now.Month - 3 && l.fk_user_id == userid
                                            select l).ToList();
                        if (leavedetails.Count > 0)
                        {
                            for (int i = 0; i < leavedetails.Count; i++)
                            {
                                if (leavedetails[i].leave_status == 1)
                                {
                                    ApproveLeave++;
                                }
                                else if (leavedetails[i].leave_status == 2)
                                {
                                    Rejectedleave++;
                                }
                                else
                                {
                                    pendingleave++;

                                }


                            }
                            ViewBag.Approve = ApproveLeave;
                            ViewBag.Rejected = Rejectedleave;
                            ViewBag.Pending = pendingleave;

                        }

                    }
                    if (RoleName == "Manager")
                    {
                        Session["displayMenu"] = "Manager";
                        var TotalTask = (from a in db.AssignTasks join t in db.tbl_Task on a.TaskId equals t.TaskId select t).ToList();
                        ViewBag.TotalTask = TotalTask.Count;

                        if (TotalTask.Count > 0)
                        {
                            ViewBag.TotalTask = TotalTask.Count;

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
                            ViewBag.TotalOpenTask = opentask;
                            ViewBag.TotalReOpenTask = reopentask;
                            ViewBag.TotalcompletTask = completedtask;
                        }
                    }

                    // DateTime todayDate = Convert.ToDateTime(DateTime.Now.ToString("dd-MM-yyyy"));
                    DateTime todaydatetime = DateTime.Now;
                    DateTime Checktime = DateTime.Now.Date.AddHours(15);
                 
                    if(todaydatetime> Checktime)
                    {
                        vm.AttBtnHideShow = true;
                    }
                    else
                    {
                        vm.AttBtnHideShow = false;
                    }


                    DateTime todayDate = DateTime.Now.Date;
                    bool AttendanceValid = db.Attendances.Any(c => c.DateOfDay == todayDate && c.EmployeeID == userid);

                    if (AttendanceValid == true)
                    {
                        vm.iscoming = true;
                        bool AttendanceAllValid = db.Attendances.Any(c => c.DateOfDay == todayDate && c.EmployeeID == userid && c.LeaveTime != null);
                        if (AttendanceAllValid)
                        {
                            vm.isLeave = true;
                        }

                    }
                    else
                    {
                        vm.iscoming = false;
                    }
                    var userId = User.Identity.GetUserId();
                    var userdetails = db.AspNetUsers.Where(x => x.Id == userId).FirstOrDefault();
                  
                    Session["EmailId"] = userdetails.Email.ToString();
                    Session["Name"] = userdetails.Name.ToString();

                    return View(vm);
                }
                else
                {
                    return RedirectToAction("Login", "Home");
                }
            }
            catch(Exception ex)
            {
                throw;
            
            }

        }

        [HttpPost]
        [Authorize]
        public ActionResult Dashboard(Attendance attendance, AttendanceViewModel vm)
        {
            var userid = User.Identity.GetUserId();
            //DateTime myDateTime = Convert.ToDateTime(DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss"));

            //DateTime todayDate = Convert.ToDateTime(DateTime.Now.ToString("dd-MM-yyyy"));
            DateTime myDateTime = DateTime.Now;

            DateTime todayDate = DateTime.Now.Date; ;
            DateTime CheckTime = DateTime.Now.Date.AddHours(15);
            bool AttendanceValid = db.Attendances.Any(c => c.DateOfDay == todayDate && c.EmployeeID == userid);

            if (AttendanceValid)
            {
                var attendanceRow = db.Attendances.Where(c => c.DateOfDay == todayDate && c.EmployeeID == userid).Single();
                vm.iscoming = true;
                vm.isLeave = true;
                attendanceRow.LeaveTime = myDateTime;


                db.Entry(attendanceRow).State = EntityState.Modified;
                db.SaveChanges();

                return View(vm);

            }
            else
            {
                DateTime now = DateTime.Now;
                vm.isLeave = false;
                Attendance Attendance = new Attendance
                {
                    EmployeeID = userid,
                    ComingTime = myDateTime,
                   // DateOfDay = Convert.ToDateTime(DateTime.Now.ToString("dd-MM-yyyy")),
                    DateOfDay = DateTime.Now.Date,

                };

                db.Attendances.Add(Attendance);
                db.SaveChanges();
                vm.iscoming = true;

                if (myDateTime>CheckTime)
                {
                    var leavedetails = db.Leave_Management_Leave_Taken.Where(x => x.fk_user_id == userid && x.leave_days == todayDate).FirstOrDefault();
                    if (leavedetails == null)
                    {
                        DateTime d = todayDate;

                        Leave_Management_Leave_Taken lt = new Leave_Management_Leave_Taken();
                        lt.fk_leave_type = 4;
                        lt.leave_days = d;
                        lt.fk_user_id = userid;
                        lt.leave_status = 1;
                        lt.is_halfday = true;
                        lt.is_afternoon = false;
                        db.Leave_Management_Leave_Taken.Add(lt);
                        db.SaveChanges();
                    }
                }
                return View(vm);
            }

        }

    
        public ActionResult MarkAutomaticAttandance()
        {
           
            DateTime CheckTime=DateTime.Now.Date.AddHours(15);
            if (DateTime.Now>CheckTime)
            {
                List<AspNetUser> Employees = db.AspNetUsers.ToList();
                for (int i = 0; i < Employees.Count; i++)
                {
               
                    string UserID = Employees[i].Id;
                    var userdeRole = db.UserRoles.Where(x => x.UserID ==UserID).FirstOrDefault();
                    if (userdeRole.RoleId!="2")
                    {
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
                                DateTime d = todayDate;

                                Leave_Management_Leave_Taken lt = new Leave_Management_Leave_Taken();
                                lt.fk_leave_type = 4;
                                lt.leave_days = d;
                                lt.fk_user_id = UserID;
                                lt.leave_status = 1;
                                lt.is_halfday = false;
                                db.Leave_Management_Leave_Taken.Add(lt);
                                db.SaveChanges();
                            }

                        }
                    }

                }
            }
            return View();
            }

        public ActionResult GoTODashBoard()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Dashboard", "WebDevloper");

            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }



    }

}