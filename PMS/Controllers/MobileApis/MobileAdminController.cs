using Newtonsoft.Json.Linq;
using PMS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Results;

namespace PMS_API.Controllers
{
    [RoutePrefix("api/Admin")]
    public class MobileAdminController : ApiController
    {
        private PMSEntities db = new PMSEntities();
        [HttpPost]
        [Route("UserList")]
        public JsonResult<object> AllUserList([FromBody]JObject json)
        {
       
            var obj = new object();
            Dictionary<string, object> dic = new Dictionary<string, object>();
            if (User.Identity.IsAuthenticated)
                try
                {
                    var AllUserList = (from u in db.AspNetUsers
                                       select new
                                       {
                                           u.Id,
                                           u.Name,
                                           u.Email,
                                       
                                       }).ToList();
               
                    
                    var UserAllData = AllUserList.OrderBy(x => x.Name).ToList();
                    if (UserAllData!=null)
                    {
                        dic.Add("result", UserAllData);
                        dic.Add("Message", "UseerList");
                        dic.Add("Status", "1");
                    }
                    else
                    {

                        dic.Add("Message", "No data available.");
                        dic.Add("Status", "2");
                    }


                }
                catch (Exception ex)
                {
                    dic.Add("Message", ex);
                    dic.Add("Status", "0");
                }
            obj = dic;
            return Json(obj);
        }

    }
}