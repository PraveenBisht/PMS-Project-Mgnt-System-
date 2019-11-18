using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PMS;
using PMS.Models;
using PMS_API.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Results;
using static PMS_API.Models.CommonModal;

namespace PMS.Controllers.MobileApis
{
    [Authorize]
    [RoutePrefix("api/Common")] 
    public class CommonController : ApiController
    {
        PMSEntities _context = new PMSEntities();

        [HttpGet]
        [Route("ProjectList")]
        public JsonResult<object> ProjectList()
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
        //[HttpGet]
        //[Route("EmployeeList")]
        //public JsonResult<object> EmployeeList()
        //{
        //    var obj = new object();
        //    List<EmployeeModal> UserList = new List<EmployeeModal>();
        //    Dictionary<string, object> dic = new Dictionary<string, object>();

        //    if (User.Identity.IsAuthenticated)

        //        try
        //        {
        //            var Employeelist = (from j in _context.AspNetUsers

        //                               select new EmployeeModal
        //                               {
        //                                   id = j.Id,
        //                                   // ProjectCode = j.Projectcode,
        //                                   Name = j.Name

        //                               }).ToList();
        //            if (Employeelist.Count > 0)
        //            {
        //                dic.Add("result", Employeelist);
        //                dic.Add("Message", "Employeelist");
        //                dic.Add("Status", "1");
        //            }
        //            else
        //            {

        //                dic.Add("Message", "No data available.");
        //                dic.Add("Status", "2");
        //            }


        //        }
        //        catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
        //        {
        //            Exception raise = dbEx;
        //            string message;
        //            foreach (var validationErrors in dbEx.EntityValidationErrors)
        //            {
        //                foreach (var validationError in validationErrors.ValidationErrors)
        //                {
        //                    message = string.Format("{0}:{1}",
        //                       validationErrors.Entry.Entity.ToString(),
        //                       validationError.ErrorMessage);
        //                    raise = new InvalidOperationException(message, raise);
        //                }

        //            }
        //            dic.Add("Message", raise);
        //            dic.Add("Status", "0");

        //        }
        //    obj = dic;
        //    return Json(obj);
        //}

        [HttpPost]
        [Route("EmployeeList")]
        public JsonResult<object> EmployeeList([FromBody]JObject json)
        {
            string RoleName;
            //paging request = JsonConvert.DeserializeObject<paging>(json.ToString());
            var obj = new object();
            Dictionary<string, object> dic = new Dictionary<string, object>();

            string userId = User.Identity.GetUserId();
            ApplicationDbContext context = new ApplicationDbContext();
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            var s = UserManager.GetRoles(userId);
            RoleName = s[0].ToString();
            try
            {
                SqlParameter param1 = new SqlParameter("@EmpId", userId);
                SqlParameter param2 = new SqlParameter("@Role", RoleName);
                var ddlEmployeeList = context.Database.SqlQuery<SelectListItem1>("EmpListForDDL @EmpId, @Role", param1, param2).ToList();
                if (ddlEmployeeList != null && ddlEmployeeList.Count > 0)
                {
                    dic.Add("result", ddlEmployeeList);
                    dic.Add("Message", "EmployeeList");
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

    }
}