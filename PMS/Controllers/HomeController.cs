using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMS.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Dashboard", "WebDevloper");
            }
            else
            {

                Session["displayMenu"] = "";
                ViewBag.ReturnUrl = returnUrl;
                return View();
            }
        }
        // GET: /Home/Register
        [AllowAnonymous]
        public ActionResult Register(string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Dashboard", "WebDevloper");
            }
            else
            {
                Session["displayMenu"] = "";
                ViewBag.ReturnUrl = returnUrl;
                return View();
            }
        }
        // GET: /Home/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code,string username)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Dashboard", "WebDevloper");
            }
            else
            {
                return View();
            }
        }
    }
}
