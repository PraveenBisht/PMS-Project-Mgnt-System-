using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using PMS.Models;
namespace PMS.Controllers
{
    public class TaskController : Controller
    {

        PMSEntities db = new PMSEntities();
        //
        // GET: /Support/

        public ActionResult Tasklist()
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

     
        public ActionResult TasklistData()
        {
            var obj = new object();
            Dictionary<string, object> dic = new Dictionary<string, object>();
          
            try
            {
             
                List<TaskViewModel> TaskList = new List<TaskViewModel>();
                List<TaskViewModel> taskfileList = new List<TaskViewModel>();
                var tasklistdetails = db.tbl_Task.ToList();
          
                if (tasklistdetails.Count > 0)
                    {
                      
                        for (int i = 0; i < tasklistdetails.Count; i++)
                        {
                        string taskid = tasklistdetails[i].TaskId;
                        var checkassigntask = db.AssignTasks.Where(x => x.TaskId == taskid).FirstOrDefault();
                        if (checkassigntask==null)
                        {
                            var taskdetails = db.tbl_Task.Where(x => x.TaskId == taskid).FirstOrDefault();

                            var projectdetails = db.ProjectMasters.Where(x => x.ProjectID == taskdetails.projectid).FirstOrDefault();
                            if (taskdetails != null)
                            {
                                var taskfiledetail = db.Filedetails.Where(x => x.TaskId == taskid).ToList();

                                for (int j = 0; j < taskfiledetail.Count; j++)
                                {
                                    var filedetails = db.Filedetails.Where(x => x.TaskId == taskid).FirstOrDefault();
                                    taskfileList.Add(new TaskViewModel
                                    {
                                        imageurl = filedetails.imageName

                                    });

                                }
                                var Taskfiledata = TaskList;

                                TaskList.Add(new TaskViewModel
                                {
                                    description = taskdetails.Description,
                                    TaskName = taskdetails.SummaryName,
                                    Devicetype = taskdetails.devicetype,
                                    taskId = taskdetails.TaskId,
                                    id = taskdetails.id,
                                    FileDetails = taskfileList,
                                    CountAttechedFile = taskfiledetail.Count + " " + "files",
                                    projectname = projectdetails.ProjectName,
                                    status = taskdetails.Status.ToString()

                                });
                            }

                        }

                        }
                        var TaskLists = TaskList;
                        dic.Add("result", TaskLists);
                        dic.Add("Message", "TaskLists");
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


        public ActionResult Create()
        {
            if (User.Identity.IsAuthenticated)
            {
                List<SelectListItem> projectList = Getprojectlist();
                ViewBag.projectlist = projectList;
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
           
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TaskViewModel task, List<HttpPostedFileBase> postedFiles)
        {
            var userid = User.Identity.GetUserId();
            tbl_Task taskdetails = new tbl_Task();
            string taskid = Guid.NewGuid().ToString();
            taskdetails.TaskId = taskid;
            taskdetails.SummaryName = task.TaskName;
            taskdetails.Description = task.description;
            taskdetails.Status = 1;
            taskdetails.Createdby = userid;
            taskdetails.Createdon = DateTime.Now.Date;
            taskdetails.devicetype = task.Devicetype;
            taskdetails.projectid =Convert.ToInt32(task.projectList);
            db.tbl_Task.Add(taskdetails);
            db.SaveChanges();

            string path = Server.MapPath("~/Document/Upload/");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            foreach (HttpPostedFileBase postedFile in postedFiles)
            {
                if (postedFile != null)
                {
                    string fileName = Path.GetFileName(postedFile.FileName);
                    postedFile.SaveAs(path + fileName);

                    Filedetail filedetails = new Filedetail();
                    filedetails.TaskId = taskid;
                    filedetails.Extension = Path.GetExtension(fileName);
                    filedetails.imageName = "/Document/Upload/" + fileName;
                    db.Filedetails.Add(filedetails);
                    db.SaveChanges();
                    ViewBag.Message += string.Format("<b>{0}</b> uploaded.<br />", fileName + Path.GetExtension(fileName));
                }
            }

            return RedirectToAction("Tasklist", "Task");

        }
        private static List<SelectListItem> Getprojectlist()
        {
            PMSEntities context = new PMSEntities();
            List<SelectListItem> projectlist = (from p in context.ProjectMasters.AsEnumerable()
                                                 select new SelectListItem
                                                 {
                                                     Text = p.ProjectName,
                                                     Value = p.ProjectID.ToString()
                                                 }).ToList();


            //Add Default Item at First Position.
            projectlist.Insert(0, new SelectListItem { Text = "Select Project", Value = "" });
            return projectlist;
        }
        public ActionResult TaskDetails()
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
        public ActionResult viewtaskbyId(string taskId)
        {
            if (User.Identity.IsAuthenticated)
            {

                var obj = new object();
                Dictionary<string, object> dic = new Dictionary<string, object>();
                try
                {
                    List<TaskViewModel> TaskList = new List<TaskViewModel>();
                    var taskdetails = db.tbl_Task.Where(x => x.TaskId == taskId).FirstOrDefault();
                    var  taskimagefile = db.Filedetails.Where(x => x.TaskId == taskId).ToList();
                    var projectdetails = db.ProjectMasters.Where(x => x.ProjectID == taskdetails.projectid).FirstOrDefault();

                    TaskList.Add(new TaskViewModel
                    {
                        description = taskdetails.Description,
                         TaskName= taskdetails.SummaryName,
                        Devicetype = taskdetails.devicetype,
                        taskId = taskdetails.TaskId,
                        id = taskdetails.id,
                        projectname = projectdetails.ProjectName,
                        Createdon = taskdetails.Createdon.ToString(),
                        status = taskdetails.Status.ToString()

                    }); ;
                    var TaskListData = TaskList;
                    if (taskimagefile!=null)
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

    
    }
}

