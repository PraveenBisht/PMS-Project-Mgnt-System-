using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PMS.Models;
namespace PMS.Controllers
{
    public class AdminController : Controller
    {
        PMSEntities db = new PMSEntities();


        // GET: Admin
        public ActionResult ChangeUserRole()
        {
            if (User.Identity.IsAuthenticated)
            {

                return View();
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }


        [HttpPost]
        [Route("ChangeUserRolebyUserId")]
        public ActionResult ChangeUserRolebyUserId(AssignViewModel request)
        {
            ApplicationDbContext context = new ApplicationDbContext();
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            var obj = new object();
            Dictionary<string, object> dic = new Dictionary<string, object>();
            try
            {
                var userid = request.Userid;
                UserManager.RemoveFromRole(userid, "User");
                var changerole = UserManager.AddToRoleAsync(userid, "Manager");
                var Rolename = db.AspNetRoles.Where(x => x.Name == "Manager").FirstOrDefault();
                var UpdateTask = db.UserRoles.Where(x => x.UserID == request.Userid).FirstOrDefault();
                UpdateTask.RoleId = Rolename.Id;
                db.Entry(UpdateTask).State = EntityState.Modified;
                db.SaveChanges();
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
                        // raise a new exception nesting  
                        // the current instance as InnerException  
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
        public JsonResult GetUserlist()
        {

            PMSEntities context = new PMSEntities();


            List<SelectListItem> userlist = (from p in context.AspNetUsers
                                             join r in context.UserRoles on p.Id equals r.UserID
                                             select new SelectListItem
                                             {
                                                 Text = p.UserName,
                                                 Value = p.Id.ToString()
                                             }).ToList();


            return Json(userlist);
        }

        public ActionResult UserList()
        {
            if (User.Identity.IsAuthenticated)
            {

                return View();
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }
        [HttpPost]
        public JsonResult GetAllUserlist()
        {
            var obj = new object();
            Dictionary<string, object> dic = new Dictionary<string, object>();
            try
            {

                List<UserViewModel> userlist = (from p in db.AspNetUsers
                                                join r in db.UserRoles on p.Id equals r.UserID
                                                join u in db.UserRoles on p.Id equals u.UserID
                                                join d in db.Departments on p.DepartMentId equals d.Id
                                                into joined
                                                from j in joined.DefaultIfEmpty()
                                                orderby u.id ascending
                                                select new UserViewModel
                                                {
                                                    Name = p.Name,
                                                    EmplyoeeId = u.id,
                                                    Email = p.Email,
                                                    id = p.Id.ToString(),
                                                    mobilenumber = p.MobileNumber,
                                                    qualification = p.Qualification,
                                                    joiningDate = p.JoiningDate.ToString(),
                                                    address = p.Address,
                                                    designation = p.Designation,
                                                    profilePic = p.ProfilePic,
                                                    DepartmentName = j.DepartmentName


                                                }

                                           ).ToList();

                dic.Add("Result", userlist);
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
                        // raise a new exception nesting  
                        // the current instance as InnerException  
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
        public JsonResult GetUserdetailsByuserId(string userId)
        {
            string departmentName;
            var obj = new object();
            Dictionary<string, object> dic = new Dictionary<string, object>();
            try
            {
                var departmentn = (from p in db.AspNetUsers
                                   join d in db.Departments on p.DepartMentId equals d.Id
                                   where p.Id == userId
                                   select new
                                   {
                                       d.Id,
                                       d.DepartmentName
                                   }
                                 ).FirstOrDefault();

                if (departmentn != null)
                {
                    departmentName = departmentn.DepartmentName;
                }
                else
                {
                    departmentName = "";
                }
                var userlist = (from p in db.AspNetUsers
                                join r in db.UserRoles on p.Id equals r.UserID
                                join u in db.UserRoles on p.Id equals u.UserID
                                where p.Id == userId
                                select new UserViewModel
                                {
                                    EmplyoeeId = u.id,
                                    Name = p.Name,
                                    Email = p.Email,
                                    id = p.Id.ToString(),
                                    mobilenumber = (p.MobileNumber == null) ? "NA" : p.MobileNumber,
                                    qualification = (p.Qualification == null) ? "NA" : p.Qualification,
                                    joiningDate = (p.JoiningDate.ToString()==null|| p.JoiningDate.ToString() ==""|| p.JoiningDate.ToString() == "NULL") ?"NA": p.JoiningDate.ToString(),
                                    address = (p.Address == null) ? "NA" : p.Address,
                                    designation = (p.Designation == null) ? "NA" : p.Designation,
                                    profilePic = (p.ProfilePic==null) ? "NA" : p.ProfilePic,
                                    DepartmentName = (departmentName=="")?"NA": departmentName

                                }

                             ).FirstOrDefault();

                dic.Add("Result", userlist);
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
                        // raise a new exception nesting  
                        // the current instance as InnerException  
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
        public JsonResult GetDepartmentlist()
        {

            PMSEntities context = new PMSEntities();


            List<SelectListItem> departmentlist = (from p in context.Departments

                                                   select new SelectListItem
                                                   {
                                                       Text = p.DepartmentName,
                                                       Value = p.Id.ToString()
                                                   }).ToList();


            return Json(departmentlist);
        }

        public ActionResult LeaveManage()
        {
            if (User.Identity.IsAuthenticated)
            {

                return View();
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        public ActionResult UserLeaveList()
        {
            if (User.Identity.IsAuthenticated)
            {

                return View();
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        [HttpPost]
        public ActionResult AllleaveByUserId(UserViewModel user)
        {
            var obj = new object();
            Dictionary<string, object> dic = new Dictionary<string, object>();

            try
            {

                List<LeaveViewModel> LeaveList = new List<LeaveViewModel>();

                var leavedeta = db.Leave_Management_Leave_Taken.Where(x => x.fk_user_id == user.id).ToList();
                var username = db.AspNetUsers.Where(x => x.Id == user.id).FirstOrDefault();
                if (leavedeta.Count > 0)
                {
                    string approvername;
                    for (int i = 0; i < leavedeta.Count; i++)
                    {
                        if (leavedeta[i].fk_approved_user_id != null)
                        {
                            string userid = leavedeta[i].fk_approved_user_id;
                            var approver = db.AspNetUsers.Where(x => x.Id == userid).FirstOrDefault();
                            approvername = approver.Name;
                        }
                        else
                        {
                            approvername = "NA";
                        }
                        LeaveList.Add(new LeaveViewModel
                        {
                            Leave_Taken_Id = leavedeta[i].pk_leave_taken_id,
                            Leavestatus = leavedeta[i].leave_status.ToString(),
                            UserName = username.Name,
                            ApproverName = approvername,
                            Leave_days = leavedeta[i].leave_days.ToString(),

                        });

                    }
                    var LeaveData = LeaveList;
                    dic.Add("result", LeaveData);
                    dic.Add("Message", "LeaveData");
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



        /// <summary>
        /// Update EditEmployee 
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        #region
        [HttpPost]
        public ActionResult EditEmployee(UserViewModel emp)
        {
            var obj = new object();
            Dictionary<string, object> dic = new Dictionary<string, object>();
            try
            {
                var UpdateEmp = db.AspNetUsers.Where(x => x.UserName == emp.Email).FirstOrDefault();
                if (UpdateEmp != null)
                {
                    UpdateEmp.Name = emp.Name;
                    UpdateEmp.Designation = emp.designation;
                    UpdateEmp.UserName = emp.Email;
                    UpdateEmp.MobileNumber = emp.mobilenumber;
                    if (emp.DepartmentId != 0)
                    {
                        UpdateEmp.DepartMentId = emp.DepartmentId;
                    }
                    UpdateEmp.Qualification = emp.qualification;
                    //UpdateEmp.JoiningDate = Convert.ToDateTime(emp.joiningDate);
                    db.Entry(UpdateEmp).State = EntityState.Modified;
                    db.SaveChanges();
                    dic.Add("Message", "Success");
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
        #endregion






    }
}