using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Results;
using Newtonsoft.Json.Linq;
using PMS.Models;

namespace PMS.Controllers.webapi
{
  
    [RoutePrefix("api/ChatUser")]
    public class ChatApiController : ApiController
    {
        private PMSEntities db = new PMSEntities();
        [HttpPost]
        public HttpResponseMessage SendNotification(NotifModels obj)
        {
            ChatHub objNotifHub = new ChatHub();
            Notification objNotif = new Notification();
            objNotif.SentTo = obj.UserID;

            db.Configuration.ProxyCreationEnabled = false;
            db.Notifications.Add(objNotif);
            db.SaveChanges();

            objNotifHub.SendNotification(objNotif.SentTo);

            var query = (from t in db.Notifications
                         select t).ToList();

            return Request.CreateResponse(HttpStatusCode.OK, new { query });
        }

        [HttpGet]
        [Route("NotificationByUser")]
        public JsonResult<object> SendNotificationToAndroid()
        {
            var obj = new object();
            Dictionary<string, object> dic = new Dictionary<string, object>();
            if (User.Identity.IsAuthenticated)
                try
                {
                    string UserEmailId = User.Identity.Name;
                    var Notification = (from t in db.ChatPrivateMessageDetails join u in db.AspNetUsers  on t.ChatToEmailID equals u.UserName where t.ChatToEmailID == UserEmailId && t.IsRead == false  orderby t.ChatDateTime descending
                               select new NotificationModels
                                 {
                                   Message=t.Message,
                                   FromEmailID=t.MasterEmailID,
                                   ProfilePic=u.ProfilePic==null?"NA": u.ProfilePic,
                                   UserEmailID=t.ChatToEmailID,
                               }).FirstOrDefault();

                 
                    if (Notification!=null)
                    {
                        var FromUserDetails = db.AspNetUsers.Where(x => x.UserName == Notification.FromEmailID).FirstOrDefault();
                        string LastMassage = Notification.Message;
                        string Name = FromUserDetails.Name;
                        string FromEmailID = Notification.FromEmailID;
                        string ProfilePic = Notification.ProfilePic;
                        string designation = FromUserDetails.Designation== null ? "NA" : FromUserDetails.Designation;
                        //dic.Add("result", Notification);
                        dic.Add("SendToEmailID", FromEmailID);
                        dic.Add("Profilepic", ProfilePic);
                        dic.Add("Designation", designation);
                        dic.Add("message", LastMassage);
                        dic.Add("Name", Name);
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

        [HttpPost]
        [Route("AllUserList")]
        public JsonResult<object> AllUserList([FromBody]JObject json)
        {
            string onlineStatus,LastMassage;
            DateTime ChatDatetime;
            int incomingMessagecount;
            var obj = new object();
            List<UserChatViewModel> UserData = new List<UserChatViewModel>();
            Dictionary<string, object> dic = new Dictionary<string, object>();
            if (User.Identity.IsAuthenticated)
                try
                {
                    var AllUserList = (from u in db.AspNetUsers
                                       select new
                                       {
                                           u.Id,
                                           u.Last_Online_Seen,
                                           u.Name,
                                           u.ProfilePic,
                                           u.UserName,
                                           u.Email,
                                           u.Designation
                                       }).ToList();
                    for (int i = 0; i < AllUserList.Count; i++)
                    {
                        string EmailId = AllUserList[i].UserName;


                        var lastmassageByUser = (from values in db.ChatPrivateMessageDetails
                                                 where (values.MasterEmailID == EmailId && values.ChatToEmailID==User.Identity.Name)|| (values.MasterEmailID == User.Identity.Name && values.ChatToEmailID == EmailId)
                                                 orderby  values.ChatDateTime descending
                                                 select new
                                                 {
                                                     values.Message,
                                                     values.ChatDateTime,
                                                     values.MasterEmailID,
                                                    
                                                 }).FirstOrDefault();


                        var isread = from c in db.ChatPrivateMessageDetails
                                     where c.MasterEmailID == EmailId && c.ChatToEmailID == User.Identity.Name && c.IsRead == false select c.ID;
                                      
                        if (lastmassageByUser !=null)
                        {
                            LastMassage = lastmassageByUser.Message;
                            ChatDatetime =Convert.ToDateTime(lastmassageByUser.ChatDateTime);
                        }
                        else
                        {
                            LastMassage = "NA";
                            ChatDatetime = default(DateTime);
                        }

                        if (isread.Count() > 0)
                        {
                            incomingMessagecount = isread.Count();
                        }
                        else
                        {
                            incomingMessagecount =0;
                        }

                        var userOnlisneStatus = db.ChatUserDetails.Where(x => x.EmailID == EmailId).FirstOrDefault();

                        if(userOnlisneStatus!=null)
                        {
                            onlineStatus = "Online";
                        }
                        else
                        {
                            onlineStatus = "Offline";
                        }
                        DateTime dt1 = DateTime.Now.Date;
                        string LastSeenChat;
                        if (AllUserList[i].Last_Online_Seen != null)
                        {
                            if (AllUserList[i].Last_Online_Seen < dt1)
                            {
                                DateTime comeingTime = DateTime.Parse(Convert.ToString(AllUserList[i].Last_Online_Seen));
                                LastSeenChat = comeingTime.ToString("dd MMMM yyyy");

                            }
                            else
                            {
                                DateTime comeingTime = DateTime.Parse(Convert.ToString(AllUserList[i].Last_Online_Seen));
                                LastSeenChat = comeingTime.ToString("hh:mm tt");
                            }
                        }
                        else
                        {
                            LastSeenChat = "NA";
                        }

                        UserData.Add(new UserChatViewModel
                        {
                            userId = AllUserList[i].Id,
                            OnlineStatus = onlineStatus,
                            Picture = AllUserList[i].ProfilePic == null?"NA":AllUserList[i].ProfilePic,
                            Name = AllUserList[i].Name,
                            EmailID = AllUserList[i].UserName,
                            LastMassage = LastMassage,
                            LastSeen = LastSeenChat,
                            designation =((AllUserList[i].Designation == null)|| (AllUserList[i].Designation=="")) ? "NA" : AllUserList[i].Designation,
                            InComingmessagecount= incomingMessagecount,
                            ChatDateTime= ChatDatetime


                        }) ; 
 
                    }
                    var UserAllData = UserData.OrderByDescending(x => x.OnlineStatus).OrderByDescending(x=>x.ChatDateTime).ToList();
                    if (UserAllData.Count > 0)
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
