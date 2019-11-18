using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PMS.Models;

namespace PMS.Controllers
{
    public class UserController : Controller
    {
        PMSEntities db = new PMSEntities();
        
        public ActionResult UserAssignTask()
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
        public ActionResult UserAssignTaskList()
        {
            var obj = new object();
            Dictionary<string, object> dic = new Dictionary<string, object>();
            try
            {

                var userid = User.Identity.GetUserId();
                List<AssignDetailsViewModel> AssignTaskListData = new List<AssignDetailsViewModel>();

                var AssignTaskList = db.AssignTasks.Where(x=>x.ToAssignId==userid).ToList();

                if (AssignTaskList.Count > 0)
                {

                    for (int i = 0; i < AssignTaskList.Count; i++)
                    {

                        string taskid = AssignTaskList[i].TaskId;
                        string UserId = AssignTaskList[i].ToAssignId;
                        string fromid = AssignTaskList[i].FromassignId;

                        var taskdetails = db.tbl_Task.Where(x => x.TaskId == taskid).FirstOrDefault();

                        var touserdetails = db.AspNetUsers.Where(x => x.Id == UserId).FirstOrDefault();

                        var fromuserdetails = db.AspNetUsers.Where(x => x.Id == fromid).FirstOrDefault();
                        var projectdetails = db.ProjectMasters.Where(x => x.ProjectID == taskdetails.projectid).FirstOrDefault();

                        if (taskdetails != null)
                        {


                            AssignTaskListData.Add(new AssignDetailsViewModel
                            {
                                description = taskdetails.Description,
                                TaskName = taskdetails.SummaryName,
                                Devicetype = taskdetails.devicetype,
                                taskId = taskdetails.TaskId,
                                id = taskdetails.id,
                                projectname = projectdetails.ProjectName,
                                status = taskdetails.Status.ToString(),
                                AssignTouserName = touserdetails.Name,
                                AssignFromUserName = fromuserdetails.Name

                            });
                        }

                    }
                    var AssignTaskLists = AssignTaskListData;
                    dic.Add("result", AssignTaskLists);
                    dic.Add("Message", "AssignTaskListData");
                    dic.Add("Status", "1");
                }
                else
                {
                    dic.Add("Message", "No Data available");
                    dic.Add("Status", "2");
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
        public ActionResult OpenTask()
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
        public ActionResult UserOpenTaskList()
        {
            var obj = new object();
            Dictionary<string, object> dic = new Dictionary<string, object>();
            try
            {

                var userid = User.Identity.GetUserId();
                List<AssignDetailsViewModel> AssignTaskListData = new List<AssignDetailsViewModel>();

               // var AssignTaskList= db.AssignTasks.Where(x => x.ToAssignId == userid).ToList();
                var AssignTaskList = (from a in db.AssignTasks join t in db.tbl_Task on a.TaskId equals t.TaskId where a.ToAssignId==userid && t.Status==1 select a).ToList();

                if (AssignTaskList.Count > 0)
                {

                    for (int i = 0; i < AssignTaskList.Count; i++)
                    {

                        string taskid = AssignTaskList[i].TaskId;
                        string UserId = AssignTaskList[i].ToAssignId;
                        string fromid = AssignTaskList[i].FromassignId;

                        var taskdetails = db.tbl_Task.Where(x => x.TaskId == taskid).FirstOrDefault();

                        var touserdetails = db.AspNetUsers.Where(x => x.Id == UserId).FirstOrDefault();

                        var fromuserdetails = db.AspNetUsers.Where(x => x.Id == fromid).FirstOrDefault();
                        var projectdetails = db.ProjectMasters.Where(x => x.ProjectID == taskdetails.projectid).FirstOrDefault();

                        if (taskdetails != null)
                        {


                            AssignTaskListData.Add(new AssignDetailsViewModel
                            {
                                description = taskdetails.Description,
                                TaskName = taskdetails.SummaryName,
                                Devicetype = taskdetails.devicetype,
                                taskId = taskdetails.TaskId,
                                id = taskdetails.id,
                                projectname = projectdetails.ProjectName,
                                status = taskdetails.Status.ToString(),
                                AssignTouserName = touserdetails.Name,
                                AssignFromUserName = fromuserdetails.Name

                            });
                        }

                    }
                    var AssignTaskLists = AssignTaskListData;
                    dic.Add("result", AssignTaskLists);
                    dic.Add("Message", "AssignTaskListData");
                    dic.Add("Status", "1");
                }
                else
                {
                    dic.Add("Message", "No Data available");
                    dic.Add("Status", "2");
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
        public ActionResult ReOpenTask()
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
        public ActionResult UserReOpenTaskList()
        {
            var obj = new object();
            Dictionary<string, object> dic = new Dictionary<string, object>();
            try
            {

                var userid = User.Identity.GetUserId();
                List<AssignDetailsViewModel> AssignTaskListData = new List<AssignDetailsViewModel>();

                //var AssignTaskList = db.AssignTasks.Where(x => x.ToAssignId == userid).ToList();
                var AssignTaskList = (from a in db.AssignTasks join t in db.tbl_Task on a.TaskId equals t.TaskId where a.ToAssignId == userid && t.Status == 2 select a).ToList();
                if (AssignTaskList.Count > 0)
                {

                    for (int i = 0; i < AssignTaskList.Count; i++)
                    {

                        string taskid = AssignTaskList[i].TaskId;
                        string UserId = AssignTaskList[i].ToAssignId;
                        string fromid = AssignTaskList[i].FromassignId;

                        var taskdetails = db.tbl_Task.Where(x => x.TaskId == taskid).FirstOrDefault();

                        var touserdetails = db.AspNetUsers.Where(x => x.Id == UserId).FirstOrDefault();

                        var fromuserdetails = db.AspNetUsers.Where(x => x.Id == fromid).FirstOrDefault();
                        var projectdetails = db.ProjectMasters.Where(x => x.ProjectID == taskdetails.projectid).FirstOrDefault();

                        if (taskdetails != null)
                        {


                            AssignTaskListData.Add(new AssignDetailsViewModel
                            {
                                description = taskdetails.Description,
                                TaskName = taskdetails.SummaryName,
                                Devicetype = taskdetails.devicetype,
                                taskId = taskdetails.TaskId,
                                id = taskdetails.id,
                                projectname = projectdetails.ProjectName,
                                status = taskdetails.Status.ToString(),
                                AssignTouserName = touserdetails.Name,
                                AssignFromUserName = fromuserdetails.Name

                            });
                        }

                    }
                    var AssignTaskLists = AssignTaskListData;
                    dic.Add("result", AssignTaskLists);
                    dic.Add("Message", "AssignTaskListData");
                    dic.Add("Status", "1");
                }
                else
                {
                    dic.Add("Message", "No Data available");
                    dic.Add("Status", "2");
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
        public ActionResult CompletedTask()
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
        public ActionResult UserCompletTaskList()
        {
            var obj = new object();
            Dictionary<string, object> dic = new Dictionary<string, object>();
            try
            {

                var userid = User.Identity.GetUserId();
                List<AssignDetailsViewModel> AssignTaskListData = new List<AssignDetailsViewModel>();

               // var AssignTaskList = db.AssignTasks.Where(x => x.ToAssignId == userid).ToList();
                var AssignTaskList = (from a in db.AssignTasks join t in db.tbl_Task on a.TaskId equals t.TaskId where a.ToAssignId == userid && t.Status == 3 select a).ToList();
                if (AssignTaskList.Count > 0)
                {

                    for (int i = 0; i < AssignTaskList.Count; i++)
                    {

                        string taskid = AssignTaskList[i].TaskId;
                        string UserId = AssignTaskList[i].ToAssignId;
                        string fromid = AssignTaskList[i].FromassignId;

                        var taskdetails = db.tbl_Task.Where(x => x.TaskId == taskid).FirstOrDefault();

                        var touserdetails = db.AspNetUsers.Where(x => x.Id == UserId).FirstOrDefault();

                        var fromuserdetails = db.AspNetUsers.Where(x => x.Id == fromid).FirstOrDefault();
                        var projectdetails = db.ProjectMasters.Where(x => x.ProjectID == taskdetails.projectid).FirstOrDefault();

                        if (taskdetails != null)
                        {


                            AssignTaskListData.Add(new AssignDetailsViewModel
                            {
                                description = taskdetails.Description,
                                Devicetype = taskdetails.devicetype,
                                taskId = taskdetails.TaskId,
                                id = taskdetails.id,
                                projectname = projectdetails.ProjectName,
                                status = taskdetails.Status.ToString(),
                                AssignTouserName = touserdetails.Name,
                                AssignFromUserName = fromuserdetails.Name

                            });
                        }

                    }
                    var AssignTaskLists = AssignTaskListData;
                    dic.Add("result", AssignTaskLists);
                    dic.Add("Message", "AssignTaskListData");
                    dic.Add("Status", "1");
                }
                else
                {
                    dic.Add("Message", "No Data available");
                    dic.Add("Status", "2");
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
        public ActionResult Calendar()
        {
            return View();
        }

    }
}
