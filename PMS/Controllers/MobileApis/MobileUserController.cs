using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PMS.Models;
using PMS_API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Results;
using Microsoft.AspNet.Identity;
using PMS;
using System.Data.SqlClient;
using PMS.Models.MobileApisModels;
using static PMS_API.Models.CommonModal;

namespace PMS_API.Controllers
{
    [Authorize]
    [RoutePrefix("api/Users")]
    public class MobileUserController : ApiController
    {
        ApplicationDbContext context = new ApplicationDbContext();
        [HttpPost]
        [Route("UserAssignTaskList")]
        public JsonResult<object> UserAssignTaskList([FromBody]JObject json)
        {
            PagingParameterModel re = JsonConvert.DeserializeObject<PagingParameterModel>(json.ToString());
            var obj = new object();
            List<UserViewModel> UserList = new List<UserViewModel>();
            Dictionary<string, object> dic = new Dictionary<string, object>();

            try
            {
                PMSEntities db = new PMSEntities();
                //var userid = User.Identity.GetUserId();
                string userid = User.Identity.GetUserId();
                List<AssignDetailsViewModel> AssignTaskListData = new List<AssignDetailsViewModel>();
                var AssignTaskList = db.AssignTasks.Where(x => x.ToAssignId == userid).ToList();
                int count = AssignTaskList.Count();
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
                                createdby=taskdetails.Createdby,
                                Createdon = string.IsNullOrEmpty(Convert.ToString(taskdetails.Createdon))?"": taskdetails.Createdon?.ToString("yyyy MMM dd"),
                                projectname = projectdetails.ProjectName,
                                status = taskdetails.Status.ToString() == "1" ? "Open" : taskdetails.Status.ToString() == "2" ? "Re-open" : "Completed",
                                AssignTouserName = touserdetails.Name == null ? "" : touserdetails.Name.Trim(),
                                AssignFromUserName = fromuserdetails.Name == null ? "" : fromuserdetails.Name.Trim()

                            }); 
                        }
                    }
                    int CurrentPage = re.pageNumber;
                    int PageSize = re._pageSize;
                    int TotalCount = count;
                    int TotalPages = (int)Math.Ceiling(count / (double)PageSize);

                    // Returns List of Customer after applying Paging   
                    var items = AssignTaskListData.Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();

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
                    var AssignTaskLists = items;
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
        [Route("EmployeeTaskList")]
        public JsonResult<object> EmployeeTaskList([FromBody]JObject json)
        {
            PagingParameterModel request = JsonConvert.DeserializeObject<PagingParameterModel>(json.ToString());
            var obj = new object();
            Dictionary<string, object> dic = new Dictionary<string, object>();
            try
            {
                int FromRow = request.pageNumber;
                string userId = User.Identity.GetUserId();
                SqlParameter param1 = new SqlParameter("@FromRow", FromRow);
                SqlParameter param2 = new SqlParameter("@userId", userId);
                var EmployeeTaskList = context.Database.SqlQuery<EmpTaskListVM>("EmpTaskList @FromRow, @userId", param1, param2).ToList();
                if (EmployeeTaskList != null && EmployeeTaskList.Count>0)
                { 
                dic.Add("result", EmployeeTaskList);
                dic.Add("Message", "EmployeeTaskList");
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
        [Route("EmployeeOpenTaskList")]
        public JsonResult<object> EmployeeOpenTaskList([FromBody]JObject json)
        {
            TaskList request = JsonConvert.DeserializeObject<TaskList>(json.ToString());
            var obj = new object();
            Dictionary<string, object> dic = new Dictionary<string, object>();
            try
            {
                int FromRow = request.pageNumber;
                string userId = User.Identity.GetUserId();
                if (request.FromDate==""&& request.Todate=="")
                {
                    SqlParameter param1 = new SqlParameter("@FromRow", FromRow);
                    SqlParameter param2 = new SqlParameter("@userId", userId);
                    var EmpOpenTaskList = context.Database.SqlQuery<EmpTaskListVM>("EmpOpenTaskList @FromRow, @userId", param1, param2).ToList();


                    if (EmpOpenTaskList != null && EmpOpenTaskList.Count > 0)
                    {
                        dic.Add("result", EmpOpenTaskList);
                        dic.Add("Message", "EmpOpenTaskList");
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
                    SqlParameter param3 = new SqlParameter("@Fromdate", request.FromDate);
                    SqlParameter param4 = new SqlParameter("@Todate", request.Todate);
                    var EmpOpenTaskList = context.Database.SqlQuery<EmpTaskListVM>("EmpOpenTaskList_ByTwoDate @FromRow, @userId,@Fromdate,@Todate", param1, param2, param3, param4).ToList();

                    if (EmpOpenTaskList != null && EmpOpenTaskList.Count > 0)
                    {
                        dic.Add("result", EmpOpenTaskList);
                        dic.Add("Message", "EmpOpenTaskList");
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
        [Route("EmployeeReOpenTaskList")]
        public JsonResult<object> EmployeeReOpenTaskList([FromBody]JObject json)
        {
            TaskList request = JsonConvert.DeserializeObject<TaskList>(json.ToString());
            var obj = new object();
            Dictionary<string, object> dic = new Dictionary<string, object>();
            try
            {
                int FromRow = request.pageNumber;
                string userId = User.Identity.GetUserId();
                if (request.FromDate == "" && request.Todate == "")
                {
                    SqlParameter param1 = new SqlParameter("@FromRow", FromRow);
                    SqlParameter param2 = new SqlParameter("@userId", userId);
                    var EmpReOpenTaskList = context.Database.SqlQuery<EmpTaskListVM>("EmpReOpenTaskList @FromRow, @userId", param1, param2).ToList();
                    if (EmpReOpenTaskList != null && EmpReOpenTaskList.Count > 0)
                    {
                        dic.Add("result", EmpReOpenTaskList);
                        dic.Add("Message", "EmpReOpenTaskList");
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
                    SqlParameter param3 = new SqlParameter("@Fromdate", request.FromDate);
                    SqlParameter param4 = new SqlParameter("@Todate", request.Todate);
                    var EmpOpenTaskList = context.Database.SqlQuery<EmpTaskListVM>("EmpReOpenTaskList_ByTwoDate @FromRow, @userId,@Fromdate,@Todate", param1, param2, param3, param4).ToList();

                    if (EmpOpenTaskList != null && EmpOpenTaskList.Count > 0)
                    {
                        dic.Add("result", EmpOpenTaskList);
                        dic.Add("Message", "EmpOpenTaskList");
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
        [Route("EmployeeCompletedTaskList")]
        public JsonResult<object> EmployeeCompletedTaskList([FromBody]JObject json)
        {
            TaskList request = JsonConvert.DeserializeObject<TaskList>(json.ToString());
            var obj = new object();
            Dictionary<string, object> dic = new Dictionary<string, object>();
            try
            {
                int FromRow = request.pageNumber;
                string userId = User.Identity.GetUserId();
                if (request.FromDate == "" && request.Todate == "")
                {
                    SqlParameter param1 = new SqlParameter("@FromRow", FromRow);
                    SqlParameter param2 = new SqlParameter("@userId", userId);
                    var EmpCompletedTaskList = context.Database.SqlQuery<EmpTaskListVM>("EmpCompletedTaskList @FromRow, @userId", param1, param2).ToList();
                    if (EmpCompletedTaskList != null && EmpCompletedTaskList.Count > 0)
                    {
                        dic.Add("result", EmpCompletedTaskList);
                        dic.Add("Message", "EmployeeCompletedTaskList");
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
                    SqlParameter param3 = new SqlParameter("@Fromdate", request.FromDate);
                    SqlParameter param4 = new SqlParameter("@Todate", request.Todate);
                    var EmpOpenTaskList = context.Database.SqlQuery<EmpTaskListVM>("EmpCompletedTaskList_ByTwoDate @FromRow, @userId,@Fromdate,@Todate", param1, param2, param3, param4).ToList();

                    if (EmpOpenTaskList != null && EmpOpenTaskList.Count > 0)
                    {
                        dic.Add("result", EmpOpenTaskList);
                        dic.Add("Message", "EmpOpenTaskList");
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
        [Route("EmpCompletedTaskDetail")]
        public JsonResult<object> EmpCompletedTaskDetail([FromBody]JObject json)
        {
            IdModal request = JsonConvert.DeserializeObject<IdModal>(json.ToString());
            var obj = new object();
            Dictionary<string, object> dic = new Dictionary<string, object>();
            try
            {
                string TaskId = request.Id;
                string userId = User.Identity.GetUserId();
                string pp = "EmpCompletedTaskDetail '" + userId + "','" + TaskId + "'";
                var EmpCompletedTaskDetail = new ApplicationDbContext().MultipleResults(pp).With<EmpTaskListVM>().With<Img>().Execute();
                EmpTaskListVM vm = new EmpTaskListVM();
                if (EmpCompletedTaskDetail[0] != null)
                {
                    var aa = EmpCompletedTaskDetail[0];
                    foreach (dynamic item in aa)
                    {
                        vm.TaskId = item.TaskId;
                        vm.ProjectName = item.ProjectName;
                        vm.TaskName = item.TaskName;
                        vm.Createdby = item.Createdby;
                        vm.CreatedTo = item.CreatedTo;
                        vm.Status = item.Status;
                        vm.Description = item.Description;
                        vm.Createdon = item.Createdon;
                    }
                    dic.Add("result", vm);
                    dic.Add("ImagesResult", EmpCompletedTaskDetail[1]);
                    dic.Add("Message", "EmpCompletedTaskDetail");
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
        [Route("EmpOpenTaskDetail")]
        public JsonResult<object> EmpOpenTaskDetail([FromBody]JObject json)
        {
            IdModal request = JsonConvert.DeserializeObject<IdModal>(json.ToString());
            var obj = new object();
            Dictionary<string, object> dic = new Dictionary<string, object>();
            try
            {
                string TaskId = request.Id;
                string userId = User.Identity.GetUserId();
                string pp = "EmpOpenTaskDetail '" + userId + "','" + TaskId + "'";
                var EmpOpenTaskDetail = new ApplicationDbContext().MultipleResults(pp).With<EmpTaskListVM>().With<Img>().Execute();
                EmpTaskListVM vm = new EmpTaskListVM();
                if (EmpOpenTaskDetail[0] != null)
                {
                    var aa = EmpOpenTaskDetail[0];
                    foreach (dynamic item in aa)
                    {
                        vm.TaskId = item.TaskId;
                        vm.ProjectName = item.ProjectName;
                        vm.TaskName = item.TaskName;
                        vm.Createdby = item.Createdby;
                        vm.CreatedTo = item.CreatedTo;
                        vm.Status = item.Status;
                        vm.Description = item.Description;
                        vm.Createdon = item.Createdon;
                    }
                    dic.Add("result", vm);
                    dic.Add("ImagesResult", EmpOpenTaskDetail[1]);
                    dic.Add("Message", "EmpOpenTaskDetail");
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
        [Route("EmpReOpenTaskDetail")]
        public JsonResult<object> EmpReOpenTaskDetail([FromBody]JObject json)
        {
            IdModal request = JsonConvert.DeserializeObject<IdModal>(json.ToString());
            var obj = new object();
            Dictionary<string, object> dic = new Dictionary<string, object>();
            try
            {
                string TaskId = request.Id;
                string userId = User.Identity.GetUserId();
                string pp = "EmpReOpenTaskDetail '" + userId + "','" + TaskId + "'";
                var EmpReOpenTaskDetail = new ApplicationDbContext().MultipleResults(pp).With<EmpTaskListVM>().With<Img>().Execute();
                EmpTaskListVM vm = new EmpTaskListVM();
                if (EmpReOpenTaskDetail[0] != null)
                {
                    var aa = EmpReOpenTaskDetail[0];
                    foreach (dynamic item in aa)
                    {
                        vm.TaskId = item.TaskId;
                        vm.ProjectName = item.ProjectName;
                        vm.TaskName = item.TaskName;
                        vm.Createdby = item.Createdby;
                        vm.CreatedTo = item.CreatedTo;
                        vm.Status = item.Status;
                        vm.Description = item.Description;
                        vm.Createdon = item.Createdon;
                    }
                    dic.Add("result", vm);
                    dic.Add("ImagesResult", EmpReOpenTaskDetail[1]);
                    dic.Add("Message", "EmpReOpenTaskDetail");
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
        [Route("EmpTaskDetail")]
        public JsonResult<object> EmpTaskDetail([FromBody]JObject json)
        {
            IdModal request = JsonConvert.DeserializeObject<IdModal>(json.ToString());
            var obj = new object();
            Dictionary<string, object> dic = new Dictionary<string, object>();
            try
            {
                string TaskId = request.Id;
                string userId = User.Identity.GetUserId();
                string pp = "EmpTaskDetail '" + userId + "','" + TaskId + "'";
                var EmpTaskDetail = new ApplicationDbContext().MultipleResults(pp).With<EmpTaskListVM>().With<Img>().Execute();
                EmpTaskListVM vm = new EmpTaskListVM();
                if (EmpTaskDetail[0] != null)
                {
                    var aa = EmpTaskDetail[0];
                    foreach (dynamic item in aa)
                    {
                        vm.TaskId = item.TaskId;
                        vm.ProjectName = item.ProjectName;
                        vm.TaskName = item.TaskName;
                        vm.Createdby = item.Createdby;
                        vm.CreatedTo = item.CreatedTo;
                        vm.Status = item.Status;
                        vm.Description = item.Description;
                        vm.Createdon = item.Createdon;
                    }

                    dic.Add("result", vm);
                    dic.Add("ImagesResult", EmpTaskDetail[1]);
                    dic.Add("Message", "EmpTaskDetail");
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
        //========================================================
        [HttpPost]
        [Route("UserReOpenTaskList")]
        public JsonResult<object> UserReOpenTaskList([FromBody]JObject json)
        {
            PagingParameterModel re = JsonConvert.DeserializeObject<PagingParameterModel>(json.ToString());
            var obj = new object();
            List<UserViewModel> UserList = new List<UserViewModel>();
            Dictionary<string, object> dic = new Dictionary<string, object>();

            try
            {
                PMSEntities db = new PMSEntities();
                string userid = User.Identity.GetUserId();
                List<AssignDetailsViewModel> AssignTaskListData = new List<AssignDetailsViewModel>();

                //var AssignTaskList = db.AssignTasks.Where(x => x.ToAssignId == userid).ToList();
                var AssignTaskList = (from a in db.AssignTasks join t in db.tbl_Task on a.TaskId equals t.TaskId where a.ToAssignId == userid && t.Status == 2 select a).ToList();
                int count = AssignTaskList.Count();
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
                                createdby = taskdetails.Createdby,
                                Createdon = string.IsNullOrEmpty(Convert.ToString(taskdetails.Createdon)) ? "" : taskdetails.Createdon?.ToString("yyyy MMM dd"),
                                projectname = projectdetails.ProjectName,
                                status = taskdetails.Status.ToString() == "1" ? "Open" : taskdetails.Status.ToString() == "2" ? "Re-open" : "Completed",
                                AssignTouserName = touserdetails.Name == null ? "" : touserdetails.Name.Trim(),
                                AssignFromUserName = fromuserdetails.Name == null ? "" : fromuserdetails.Name.Trim()
                            });
                        }
                    }
                    // var AssignTaskList = AssignTaskListData;
                    int CurrentPage = re.pageNumber;
                    int PageSize = re._pageSize;
                    int TotalCount = count;
                    int TotalPages = (int)Math.Ceiling(count / (double)PageSize);

                    // Returns List of Customer after applying Paging   
                    var items = AssignTaskListData.Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();

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
                    var AssignTaskLists = items;
                    dic.Add("result", AssignTaskLists);
                    dic.Add("Message", "ReOpenTaskListData");
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
        [Route("UserCompletTaskList")]
        public JsonResult<object> UserCompletTaskList([FromBody]JObject json)
        {
            PagingParameterModel re = JsonConvert.DeserializeObject<PagingParameterModel>(json.ToString());
            var obj = new object();
            List<UserViewModel> UserList = new List<UserViewModel>();
            Dictionary<string, object> dic = new Dictionary<string, object>();

            try
            {
                PMSEntities db = new PMSEntities();
                string userid = User.Identity.GetUserId();
                List<AssignDetailsViewModel> AssignTaskListData = new List<AssignDetailsViewModel>();

                // var AssignTaskList = db.AssignTasks.Where(x => x.ToAssignId == userid).ToList();
                var AssignTaskList = (from a in db.AssignTasks join t in db.tbl_Task on a.TaskId equals t.TaskId where a.ToAssignId == userid && t.Status == 3 select a).ToList();
                int count = AssignTaskList.Count();
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
                                createdby = taskdetails.Createdby,
                                Createdon = string.IsNullOrEmpty(Convert.ToString(taskdetails.Createdon)) ? "" : taskdetails.Createdon?.ToString("yyyy MMM dd"),
                                projectname = projectdetails.ProjectName,
                                status = taskdetails.Status.ToString() == "1" ? "Open" : taskdetails.Status.ToString() == "2" ? "Re-open" : "Completed",
                                AssignTouserName = touserdetails.Name == null ? "" : touserdetails.Name.Trim(),
                                AssignFromUserName = fromuserdetails.Name == null ? "" : fromuserdetails.Name.Trim()
                            });
                        }
                    }
                    int CurrentPage = re.pageNumber;
                    int PageSize = re._pageSize;
                    int TotalCount = count;
                    int TotalPages = (int)Math.Ceiling(count / (double)PageSize);

                    // Returns List of Customer after applying Paging   
                    var items = AssignTaskListData.Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();

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
                    var AssignTaskLists = items;
                    dic.Add("result", AssignTaskLists);
                    dic.Add("Message", "CompleteTaskListData");
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