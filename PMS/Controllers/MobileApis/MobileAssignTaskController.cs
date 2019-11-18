using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PMS.Models;
using PMS_API.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Results;
using static PMS_API.Models.CommonModal;
using Microsoft.AspNet.Identity.EntityFramework;
using PMS;

namespace PMS_API.Controllers
{
    [RoutePrefix("api/AssignTask")]
    [Authorize]
    public class MobileAssignTaskController : ApiController
    {
        PMSEntities db = new PMSEntities();

        [HttpPost]
        [Route("InsertAssignTask")]
        public JsonResult<object> InsertAssignTask([FromBody]JObject json)//AssignViewModel task)
        {
            Attendance re = JsonConvert.DeserializeObject<Attendance>(json.ToString());
            var obj = new object();
            Dictionary<string, object> dic = new Dictionary<string, object>();

            try
            {
                tbl_Task taskdetails = new tbl_Task();
                string taskid = Guid.NewGuid().ToString();
                taskdetails.TaskId = taskid;

                db.tbl_Task.Add(taskdetails);
                db.SaveChanges();
                //return RedirectToAction("Tasklist", "Task");
                var newUrl = this.Url.Link("Default", new
                {
                    Controller = "Account",
                    Action = "Register"
                });
               // return Request.CreateResponse(HttpStatusCode.OK, new { Success = true, RedirectUrl = newUrl });
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
        [Route("GetUserlist")]
        public JsonResult<object> GetUserlist([FromBody]JObject json)
        {

            PMSEntities context = new PMSEntities();
            PagingParameterModel re = JsonConvert.DeserializeObject<PagingParameterModel>(json.ToString());
            var obj = new object();
            List<UserViewModel> UserList = new List<UserViewModel>();
            Dictionary<string, object> dic = new Dictionary<string, object>();
            try
            {
                List<SelectListItem> userlist = (from p in context.AspNetUsers
                                             join r in context.UserRoles on p.Id equals r.UserID
                                             where r.RoleId == "1"
                                             select new SelectListItem
                                             {
                                                 Text = p.Name,
                                                 Value = p.Id.ToString()
                                             }).ToList();
                int CurrentPage = re.pageNumber;
                int PageSize = re._pageSize;
                int count = userlist.Count();
                int TotalCount = count;
                int TotalPages = (int)Math.Ceiling(count / (double)PageSize);

                // Returns List of Customer after applying Paging   
                var items = userlist.Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();

                var previousPage = CurrentPage > 1 ? "Yes" : "No";
                var nextPage = CurrentPage < TotalPages ? "Yes" : "No";
                // Object which we are going to send in header   
                var paginationMetadata = new
                {
                    totalCount = TotalCount,
                    pageSize = PageSize,
                    currentPage = CurrentPage,
                    totalPages = TotalPages,
                    previousPage,
                    nextPage
                };
                var userlists = userlist;
                dic.Add("result", userlists);
                dic.Add("Message", "userlist");
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
        [Route("AssignTaskbyUserId")]
        public JsonResult<object> AssignTaskbyUserId([FromBody]JObject json)//AssignViewModel task)
        {
            AssignViewModel re = JsonConvert.DeserializeObject<AssignViewModel>(json.ToString());
            var obj = new object();
            Dictionary<string, object> dic = new Dictionary<string, object>();

            try

            {
                var userid = User.Identity.GetUserId();
                AssignTask taskdata = new AssignTask();
                taskdata.TaskId = re.taskId;
                taskdata.ToAssignId = re.toassignId;
                taskdata.FromassignId = userid;
                taskdata.CreatedOn = DateTime.Now;
                db.AssignTasks.Add(taskdata);
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
        [Route("AssignTasklistData")]
        public JsonResult<object> AssignTasklistData([FromBody]JObject json)
        {
            Attendance re = JsonConvert.DeserializeObject<Attendance>(json.ToString());
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

        [HttpPost]
        [Route("viewAssigntaskbyId")]
        public JsonResult<object> viewAssigntaskbyId([FromBody]JObject json)//string taskId)
        {
                IdModal re = JsonConvert.DeserializeObject<IdModal>(json.ToString());
                var obj = new object();
                Dictionary<string, object> dic = new Dictionary<string, object>();
                string taskId = re.Id;
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


        [HttpPost]
        [Route("ChangeStatusbyUserId")]
        public JsonResult<object> ChangeStatusbyUserId([FromBody]JObject json)//ChangeTaskStatusViewModel task)
        {
            ChangeTaskStatusViewModel re = JsonConvert.DeserializeObject<ChangeTaskStatusViewModel>(json.ToString());
            var obj = new object();
            Dictionary<string, object> dic = new Dictionary<string, object>();

            try
            {
                var userid = User.Identity.GetUserId();
                var UpdateTask = db.tbl_Task.Where(x => x.TaskId == re.taskId).FirstOrDefault();
                UpdateTask.Status = Convert.ToInt32(re.Status);
                db.Entry(UpdateTask).State = EntityState.Modified;
                db.SaveChanges();

                ChangeIssueDetail ChangeStatusRemark = new ChangeIssueDetail();
                ChangeStatusRemark.statusid = Convert.ToInt32(re.Status);
                ChangeStatusRemark.CreatedOn = DateTime.Now;
                ChangeStatusRemark.Createdby = userid;
                ChangeStatusRemark.issueid = re.taskId;
                ChangeStatusRemark.Remark = re.Remark;
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
        public JsonResult<object> TaskFollowList([FromBody]JObject json)//TaskFollowedViewModel followed)
        {
            TaskFollowedViewModel re = JsonConvert.DeserializeObject<TaskFollowedViewModel>(json.ToString());
            var obj = new object();
            Dictionary<string, object> dic = new Dictionary<string, object>();

            try
            {
                var userid = User.Identity.GetUserId();
                List<AssignDetailsViewModel> AssignTaskListData = new List<AssignDetailsViewModel>();

                // var AssignTaskList= db.AssignTasks.Where(x => x.ToAssignId == userid).ToList();
                var TaskFollowList = (from a in db.AspNetUsers
                                      join t in db.ChangeIssueDetails on a.Id equals t.Createdby
                                      where t.issueid == re.TaskId
                                      select new TaskFollowedViewModel
                                      {
                                          FollwID = t.EfollowId,
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