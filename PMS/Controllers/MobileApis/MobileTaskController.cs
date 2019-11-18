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
using static PMS_API.Models.CommonModal;
using System.IO;
using System.Web;
using System.Net.NetworkInformation;
using PMS;
using System.Drawing;
using System.Data.Entity.Infrastructure;
using PMS.Models.MobileApisModels;
using System.Data.SqlClient;
using System.Data.Entity;

namespace PMS_API.Controllers
{
    [RoutePrefix("api/MobileTask")]
    [Authorize]
    public class MobileTaskController : ApiController
    { //UserAssignTask

        PMSEntities db = new PMSEntities();
        ApplicationDbContext db1 = new ApplicationDbContext();

     
        [HttpPost]
        [Route("CreateTask")]
        public JsonResult<object> CreateTask([FromBody]JObject json)//TaskViewModel task, List<HttpPostedFileBase> postedFiles)
        {
            var obj = new object();
            Dictionary<string, object> dic = new Dictionary<string, object>();
            try
            {
                RootObject re = JsonConvert.DeserializeObject<RootObject>(json.ToString());
                tbl_Task taskdetails = new tbl_Task();
                string taskid = Guid.NewGuid().ToString();
                taskdetails.TaskId = taskid;
                taskdetails.SummaryName = re.details.TaskName;
                taskdetails.Description = re.details.description;
                taskdetails.Status = 1;
                taskdetails.Createdby = User.Identity.GetUserId();// User.Identity.Name; ;// "Admin";
                taskdetails.CreatedTo = re.details.CreatedTo;
                taskdetails.Createdon = Convert.ToDateTime(re.details.Createdon);
                taskdetails.devicetype = re.details.Devicetype;// "Mobile";
                taskdetails.projectid = Convert.ToInt32(re.details.projectid);
                db.tbl_Task.Add(taskdetails);
                db.SaveChanges();
                AssignTask assigntaskdata = new AssignTask();
                assigntaskdata.TaskId = taskid;
                assigntaskdata.CreatedOn = Convert.ToDateTime(re.details.Createdon);
                assigntaskdata.FromassignId = User.Identity.GetUserId(); 
                assigntaskdata.ToAssignId = re.details.CreatedTo;
                db.AssignTasks.Add(assigntaskdata);
                db.SaveChanges();
                foreach (PostedFile postedFile in re.postedFiles)
                {
                    Filedetail filedetails = new Filedetail();
                    filedetails.TaskId = taskid;
                    filedetails.imageName = postedFile.file;
                    filedetails.Status = 1;//i thik pending 
                    db.Filedetails.Add(filedetails);
                    db.SaveChanges();
                }
                dic.Add("Message", "Task Created Successfully.");
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
        [Route("InsertImage")]
        public JsonResult<object> InsertImage([FromBody]JObject json)
        {
            PicVM re = JsonConvert.DeserializeObject<PicVM>(json.ToString());
            var obj = new object();
            Dictionary<string, object> dic = new Dictionary<string, object>();
            string Image1 = re.ProfilePic;
            string ImageName = Guid.NewGuid().ToString();
            string ImageExt = re.PicExtention;
            string SavePath1 = "";
            try
            {
                if (!string.IsNullOrWhiteSpace(Image1))
                {
                    var postedFile = Image1;
                    if (!Directory.Exists(HttpContext.Current.Server.MapPath("~/Document/Upload/")))
                    {
                        Directory.CreateDirectory(HttpContext.Current.Server.MapPath("~/Document/Upload/"));
                    }
                    string ImagePath1 = HttpContext.Current.Server.MapPath("~/Document/Upload/") + ImageName + "." + ImageExt;
                    SavePath1 = "/Document/Upload/" + ImageName + "." + ImageExt;
                    if (File.Exists(ImagePath1))
                    {
                        File.Delete(ImagePath1);
                    }
                    byte[] Image1D = Convert.FromBase64String(Image1.ToString());

                    Stream stmImage1D = new MemoryStream(Image1D);
                    Bitmap originalBMP1 = new Bitmap(stmImage1D);
                    originalBMP1.Save(ImagePath1);
                    dic.Add("Status", "1");
                    dic.Add("Message", "Inserted Successfully.");
                    dic.Add("ImgPath", SavePath1);
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
        [Route("DeleteImage")]
        public JsonResult<object> DeleteImage([FromBody]JObject json)
        {
            PicVM re = JsonConvert.DeserializeObject<PicVM>(json.ToString());
            var obj = new object();
            Dictionary<string, object> dic = new Dictionary<string, object>();
            var ImagePath1 = System.Web.Hosting.HostingEnvironment.MapPath("~" + re.ImagePath);
            try
            {
                if (!string.IsNullOrWhiteSpace(ImagePath1))
                {
                    if (File.Exists(ImagePath1))
                    {
                        File.Delete(ImagePath1);
                        dic.Add("Status", "1");
                        dic.Add("Message", "Deleted Successfully.");
                    }
                    else
                    {
                        dic.Add("Status", "2");
                        dic.Add("Message", "Image Not Found.");
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
        [Route("ReassignTask")]
        public JsonResult<object> ReassignTask([FromBody]JObject json)
        {
            var obj = new object();
            Dictionary<string, object> dic = new Dictionary<string, object>();

            try
            {
                RootObject re = JsonConvert.DeserializeObject<RootObject>(json.ToString());
                AssignTask taskdetails = new AssignTask();
                string UserId = User.Identity.GetUserId();
                string taskid = re.details.TaskId;
                var taskdata= db.tbl_Task.Where(x => x.TaskId == taskid).FirstOrDefault();
                taskdetails.TaskId = taskid;
                taskdata.Createdby = UserId;
                taskdata.CreatedTo= re.details.CreatedTo;
                taskdata.Description = re.details.description;
                taskdata.Createdon = Convert.ToDateTime(re.details.Createdon);
                db.Entry(taskdata).State = EntityState.Modified;
                db.SaveChanges();
                var Assigntaskdata = db.AssignTasks.Where(x => x.TaskId == taskid).FirstOrDefault();
                Assigntaskdata.FromassignId = UserId;
                Assigntaskdata.ToAssignId = re.details.CreatedTo;
                db.AssignTasks.Add(taskdetails);
                db.SaveChanges();

                foreach (PostedFile postedFile in re.postedFiles)
                {
                    Filedetail filedetails = new Filedetail();
                    filedetails.TaskId = taskid;
                    filedetails.imageName = postedFile.file;
                    filedetails.Status = 1;//i thik pending 
                    db.Filedetails.Add(filedetails);
                    db.SaveChanges();
                }
                dic.Add("Message", "Reassign Task Successfully.");
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
        [Route("InsComment")]
        public JsonResult<object> InsComment([FromBody]JObject json)
        {
            Comment re = JsonConvert.DeserializeObject<Comment>(json.ToString());
            var obj = new object();
            Dictionary<string, object> dic = new Dictionary<string, object>();
            string UserId = User.Identity.GetUserId();
            Comment cmt = new Comment();
            try
            {
                SqlParameter TaskId = new SqlParameter("@TaskId", re.TaskId);
                //SqlParameter FromassignId = new SqlParameter("@FromassignId", re.FromassignId); 
                SqlParameter ToAssignId = new SqlParameter("@ToAssignId", UserId);
                SqlParameter Status = new SqlParameter("@Status", Convert.ToInt32(re.Status));
                SqlParameter Description = new SqlParameter("@Description", re.Description);
                SqlParameter CreatedOn = new SqlParameter("@CreatedOn", re.CreatedOn);
                ApplicationDbContext context = new ApplicationDbContext();
                var Result = context.Database.SqlQuery<List<string>>("uspComment @TaskId,@ToAssignId,@Status,@Description,@CreatedOn", TaskId, ToAssignId, Status, Description, CreatedOn).ToList();
                if (Result != null)
                {
                    dic.Add("Message", "Data Inserted Successfully.");
                    dic.Add("Status", "1");
                }
                else
                {
                    dic.Add("Message", "Unable to Insert the Data. Please Try Again.");
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