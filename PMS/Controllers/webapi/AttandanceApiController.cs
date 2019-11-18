using Microsoft.AspNet.Identity;
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
using System.Web.Mvc;

namespace PMS.Controllers.webapi
{
    [System.Web.Http.RoutePrefix("api/EmployeeAttandance")]
    public class AttandanceApiController : ApiController
    {
        // GET: api/AttandanceApi
        PMSEntities db = new PMSEntities();

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("SearchAttandanceByUser")]
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


                if (start != "" && end != ""&& Employee!="")
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


        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("UserAttandanceByDate")]
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
