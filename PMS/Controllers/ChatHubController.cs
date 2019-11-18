using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMS.Controllers
{
    public class ChatHubController : Controller
    {
        PMSEntities db = new PMSEntities();
        // GET: ChatHub

        public ActionResult ChatPrivate()
        {
            if (User.Identity.IsAuthenticated)
            {
                var userid = User.Identity.GetUserId();
                var userdetails = db.AspNetUsers.Where(x => x.Id == userid).FirstOrDefault();
                ViewBag.EmailId = userdetails.Email;
                ViewBag.Name = userdetails.Name;
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }

        }
    }
}