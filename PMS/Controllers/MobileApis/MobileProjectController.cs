using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PMS;
using PMS.Models;
using PMS_API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Results;

namespace PMS_API.Controllers
{
    [Authorize]
    [RoutePrefix("api/Project")]
    public class MobileProjectController : ApiController
    {
        PMSEntities _context = new PMSEntities();
        [HttpPost]
        [Route("AddProject")] 
        public JsonResult<object> AddProject([FromBody]JObject json) //ProjectMaster ProjectMaster
        {
            ProjectMaster re = JsonConvert.DeserializeObject<ProjectMaster>(json.ToString());
            var obj = new object();
            Dictionary<string, object> dic = new Dictionary<string, object>();
            ProjectMaster pm = new ProjectMaster { ProjectName= re.ProjectName };
            try
            {
                _context.ProjectMasters.Add(pm);
                _context.SaveChanges();
                dic.Add("Message", "Successfully Submitted.");
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
        [Route("ProjectListData")]
        public JsonResult<object> ProjectListData([FromBody]JObject json)
        {
            //PagingParameterModel re = JsonConvert.DeserializeObject<PagingParameterModel>(json.ToString());
            var obj = new object();
            List<UserViewModel> UserList = new List<UserViewModel>();
            Dictionary<string, object> dic = new Dictionary<string, object>();

            if (User.Identity.IsAuthenticated)
            
                try
                {
                    var projectlist = (from j in _context.ProjectMasters

                                       select new ProjectViewModel
                                       {
                                           ProjectID = j.ProjectID,
                                          // ProjectCode = j.Projectcode,
                                           ProjectName = j.ProjectName

                                       }).ToList();
                    if (projectlist.Count > 0)
                    {
                        dic.Add("result", projectlist);
                        dic.Add("Message", "projectlist");
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
                            raise = new InvalidOperationException(message, raise);
                        }

                    }
                    dic.Add("Message", raise);
                    dic.Add("Status", "0");

                }
                obj = dic;
                return Json(obj);
            }
            //else
            //{
            //    return RedirectToAction("Login", "Account");
            //}

        [HttpPost]
        [Route("CheckProjectCodeExists")]
        public JsonResult<object> CheckProjectCodeExists([FromBody]JObject json)//string ProjectCode
        {
            var obj = new object();
            ProjectMaster re = JsonConvert.DeserializeObject<ProjectMaster>(json.ToString());
       
            Dictionary<string, object> dic = new Dictionary<string, object>();

            try
            {

                var result = (from user in _context.ProjectMasters
                         
                              select user).Count();

                if (result > 0)
                {
                    dic.Add("result", "true");
                    dic.Add("Message", "listofProjects");
                    dic.Add("Status", "1");
                }
                else
                {
                    dic.Add("Message", "false");
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
        [Route("CheckProjectNameExists")]
        public JsonResult<object> CheckProjectNameExists([FromBody]JObject json)//string ProjectName
        {
            var obj = new object();
            ProjectMaster re = JsonConvert.DeserializeObject<ProjectMaster>(json.ToString());
            string ProjectName = re.ProjectName;
            Dictionary<string, object> dic = new Dictionary<string, object>();
            try
            {
                int result = (from user in _context.ProjectMasters
                              where user.ProjectName == ProjectName
                              select user).Count();
                if (result > 0)
                {
                    dic.Add("result", "true");
                    dic.Add("Message", "listofProjects");
                    dic.Add("Status", "1");
                }
                else
                {
                    dic.Add("Message", "false");
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
        [Route("GetListofProjects")]
        public JsonResult<object> GetListofProjects([FromBody]JObject json)
        {
            PagingParameterModel re = JsonConvert.DeserializeObject<PagingParameterModel>(json.ToString());
            var obj = new object();
            List<UserViewModel> UserList = new List<UserViewModel>();
            Dictionary<string, object> dic = new Dictionary<string, object>();

            try
            {
                var listofProjects = (from project in _context.ProjectMasters
                                      select project).ToList();
                int count = listofProjects.Count();
                if (count > 0)
                {
                    int CurrentPage = re.pageNumber;
                    int PageSize = re._pageSize;
                    int TotalCount = count;
                    int TotalPages = (int)Math.Ceiling(count / (double)PageSize);

                    // Returns List of Customer after applying Paging   
                    var items = listofProjects.Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();

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
                    dic.Add("result", listofProjects);
                    dic.Add("Message", "listofProjects");
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