using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Results;
using Microsoft.AspNet.Identity;
using System.Data.Entity;
using PMS;
using static PMS_API.Models.CommonModal;

namespace PMS_API.Controllers
{
    [RoutePrefix("api/Attandance")]
    [Authorize]
    public class MobileAttandanceController : ApiController
    {
        private PMSEntities db = new PMSEntities();
        string WorkHours;
        [HttpPost]
        [Route("All")]
        public JsonResult<object> All([FromBody]JObject json)
        {
            //PagingParameterModel re = JsonConvert.DeserializeObject<PagingParameterModel>(json.ToString());
            var obj = new object();
            //List<UserViewModel> UserList = new List<UserViewModel>();
            Dictionary<string, object> dic = new Dictionary<string, object>();
            if (User.Identity.IsAuthenticated)
            {
                string startdate = DateTime.Now.ToString("MM/dd/yyyy").Replace('-', '/');

                string TodayDate = DateTime.Now.Date.ToString();
                string start = startdate;
                string end = startdate;

                DateTime dtstart = Convert.ToDateTime(TodayDate);
                DateTime dtend = Convert.ToDateTime(TodayDate);
                List<AttendanceModel> AttandanceData = new List<AttendanceModel>();

                List<AspNetUser> Employees = db.AspNetUsers.ToList();

                var attendanceList = db.Attendances.ToList().Where(x => x.DateOfDay >= dtstart && dtend >= x.DateOfDay).ToList();
                for (int i = 0; i < attendanceList.Count; i++)
                {
                    DateTime LeaveDate = attendanceList[i].DateOfDay.Value.Date;
                    string userId = attendanceList[i].EmployeeID;
                    var leavedetails = db.Leave_Management_Leave_Taken.Where(x => x.fk_user_id == userId && x.leave_days == LeaveDate).FirstOrDefault();
                    var userDetails = db.AspNetUsers.Where(x => x.Id == userId).FirstOrDefault();


                    attendanceList[i].DateOfDay.Value.AddHours(10);

                    if (attendanceList[i].LeaveTime == null)
                    {
                        WorkHours = Convert.ToString(attendanceList[i].LeaveTime - attendanceList[i].ComingTime);
                    }
                    else
                    {
                        WorkHours = Convert.ToString(attendanceList[i].LeaveTime - attendanceList[i].ComingTime).Substring(0, 8);
                    }

                    AttandanceData.Add(new AttendanceModel
                    {

                        ID = attendanceList[i].ID,
                        EmpName = userDetails.Name,
                        comingtime = Convert.ToString(attendanceList[i].ComingTime),
                        leavetime = Convert.ToString(attendanceList[i].LeaveTime),
                        dateofday = Convert.ToString(attendanceList[i].DateOfDay),
                        EmployeeID = attendanceList[i].EmployeeID,
                        LateTime = attendanceList[i].ComingTime != null ? Convert.ToString(attendanceList[i].ComingTime - attendanceList[i].DateOfDay.Value.AddHours(10)).Substring(0, 8) : "",
                        AttandanceStatus = attendanceList[i].ComingTime == null ? (leavedetails == null ? "A" : "L") : "P",
                        WorkingHours = WorkHours,
                    });

                }

                List<SelectListItem> listDD = new List<SelectListItem>();

                foreach (var e in Employees)
                {
                    listDD.Add(new SelectListItem
                    {
                        Value = e.Id.ToString(),
                        Text = e.Name

                    });
                }

                dic.Add("Employees", listDD);
                //dic.Add("EmployeesNames", Employees);
                dic.Add("Start", start);
                dic.Add("End", end);
                dic.Add("AttandanceData", AttandanceData);
                dic.Add("Status", "1");
            }
            else
            {
                dic.Add("Message", "No Data available");
                dic.Add("Status", "2");
            }
            obj = dic;
            return Json(obj);
        }
        [HttpPost]
        [Route("EditData")]
        public JsonResult<object> EditData([FromBody]JObject json)//int? id
        {
            var obj = new object();
            Dictionary<string, object> dic = new Dictionary<string, object>();
            string id = User.Identity.GetUserId();
            if (id == null)
            {
                dic.Add("Message", HttpStatusCode.BadRequest);
                dic.Add("Status", "2");
            }
            Attendance attendance = db.Attendances.Find(id);
            if (attendance == null)
            {
                dic.Add("Message", "Data Not Found!");
                dic.Add("Status", "3");
            }
            dic.Add("attendance", attendance);
            dic.Add("Status", "3");
            obj = dic;
            return Json(obj);
        }
        [HttpPost]
        [Route("Edit")]
        public JsonResult<object> Edit([FromBody]JObject json)//Attendance attendance
        {
            Attendance re = JsonConvert.DeserializeObject<Attendance>(json.ToString());
            var obj = new object();
            Dictionary<string, object> dic = new Dictionary<string, object>();

            try
            {
                Attendance att = new Attendance { EmployeeID = re.EmployeeID, ID = re.ID, ComingTime = re.ComingTime, DateOfDay = re.DateOfDay, LeaveTime = re.LeaveTime };
                if (ModelState.IsValid)
                {

                    db.Entry(att).State = EntityState.Modified;
                    db.SaveChanges();
                }
                dic.Add("Status", "1");
                dic.Add("Message", "Successfully Updated!");

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
        [Route("Delete")]
        public JsonResult<object> Delete([FromBody]JObject json)//int? id
        {
            IdModal re = JsonConvert.DeserializeObject<IdModal>(json.ToString());
            var obj = new object();
            List<UserViewModel> UserList = new List<UserViewModel>();
            Dictionary<string, object> dic = new Dictionary<string, object>();
            int Id = Convert.ToInt32(re.Id);
            try
            {
                if (Id <= 0)
                {
                    dic.Add("Message", HttpStatusCode.BadRequest);
                    dic.Add("Status", "2");
                }
                Attendance Attendance = db.Attendances.Find(re.Id);
                db.Attendances.Remove(Attendance);
                db.SaveChanges();
                dic.Add("Message", "Data Deleted Successfully!");
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
        [Route("AttendanceTable")]
        public JsonResult<object> AttendanceTable([FromBody]JObject json)
        {
            //PagingParameterModel re = JsonConvert.DeserializeObject<PagingParameterModel>(json.ToString());
            var obj = new object();
            //List<UserViewModel> UserList = new List<UserViewModel>();
            Dictionary<string, object> dic = new Dictionary<string, object>();
                List<AttendanceModel> AttandanceData = new List<AttendanceModel>();
                //Get current month number, you can pass this value to controller ActionMethod also
                int Month = DateTime.Now.Month;
                var attendanceList = db.Attendances.ToList().Where(x => x.DateOfDay.Value.Month == Month).OrderBy(x => x.DateOfDay).ToList();

                for (int i = 0; i < attendanceList.Count; i++)
                {
                    DateTime LeaveDate = attendanceList[i].DateOfDay.Value.Date;
                    string userId = attendanceList[i].EmployeeID;
                    var leavedetails = db.Leave_Management_Leave_Taken.Where(x => x.fk_user_id == userId && x.leave_days == LeaveDate).FirstOrDefault();

                    attendanceList[i].DateOfDay.Value.AddHours(10);
                    if (attendanceList[i].LeaveTime == null)
                    {
                        WorkHours = Convert.ToString(attendanceList[i].LeaveTime - attendanceList[i].ComingTime);
                    }
                    else
                    {
                        WorkHours = Convert.ToString(attendanceList[i].LeaveTime - attendanceList[i].ComingTime).Substring(0, 8);
                    }
                    AttandanceData.Add(new AttendanceModel
                    {
                        ID = attendanceList[i].ID,
                        comingtime = Convert.ToString(attendanceList[i].ComingTime),
                        leavetime = Convert.ToString(attendanceList[i].LeaveTime),
                        dateofday = Convert.ToString(attendanceList[i].DateOfDay),
                        EmployeeID = Convert.ToString(attendanceList[i].EmployeeID),
                        LateTime = attendanceList[i].ComingTime != null ? Convert.ToString(attendanceList[i].ComingTime - attendanceList[i].DateOfDay.Value.AddHours(10)).Substring(0, 8) : "",
                        WorkingHours = WorkHours,
                        //AttandanceStatus = attendanceList[i].ComingTime == null ? "A" : "P"
                        AttandanceStatus = attendanceList[i].ComingTime == null ? (leavedetails == null ? "A" : "L") : "P"
                    });
                }
                List<AspNetUser> Employees = db.AspNetUsers.ToList();
                List<SelectListItem> listDD = new List<SelectListItem>();
                foreach (var e in Employees)
                {
                    listDD.Add(new SelectListItem
                    {
                        Value = e.Id.ToString(),
                        Text = e.Name

                    });

                }
                //Dictionary<string, object> dic1 = new Dictionary<string, object>();

                dic.Add("Employees", listDD);
                //dic.Add("EmployeesNames", Employees);
                dic.Add("AttandanceData", AttandanceData);
                dic.Add("Message", "Employee and EmpNames and AttandanceData");
                dic.Add("Status", "1");
            obj = dic;
            return Json(obj);
        }
   
        [HttpPost]
        [Route("UserAttandace")]
        public JsonResult<object> UserAttandace([FromBody]JObject json)
        {
            //PagingParameterModel re = JsonConvert.DeserializeObject<PagingParameterModel>(json.ToString());
            var obj = new object();
            //List<UserViewModel> UserList = new List<UserViewModel>();
            Dictionary<string, object> dic = new Dictionary<string, object>();

            try
            {
                string startdate = DateTime.Now.ToString("MM/dd/yyyy").Replace('-', '/');

                string TodayDate = DateTime.Now.Date.ToString();
                string start = Convert.ToDateTime(TodayDate).ToString("dd MMMM yyyy");
                string end = Convert.ToDateTime(TodayDate).ToString("dd MMMM yyyy");
                DateTime dtstart = Convert.ToDateTime(TodayDate);
                DateTime dtend = Convert.ToDateTime(TodayDate);


                var userID = User.Identity.GetUserId();
                List<AttendanceModel> AttandanceData = new List<AttendanceModel>();
                var attendanceList = db.Attendances.Where(x => x.EmployeeID == userID && x.DateOfDay >= dtstart && dtend >= x.DateOfDay).ToList();
                for (int i = 0; i < attendanceList.Count; i++)
                {
                    DateTime LeaveDate = attendanceList[i].DateOfDay.Value.Date;
                    string userId = attendanceList[i].EmployeeID;
                    var leavedetails = db.Leave_Management_Leave_Taken.Where(x => x.fk_user_id == userId && x.leave_days == LeaveDate).FirstOrDefault();
                    var userDetails = db.AspNetUsers.Where(x => x.Id == userId).FirstOrDefault();
                    attendanceList[i].DateOfDay.Value.AddHours(10);
                    if (attendanceList[i].LeaveTime == null)
                    {
                        WorkHours = Convert.ToString(attendanceList[i].LeaveTime - attendanceList[i].ComingTime);
                    }
                    else
                    {
                        WorkHours = Convert.ToString(attendanceList[i].LeaveTime - attendanceList[i].ComingTime).Substring(0, 8);
                    }
               
                    string Leavetim, comeTime;
                    if (attendanceList[i].LeaveTime!=null)
                    {
                        DateTime LeaveTime = DateTime.Parse(Convert.ToString(attendanceList[i].LeaveTime));
                        Leavetim = LeaveTime.ToString("hh:mm tt");
                    }
                    else
                    {
                        Leavetim = "NA";
                    }
                    if (attendanceList[i].ComingTime != null)
                    {
                        DateTime comeingTime = DateTime.Parse(Convert.ToString(attendanceList[i].ComingTime));
                        comeTime = comeingTime.ToString("hh:mm tt");
                    }
                    else
                    {
                        comeTime = "NA";
                    }

                    AttandanceData.Add(new AttendanceModel
                    {
                        ID = attendanceList[i].ID,
                        EmpName = userDetails.Name,
                        comingtime = comeTime,
                        leavetime = Leavetim,
                        dateofday = Convert.ToDateTime(attendanceList[i].DateOfDay).ToString("dd MMMM yyyy"),
                        EmployeeID = attendanceList[i].EmployeeID,
                        LateTime = attendanceList[i].ComingTime != null ? Convert.ToString(attendanceList[i].ComingTime - attendanceList[i].DateOfDay.Value.AddHours(10)).Substring(0, 8) : "",
                        AttandanceStatus = attendanceList[i].ComingTime == null ? (leavedetails == null ? "A" : "L") : "P",
                        WorkingHours = WorkHours,
                    });

                }
                DateTime dt = DateTime.Now;
                int days = dt.AddDays(1 - dt.Day).AddMonths(1).AddDays(-1).Day - dt.Day;
                string monthname = DateTime.Now.ToString("MMMM");
                dic.Add("RemainingDaysofMonth", days);
                dic.Add("MonthName", monthname);
                dic.Add("Status", "1");
                dic.Add("Message", "UserAttandance");
                dic.Add("Start", start);
                dic.Add("End", end);
                dic.Add("AttandanceData", AttandanceData);
            }
            catch (Exception E)
            {
                dic.Add("Message", E.Message);
                dic.Add("Status", "0");
            }
            obj = dic;
            return Json(obj);
        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("UserAttandanceByToDate")]
        public JsonResult<object> UserAttandanceByDate([FromBody]JObject json)
        {
            AttendanceFilter Att = JsonConvert.DeserializeObject<AttendanceFilter>(json.ToString());
            var obj = new object();
            Dictionary<string, object> dic = new Dictionary<string, object>();
            string start = Att.start;
            string end = Att.end;
            string WorkHours;
            try
            {
                var userID = User.Identity.GetUserId();
                List<AttendanceModel> AttandanceData = new List<AttendanceModel>();


                if (start != "" && end != "")
                {
                    DateTime dtstart = Convert.ToDateTime(start);
                    DateTime dtend = Convert.ToDateTime(end);

                    var attendanceList = db.Attendances.ToList().Where(x => x.EmployeeID == userID && x.DateOfDay >= dtstart && dtend >= x.DateOfDay).ToList();
                    for (int i = 0; i < attendanceList.Count; i++)
                    {
                        DateTime LeaveDate = attendanceList[i].DateOfDay.Value.Date;
                        string userId = attendanceList[i].EmployeeID;
                        var leavedetails = db.Leave_Management_Leave_Taken.Where(x => x.fk_user_id == userId && x.leave_days == LeaveDate).FirstOrDefault();
                        var userDetails = db.AspNetUsers.Where(x => x.Id == userId).FirstOrDefault();
                        attendanceList[i].DateOfDay.Value.AddHours(10);
                        if (attendanceList[i].LeaveTime == null)
                        {
                            WorkHours = Convert.ToString(attendanceList[i].LeaveTime - attendanceList[i].ComingTime);
                        }
                        else
                        {
                            WorkHours = Convert.ToString(attendanceList[i].LeaveTime - attendanceList[i].ComingTime).Substring(0, 8);
                        }

                     
                        string Leavetim,comeTime;
                        if (attendanceList[i].LeaveTime != null)
                        {
                            DateTime LeaveTime = DateTime.Parse(Convert.ToString(attendanceList[i].LeaveTime));
                            Leavetim = LeaveTime.ToString("hh:mm tt");
                        }
                        else
                        {
                            Leavetim ="NA";
                        }
                        if (attendanceList[i].ComingTime != null)
                        {
                            DateTime comeingTime = DateTime.Parse(Convert.ToString(attendanceList[i].ComingTime));
                            comeTime = comeingTime.ToString("hh:mm tt");
                        }
                        else
                        {
                            comeTime = "NA";
                        }
                        AttandanceData.Add(new AttendanceModel
                        {
                            ID = attendanceList[i].ID,
                            EmpName = userDetails.Name,
                            comingtime = comeTime,
                            leavetime = Leavetim,
                            dateofday = Convert.ToDateTime(attendanceList[i].DateOfDay).ToString("dd MMMM yyyy"),
                            EmployeeID = attendanceList[i].EmployeeID,
                            LateTime = attendanceList[i].ComingTime != null ? Convert.ToString(attendanceList[i].ComingTime - attendanceList[i].DateOfDay.Value.AddHours(10)).Substring(0, 8) : "",
                            AttandanceStatus = attendanceList[i].ComingTime == null ? (leavedetails == null ? "A" : "L") : "P",
                            WorkingHours = WorkHours,

                        });

                    }

                }
             
                dic.Add("AttandanceData", AttandanceData);
                dic.Add("Start", start);
                dic.Add("End", end);
                dic.Add("Message", "TaskLists");
                dic.Add("Status", "1");

            }

            catch (Exception ex)
            {
                dic.Add("Message", ex);
                dic.Add("Status", "0");
            }
            obj = dic;
            return Json(obj);
        }



        [HttpPost]
        [Route("MarkEmployeeAttandance")]
        public JsonResult<object> MarkEmployeeAttandance([FromBody]JObject json)
        {
            var obj = new object();
            var userid = User.Identity.GetUserId();
            Dictionary<string, object> dic = new Dictionary<string, object>();
            //DateTime myDateTime = Convert.ToDateTime(DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss"));

            //DateTime todayDate = Convert.ToDateTime(DateTime.Now.ToString("dd-MM-yyyy"));
            DateTime myDateTime = DateTime.Now;

            DateTime todayDate = DateTime.Now.Date; ;
            DateTime CheckTime = DateTime.Now.Date.AddHours(12);
            bool AttendanceValid = db.Attendances.Any(c => c.DateOfDay == todayDate && c.EmployeeID == userid);

            if (AttendanceValid)
            {
                var attendanceRow = db.Attendances.Where(c => c.DateOfDay == todayDate && c.EmployeeID == userid).Single();
                dic.Add("Status", "1");
                dic.Add("Message", "Result");
                dic.Add("iscoming", "true");
                dic.Add("isLeave", "true");
                attendanceRow.LeaveTime = myDateTime;
                db.Entry(attendanceRow).State = EntityState.Modified;
                db.SaveChanges();
                obj = dic;
                return Json(obj);

            }
            else
            {
                DateTime now = DateTime.Now;
                Attendance Attendance = new Attendance
                {
                    EmployeeID = userid,
                    ComingTime = myDateTime,
                    // DateOfDay = Convert.ToDateTime(DateTime.Now.ToString("dd-MM-yyyy")),
                    DateOfDay = DateTime.Now.Date,

                };

                db.Attendances.Add(Attendance);
                db.SaveChanges();
                dic.Add("isLeave", "false");
                dic.Add("iscoming", "true");
                dic.Add("Status", "1");
                if (myDateTime > CheckTime)
                {
                    var leavedetails = db.Leave_Management_Leave_Taken.Where(x => x.fk_user_id == userid && x.leave_days == todayDate).FirstOrDefault();
                    if (leavedetails == null)
                    {
                        DateTime d = todayDate;

                        Leave_Management_Leave_Taken lt = new Leave_Management_Leave_Taken();
                        lt.fk_leave_type = 4;
                        lt.leave_days = d;
                        lt.fk_user_id = userid;
                        lt.leave_status = 1;
                        lt.is_halfday = true;
                        lt.is_afternoon = false;
                        db.Leave_Management_Leave_Taken.Add(lt);
                        db.SaveChanges();
                    }
                }
                obj = dic;
                return Json(obj);
            }
        }

        [HttpPost]
        [Route("SearchAttandanceByUser")]
        public JsonResult<object> SearchAttandanceByUser([FromBody]JObject json)
        {
            AttendanceFilter Att = JsonConvert.DeserializeObject<AttendanceFilter>(json.ToString());
            var obj = new object();
            Dictionary<string, object> dic = new Dictionary<string, object>();
            string Employee = Att.Employee;
            string start = Att.start;
            string end = Att.end;
            string WorkHours;
            try
            {
                List<AttendanceModel> AttandanceData = new List<AttendanceModel>();


                if (start != "" && end != "" && Employee != "")
                {


                    DateTime dtstart = Convert.ToDateTime(start);
                    DateTime dtend = Convert.ToDateTime(end);

                    var attendanceList = db.Attendances.ToList().Where(x => x.EmployeeID == Employee && x.DateOfDay >= dtstart && dtend >= x.DateOfDay).ToList();
                    for (int i = 0; i < attendanceList.Count; i++)
                    {
                        DateTime LeaveDate = attendanceList[i].DateOfDay.Value.Date;
                        string userId = attendanceList[i].EmployeeID;
                        var leavedetails = db.Leave_Management_Leave_Taken.Where(x => x.fk_user_id == userId && x.leave_days == LeaveDate).FirstOrDefault();
                        var userDetails = db.AspNetUsers.Where(x => x.Id == userId).FirstOrDefault();
                        attendanceList[i].DateOfDay.Value.AddHours(10);
                        if (attendanceList[i].LeaveTime == null)
                        {
                            WorkHours = Convert.ToString(attendanceList[i].LeaveTime - attendanceList[i].ComingTime);
                        }
                        else
                        {
                            WorkHours = Convert.ToString(attendanceList[i].LeaveTime - attendanceList[i].ComingTime).Substring(0, 8);
                        }
                        string Leavetim, comeTime;
                        if (attendanceList[i].LeaveTime != null)
                        {
                            DateTime LeaveTime = DateTime.Parse(Convert.ToString(attendanceList[i].LeaveTime));
                            Leavetim = LeaveTime.ToString("hh:mm tt");
                        }
                        else
                        {
                            Leavetim = "NA";
                        }
                        if (attendanceList[i].ComingTime != null)
                        {
                            DateTime comeingTime = DateTime.Parse(Convert.ToString(attendanceList[i].ComingTime));
                            comeTime = comeingTime.ToString("hh:mm tt");
                        }
                        else
                        {
                            comeTime = "NA";
                        }
                        AttandanceData.Add(new AttendanceModel
                        {
                            ID = attendanceList[i].ID,
                            EmpName = userDetails.Name,
                            comingtime = comeTime,
                            leavetime = Leavetim,
                            dateofday = Convert.ToDateTime(attendanceList[i].DateOfDay).ToString("dd MMMM yyyy"),
                            EmployeeID = attendanceList[i].EmployeeID,
                            LateTime = attendanceList[i].ComingTime != null ? Convert.ToString(attendanceList[i].ComingTime - attendanceList[i].DateOfDay.Value.AddHours(10)).Substring(0, 8) : "",
                            AttandanceStatus = attendanceList[i].ComingTime == null ? (leavedetails == null ? "A" : "L") : "P",
                            WorkingHours = WorkHours,

                        });

                    }

                }
       

                List<AspNetUser> Employees = db.AspNetUsers.ToList();

                List<SelectListItem> listDD = new List<SelectListItem>();
                foreach (var e in Employees)
                {
                    listDD.Add(new SelectListItem
                    {
                        Value = e.Id.ToString(),
                        Text = e.Name

                    });

                }
                DateTime dt = DateTime.Now;
                int days = dt.AddDays(1 - dt.Day).AddMonths(1).AddDays(-1).Day - dt.Day;
                string monthname = DateTime.Now.ToString("MMMM");
                dic.Add("RemainingDaysofMonth", days);
                dic.Add("MonthName", monthname);
                dic.Add("result", AttandanceData);
                dic.Add("Start", start);
                dic.Add("End", end);
                dic.Add("Message", "TaskLists");
                dic.Add("Status", "1");

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