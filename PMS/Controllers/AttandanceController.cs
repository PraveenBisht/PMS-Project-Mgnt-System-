using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PMS.Models;
using System.Web.UI.WebControls;
using System.IO;
using System.Web.UI;

namespace PMS.Controllers
{
    public class AttandanceController : Controller
    {

        private PMSEntities db = new PMSEntities();


        string WorkHours;
        public ActionResult All()
        {
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
                    var userDetails = db.AspNetUsers.Where(x => x.Id == userId ).FirstOrDefault();


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
                    }) ;

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

                TempData["Employees"] = listDD;
                TempData["EmployeesNames"] = Employees;
                TempData["Start"] = start;
                TempData["End"] = end;
                return View(AttandanceData);
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

    

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Attendance attendance = db.Attendances.Find(id);
            if (attendance == null)
            {
                return HttpNotFound();
            }
            return View(attendance);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Attendance attendance)
        {
            if (ModelState.IsValid)
            {
                db.Entry(attendance).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("All");
            }
            return View(attendance);
        }


        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Attendance Attendance = db.Attendances.Find(id);
            if (Attendance == null)
            {
                return HttpNotFound();
            }
            return View(Attendance);
        }

        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Attendance Attendance = db.Attendances.Find(id);
            db.Attendances.Remove(Attendance);
            db.SaveChanges();
            return RedirectToAction("All");
        }
        public ActionResult AttendanceTable()
        {
            if (User.Identity.IsAuthenticated)
            {
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
                        ComingTime = attendanceList[i].ComingTime,
                        LeaveTime = attendanceList[i].LeaveTime,
                        DateOfDay = attendanceList[i].DateOfDay,
                        EmployeeID = attendanceList[i].EmployeeID,
                        LateTime = attendanceList[i].ComingTime != null ? Convert.ToString(attendanceList[i].ComingTime - attendanceList[i].DateOfDay.Value.AddHours(10)).Substring(0, 8) : "",
                        WorkingHours = WorkHours,
                        AttandanceStatus = attendanceList[i].ComingTime == null ? "A" : "P"
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
            TempData["Employees"] = listDD;
            TempData["EmployeesNames"] = Employees;
            //ViewBag["selectedmonth"] = DateTime.Now.Month.ToString();
            return View(AttandanceData);
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }

        }
        [HttpPost]
        public ActionResult AttendanceTable(string MonthName)
        {
            List<AttendanceModel> AttandanceData = new List<AttendanceModel>();

            //Get current month number, you can pass this value to controller ActionMethod also
            int Month = Convert.ToInt32(MonthName);
            var attendanceList = db.Attendances.ToList().Where(x => x.DateOfDay.Value.Month == Month).OrderBy(x => x.DateOfDay).ToList();

            for (int i = 0; i < attendanceList.Count; i++)
            {
                DateTime LeaveDate = attendanceList[i].DateOfDay.Value.Date;
                string userId = attendanceList[i].EmployeeID;
                var leavedetails = db.Leave_Management_Leave_Taken.Where(x => x.fk_user_id == userId && x.leave_days == LeaveDate).FirstOrDefault();

                attendanceList[i].DateOfDay.Value.AddHours(10);

                AttandanceData.Add(new AttendanceModel
                {
                    ID = attendanceList[i].ID,
                    ComingTime = attendanceList[i].ComingTime,
                    LeaveTime = attendanceList[i].LeaveTime,
                    DateOfDay = attendanceList[i].DateOfDay,
                    EmployeeID = attendanceList[i].EmployeeID,
                    LateTime = attendanceList[i].ComingTime != null ? Convert.ToString(attendanceList[i].ComingTime - attendanceList[i].DateOfDay.Value.AddHours(10)).Substring(0, 8) : "",
                    WorkingHours = WorkHours,
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
            TempData["Employees"] = listDD;
            TempData["EmployeesNames"] = Employees;
           // ViewBag["selectedmonth"] = MonthName;
            return View(AttandanceData);

        }

        public ActionResult UserAttandace()
        {
            string startdate = DateTime.Now.ToString("MM/dd/yyyy").Replace('-', '/');

            string TodayDate = DateTime.Now.Date.ToString();
            string start = startdate;
            string end = startdate;

            DateTime dtstart = Convert.ToDateTime(TodayDate);
            DateTime dtend = Convert.ToDateTime(TodayDate);

            if (User.Identity.IsAuthenticated)
            {

                var userID = User.Identity.GetUserId();
            List<AttendanceModel> AttandanceData = new List<AttendanceModel>();

            List<AspNetUser> Employees = db.AspNetUsers.ToList();

            var attendanceList = db.Attendances.Where(x=>x.EmployeeID== userID && x.DateOfDay >= dtstart && dtend >= x.DateOfDay).ToList();
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

                TempData["EmployeesNames"] = Employees;
                TempData["Start"] = start;
                TempData["End"] = end;

                return View(AttandanceData);
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }
        public ActionResult ExportToExcel(string stratdate, string enddate,string employeeId)
        {
            string Employee = employeeId;
            string start = stratdate;
            string end = enddate;
            string WorkHours;
            List<AttendanceExportViewModel> AttandanceData = new List<AttendanceExportViewModel>();

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
                        // WorkHours = Convert.ToString(attendanceList[i].LeaveTime - attendanceList[i].ComingTime);
                        WorkHours = "NA";
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
                    AttandanceData.Add(new AttendanceExportViewModel
                    {
                      
                        Name = userDetails.Name,
                        ComingTime = comeTime,
                        LeaveTime = Leavetim,
                        Date = Convert.ToDateTime(attendanceList[i].DateOfDay).ToString("dd MMMM yyyy"),
                        LateTime = attendanceList[i].ComingTime != null ? Convert.ToString(attendanceList[i].ComingTime - attendanceList[i].DateOfDay.Value.AddHours(10)).Substring(0, 8) : "",
                        AttandanceStatus = attendanceList[i].ComingTime == null ? (leavedetails == null ? "A" : "L") : "P",
                        WorkingHours = WorkHours,

                    });

                }

            }
            else if (start != "" && end != "" && Employee == "")
            {


                DateTime dtstart = Convert.ToDateTime(start);
                DateTime dtend = Convert.ToDateTime(end);
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
                    AttandanceData.Add(new AttendanceExportViewModel
                    {
                     
                        Name = userDetails.Name,
                        ComingTime = comeTime,
                        LeaveTime = Leavetim,
                        Date = Convert.ToDateTime(attendanceList[i].DateOfDay).ToString("dd MMMM yyyy"),
                        LateTime = attendanceList[i].ComingTime != null ? Convert.ToString(attendanceList[i].ComingTime - attendanceList[i].DateOfDay.Value.AddHours(10)).Substring(0, 8) : "",
                        AttandanceStatus = attendanceList[i].ComingTime == null ? (leavedetails == null ? "A" : "L") : "P",
                        WorkingHours = WorkHours,
                    });

                }

            }

            GridView gv = new GridView();
            gv.DataSource = AttandanceData;
            gv.DataBind();
            Response.ClearContent();
            Response.Buffer = true;
            string strDateFormat = string.Empty;
            strDateFormat = string.Format("{0:yyyy-MMM-dd-hh-mm-ss}", DateTime.Now);
            Response.AddHeader("content-disposition", "attachment; filename=UserAttandanceDetails_" + strDateFormat + ".xls");
            Response.ContentType = "application/ms-excel";
            Response.Charset = "";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            gv.RenderControl(htw);
            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();
            return RedirectToAction("Attandance", "ExportToExcel");
        }
    }
}