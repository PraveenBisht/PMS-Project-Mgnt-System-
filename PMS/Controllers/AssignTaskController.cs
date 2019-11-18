using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PMS.Models;
namespace PMS.Controllers
{
    public class AssignTaskController : Controller
    {
        PMSEntities db = new PMSEntities();
        
        public ActionResult AssignTask()
        {
            if (User.Identity.IsAuthenticated)
            {
                //List<SelectListItem> Userlist = GetUserlist();
                //ViewBag.Userlist = Userlist;
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }

        }

     

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult InsertAssignTask(AssignViewModel task)
        {

            tbl_Task taskdetails = new tbl_Task();
            string taskid = Guid.NewGuid().ToString();
            taskdetails.TaskId = taskid;
        
            db.tbl_Task.Add(taskdetails);
            db.SaveChanges();
            return RedirectToAction("Tasklist", "Task");

        }



        [HttpPost]
        public JsonResult GetUserlist()
        {

            PMSEntities context = new PMSEntities();


            List<SelectListItem> userlist = (from p in context.AspNetUsers
                                             join r in context.UserRoles on p.Id equals r.UserID
                                             select new SelectListItem
                                             {
                                                 Text = p.Name,
                                                 Value = p.Id.ToString()
                                             }).ToList();


            return Json(userlist);
        }



        [HttpPost]
        [Route("AssignTaskbyUserId")]
        public ActionResult AssignTaskbyUserId(AssignViewModel task)
        {
            var obj = new object();
            Dictionary<string, object> dic = new Dictionary<string, object>();
            try
            {
                var userid = User.Identity.GetUserId();
                AssignTask taskdata = new AssignTask();
                taskdata.TaskId = task.taskId;
                taskdata.ToAssignId = task.toassignId;
                taskdata.FromassignId = userid;
                taskdata.CreatedOn = DateTime.Now;
                db.AssignTasks.Add(taskdata);
                db.SaveChanges();
                var UpdateTask = db.tbl_Task.Where(x => x.TaskId == task.taskId).FirstOrDefault();
                UpdateTask.CreatedTo = task.toassignId;
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

        public ActionResult AssignTasklist()
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
        public ActionResult AssignTasklistData()
        {
            var obj = new object();
            Dictionary<string, object> dic = new Dictionary<string, object>();

            try
            {

                List<AssignDetailsViewModel> AssignTaskListData = new List<AssignDetailsViewModel>();
         
                var AssignTaskList = db.AssignTasks.ToList();

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
                                AssignTouserName= touserdetails.Name,
                                AssignFromUserName= fromuserdetails.Name

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
        public ActionResult AssignTaskDetails()
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
        public ActionResult viewAssigntaskbyId(string taskId)
        {
            if (User.Identity.IsAuthenticated)
            {

                var obj = new object();
                Dictionary<string, object> dic = new Dictionary<string, object>();
                try
                {
                    List<AssignDetailsViewModel> TaskList = new List<AssignDetailsViewModel>();
                    var taskdetails = db.tbl_Task.Where(x => x.TaskId == taskId).FirstOrDefault();
                    var taskimagefile = db.Filedetails.Where(x => x.TaskId == taskId).ToList();
                    var projectdetails = db.ProjectMasters.Where(x => x.ProjectID == taskdetails.projectid).FirstOrDefault();

                    var assigntaskdata = db.AssignTasks.Where(x => x.TaskId == taskId).FirstOrDefault();
                    var touserdetails = db.AspNetUsers.Where(x => x.Id == assigntaskdata.ToAssignId).FirstOrDefault();

                    var fromuserdetails = db.AspNetUsers.Where(x => x.Id == assigntaskdata.FromassignId).FirstOrDefault();
                    TaskList.Add(new AssignDetailsViewModel
                    {
                        description = taskdetails.Description,
                        TaskName = taskdetails.SummaryName,
                        Devicetype = taskdetails.devicetype,
                        taskId = taskdetails.TaskId,
                        id = taskdetails.id,
                        projectname = projectdetails.ProjectName,
                        Createdon = taskdetails.Createdon.ToString(),
                        status = taskdetails.Status.ToString(),
                        AssignTouserName = touserdetails.Name,
                        AssignFromUserName = fromuserdetails.Name


                    }); ;
                    var TaskListData = TaskList;
                    if (taskimagefile != null)
                    {
                        dic.Add("taskImage", taskimagefile);
                        dic.Add("Taskdetails", TaskListData);
                        dic.Add("Message", "taskdetails");
                        dic.Add("Status", "1");
                    }
                    else
                    {

                        dic.Add("Message", "No data available.");
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
            else
            {
                return RedirectToAction("Login", "Home");
            }


        }

        [HttpPost]
        [Route("ChangeStatusbyUserId")]
        public ActionResult ChangeStatusbyUserId(ChangeTaskStatusViewModel task)
        {
            var obj = new object();
            Dictionary<string, object> dic = new Dictionary<string, object>();
            try
            {
                var userid = User.Identity.GetUserId();
                var UpdateTask = db.tbl_Task.Where(x => x.TaskId == task.taskId).FirstOrDefault();
                UpdateTask.Status = Convert.ToInt32(task.Status);
                db.Entry(UpdateTask).State = EntityState.Modified;
                db.SaveChanges();

                ChangeIssueDetail ChangeStatusRemark = new ChangeIssueDetail();
                ChangeStatusRemark.statusid = Convert.ToInt32(task.Status);
                ChangeStatusRemark.CreatedOn = DateTime.Now;
                ChangeStatusRemark.Createdby = userid;
                ChangeStatusRemark.issueid = task.taskId;
                ChangeStatusRemark.Remark = task.Remark;
                db.ChangeIssueDetails.Add(ChangeStatusRemark);
                db.SaveChanges();
             
                dic.Add("Message", "TaskFollowList");
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
        [Route("TaskFollowList")]
        public ActionResult TaskFollowList(TaskFollowedViewModel followed)
        {
            var obj = new object();
            Dictionary<string, object> dic = new Dictionary<string, object>();
            try
            {
                var userid = User.Identity.GetUserId();
                List<AssignDetailsViewModel> AssignTaskListData = new List<AssignDetailsViewModel>();

                // var AssignTaskList= db.AssignTasks.Where(x => x.ToAssignId == userid).ToList();
                var TaskFollowList = (from a in db.AspNetUsers join t in db.ChangeIssueDetails on a.Id equals t.Createdby where t.issueid == followed.TaskId
                                      select new TaskFollowedViewModel
                                      {
                                          FollwID=t.EfollowId,
                                          Createdby = a.Name,
                                          CreatedOn = t.CreatedOn.ToString(),
                                          Statusid = t.statusid.ToString(),
                                          Remark = t.Remark

                                      }).ToList();

               
                    var TaskFollowListData = TaskFollowList;
                    dic.Add("TaskFollowList", TaskFollowListData);
                    dic.Add("Message", "TaskFollowList");
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


    }
}
