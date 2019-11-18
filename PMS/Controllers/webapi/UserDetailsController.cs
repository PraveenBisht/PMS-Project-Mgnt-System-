using PMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace PMS.Controllers.webapi
{
    public class UserDetailsController : ApiController
    {
        // GET api/<controller>
        public IEnumerable<object> Get()
        {
            PMSEntities DbLMS = new PMSEntities();
            var UserDetails = DbLMS.Innovation_Management_User.Select(x => new {x.user_firstname,x.user_lastname,x.user_picture,x.user_email,x.pk_user_id });
            return UserDetails;
        }
      
    }
}