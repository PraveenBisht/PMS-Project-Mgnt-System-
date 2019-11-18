using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PMS.Models;
namespace PMS.Controllers
{
    public class ProjectController : Controller
    {
        PMSEntities _context = new PMSEntities();
       

        // GET: Project/Create
        public ActionResult AddProject()
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

        // POST: Project/Create
        [HttpPost]
        public ActionResult AddProject(ProjectViewModel ProjectMaster)
        {
            try
            {
                ProjectMaster projectdata = new ProjectMaster();
                projectdata.ProjectName = ProjectMaster.ProjectName;
                projectdata.Projectdescription = ProjectMaster.Projectdescription;
                projectdata.status =true;
                projectdata.createdDate= DateTime.Now;
                _context.ProjectMasters.Add(projectdata);
                 _context.SaveChanges();
                return View();
            }
            catch (Exception)
            {
                throw;
            }
        }

        // GET: Project/Create
        public ActionResult ProjectList()
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
        public ActionResult ProjectListData()
        {
            if (User.Identity.IsAuthenticated)
            {
                var obj = new object();
                Dictionary<string, object> dic = new Dictionary<string, object>();
                try
                {
                    var projectlist = (from j in _context.ProjectMasters

                                       select new ProjectViewModel
                                       {
                                           ProjectID = j.ProjectID,
                                           ProjectName = j.ProjectName,
                                           Projectdescription = j.Projectdescription,
                                           createDate = j.createdDate.ToString()

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

     
        // GET: Project/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Project/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        public bool CheckProjectCodeExists(string ProjectCode)
        {
            try
            {
              
                    var result = (from user in _context.ProjectMasters
                                  select user).Count();

                    if (result > 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool CheckProjectNameExists(string ProjectName)
        {
            try
            {
              
                    var result = (from user in _context.ProjectMasters
                                  where user.ProjectName == ProjectName
                                  select user).Count();

                    if (result > 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<ProjectMaster> GetListofProjects()
        {
            try
            {
                  var listofProjects = (from project in _context.ProjectMasters
                                        select project).ToList();
                    return listofProjects;
                
            }
            catch (Exception)
            {
                throw;
            }
        }

     

    }
}
