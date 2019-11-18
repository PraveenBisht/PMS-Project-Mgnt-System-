using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;
using PMS.Models;

namespace PMS
{
    public class ChatHub : Hub
    {
        private static readonly ConcurrentDictionary<string, UserHubModels> Users =
           new ConcurrentDictionary<string, UserHubModels>(StringComparer.InvariantCultureIgnoreCase);

        PMSEntities dc = new PMSEntities();
        public static string emailIDLoaded = "";

        public void SendMessage(string message, string send_connectionID,string Sender_ConnectionId)
        //I have defined 2 parameters. These are the parameters to be sent here from the client software
        {
            var Send_to_User_Details = dc.ChatUserDetails.Where(x => x.EmailID == send_connectionID).FirstOrDefault();
            var sender_From_userChatDetails = dc.ChatUserDetails.Where(x => x.EmailID == Sender_ConnectionId).FirstOrDefault();

            if (Send_to_User_Details != null && sender_From_userChatDetails!=null)
            {
                var Send_To_User_connectionID = Send_to_User_Details.ConnectionId; 
                var sender_connectionID = sender_From_userChatDetails.ConnectionId;
                var toUser = dc.ChatUserDetails.FirstOrDefault(x => x.ConnectionId == Send_To_User_connectionID);
                var fromUser = dc.ChatUserDetails.FirstOrDefault(x => x.ConnectionId == sender_connectionID);
                if (fromUser != null)
                {

                    AddPrivateMessageinCacheForMobile(fromUser.EmailID, toUser.EmailID, fromUser.UserName, message,true);
               
                    // send to 
                    Clients.Client(Send_To_User_connectionID).sendPrivateMessage(sender_connectionID, fromUser.UserName, message, fromUser.EmailID, toUser.EmailID, "Click", sender_connectionID);
                    // send to caller user
                    Clients.Caller.sendPrivateMessage(Send_To_User_connectionID, fromUser.UserName, message, fromUser.EmailID, toUser.EmailID, "Click", sender_connectionID);
                    //  Clients.Client(send_connectionID).SendMessage(message, fromUser.UserName);
                
                }
            }
           else  if (sender_From_userChatDetails != null)
            {
                var sender_connectionID = sender_From_userChatDetails.ConnectionId;
                var fromUser = dc.ChatUserDetails.FirstOrDefault(x => x.ConnectionId == sender_connectionID);
                AddPrivateMessageinCacheForMobile(Sender_ConnectionId, send_connectionID, sender_From_userChatDetails.UserName, message,false);
                Notification objNotif = new Notification();
                objNotif.SentTo = send_connectionID;
                dc.Configuration.ProxyCreationEnabled = false;
                dc.Notifications.Add(objNotif);
                dc.SaveChanges();
                Clients.Caller.sendPrivateMessage(send_connectionID, fromUser.UserName, message, fromUser.EmailID, send_connectionID, "Click", sender_connectionID);
            }

        }
        public override Task OnConnected()
        {
            string onlineStatus;
            var connectionID = Context.ConnectionId;
            string userName = Context.QueryString["username"];
            if (userName!="")
            {
                if (string.IsNullOrEmpty(userName))
                {
                    userName = Context.Headers["username"];
                }
                var UserDetailsByEmailid = dc.AspNetUsers.Where(x => x.UserName == userName).FirstOrDefault();
                if (UserDetailsByEmailid != null)
                {
                    string Name = UserDetailsByEmailid.Name;

                    var Users = dc.ChatUserDetails.ToList();
                    if (Users.Where(x => x.EmailID == userName).ToList().Count == 0)
                    {
                        var userdetails = new ChatUserDetail
                        {
                            ConnectionId = connectionID,
                            UserName = Name,
                            EmailID = userName
                        };
                        dc.ChatUserDetails.Add(userdetails);
                        dc.SaveChanges();
                        // send to all except caller client
                        Clients.AllExcept(connectionID).onNewUserConnected(connectionID, Name, userName);

                        var UserDataByEmail = (from u in dc.AspNetUsers
                                               where u.UserName == userName
                                               select new
                                               {
                                                   u.Id,
                                                   u.Name,
                                                   u.ProfilePic,
                                                   u.UserName,
                                                   u.Email,

                                               }).FirstOrDefault();
                        var userOnlisneStatus = dc.ChatUserDetails.Where(x => x.EmailID == userName).FirstOrDefault();

                        if (userOnlisneStatus != null)
                        {
                            onlineStatus = "Online";
                        }
                        else
                        {
                            onlineStatus = "Offline";
                        }


                        // send to all except caller client
                        Clients.AllExcept(connectionID).onNewUserConnect(connectionID, Name, userName, UserDataByEmail.ProfilePic, onlineStatus);
                        //Clients.AllExcept(connectionID).onNewUserConnected(connectionID, Name, userName);

                    }
                }
                // send to caller
                var connectedUsers = dc.ChatUserDetails.ToList();
                // var CurrentMessage = dc.ChatMessageDetails.ToList();
                var AllConnectedconnectedUsers = AllUserList(userName);
                Clients.Caller.onConnected(connectionID, userName, AllConnectedconnectedUsers);
                string json = JsonConvert.SerializeObject(connectedUsers); //send to client
                Clients.All.getUserList(json);
            }
            return base.OnConnected();
        }

        public List<UserChatViewModel> AllUserList(string UserEmailID)
        {
            string onlineStatus, LastMassage;
            DateTime ChatDatetime;
            int incomingMessagecount;
            List<UserChatViewModel> UserData = new List<UserChatViewModel>();
            var AllUserList = (from u in dc.AspNetUsers
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

                var isread = from c in dc.ChatPrivateMessageDetails
                             where c.MasterEmailID == EmailId && c.ChatToEmailID == UserEmailID && c.IsRead == false
                             select c.ID;


                if (isread.Count() > 0)
                {
                    incomingMessagecount = isread.Count();
                }
                else
                {
                    incomingMessagecount = 0;
                }

                var userOnlisneStatus = dc.ChatUserDetails.Where(x => x.EmailID == EmailId).FirstOrDefault();

                if (userOnlisneStatus != null)
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
                    Picture = AllUserList[i].ProfilePic == null ? "NA" : AllUserList[i].ProfilePic,
                    Name = AllUserList[i].Name,
                    EmailID = AllUserList[i].UserName,
                    LastSeen = LastSeenChat,
                    InComingmessagecount = incomingMessagecount,
                });

            }
            var UserAllData = UserData.OrderByDescending(x => x.OnlineStatus).OrderByDescending(x => x.ChatDateTime).ToList();

            return UserAllData;
        }
        #region Connect
        public void Connect(string userName, string email)
      {
            string onlineStatus;
            emailIDLoaded = email;
            var id = Context.ConnectionId;

            var item = dc.ChatUserDetails.FirstOrDefault(x => x.EmailID == email);
            if (item != null)
            {
                dc.ChatUserDetails.Remove(item);
                dc.SaveChanges();

                // Disconnect
                Clients.All.onUserDisconnectedExisting(item.ConnectionId, email);
            }
            else
            { 

            var Users = dc.ChatUserDetails.ToList();
                if (Users.Where(x => x.EmailID == email).ToList().Count == 0)
                {
                    var userdetails = new ChatUserDetail
                    {
                        ConnectionId = id,
                        UserName = userName,
                        EmailID = email
                    };
                    dc.ChatUserDetails.Add(userdetails);
                    dc.SaveChanges();
                    var AllConnectedconnectedUsers = AllUserList(userName);
                    // send to caller
                    //var connectedUsers = dc.ChatUserDetails.ToList();
                    // var CurrentMessage = dc.ChatMessageDetails.ToList();


                    Clients.Caller.Connect(id, userName, AllConnectedconnectedUsers);
                }
            }

            var UserDataByEmail = (from u in dc.AspNetUsers
                               where u.UserName == email
                               select new
                               {
                                   u.Id,
                                   u.Name,
                                   u.ProfilePic,
                                   u.UserName,
                                   u.Email,

                               }).FirstOrDefault();
            var userOnlisneStatus = dc.ChatUserDetails.Where(x => x.EmailID == email).FirstOrDefault();

            if (userOnlisneStatus != null)
            {
                onlineStatus = "Online";
            }
            else
            {
                onlineStatus = "Offline";
            }


            // send to all except caller client
            Clients.AllExcept(id).onNewUserConnect(id, userName, email, UserDataByEmail.ProfilePic, onlineStatus);

        }
        #endregion

        #region Disconnect
        public override System.Threading.Tasks.Task OnDisconnected(bool stopCalled)
        {

            var item = dc.ChatUserDetails.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
            if (item != null)
            {
                dc.ChatUserDetails.Remove(item);
                dc.SaveChanges();
                var id = Context.ConnectionId;
                string Emailid = item.EmailID;
                var UpdateUser = dc.AspNetUsers.Where(x => x.UserName == Emailid).FirstOrDefault();
                UpdateUser.Last_Online_Seen = DateTime.Now;
                dc.Entry(UpdateUser).State = EntityState.Modified;
                dc.SaveChanges();
                Clients.All.onUserDisconnected(id, item.UserName,item.EmailID);
                //Clients.All.onUserDisconnect(id, item.UserName, item.EmailID);

                var users = dc.ChatUserDetails.FirstOrDefault(x => x.EmailID == item.UserName);
                string json = JsonConvert.SerializeObject(users); 
                 Clients.All.getUserList(json);
            }
            return base.OnDisconnected(stopCalled);
        }
        #endregion

        #region Disconnect
        public void DisconnectedLoggedInUser(string userid)
        {

            var item = dc.ChatUserDetails.FirstOrDefault(x => x.ConnectionId == userid);
            if (item != null)
            {
                dc.ChatUserDetails.Remove(item);
                dc.SaveChanges();

                var id = Context.ConnectionId;
                Clients.All.onUserDisconnected(id, item.UserName);
            }
        }
        #endregion

        #region Send_To_All
        public void SendMessageToAll(string userName, string message)
        {
            // store last 100 messages in cache
            AddAllMessageinCache(userName, message);

            // Broad cast message
            Clients.All.messageReceived(userName, message);
        }
        #endregion

        #region Private_Messages
        //public void SendPrivateMessage(string toUserId, string message, string status)
        //{
        //    string fromUserId = Context.ConnectionId;
        //    var toUser = dc.ChatUserDetails.FirstOrDefault(x => x.ConnectionId == toUserId);
        //    var fromUser = dc.ChatUserDetails.FirstOrDefault(x => x.ConnectionId == fromUserId);
        //    if (toUser != null && fromUser != null)
        //    {
        //        if (status == "Click")
        //            AddPrivateMessageinCache(fromUser.EmailID, toUser.EmailID, fromUser.UserName, message);

        //        // send to 
        //        Clients.Client(toUserId).sendPrivateMessage(fromUserId, fromUser.UserName, message, fromUser.EmailID, toUser.EmailID, status, fromUserId);

        //        // send to caller user
        //        Clients.Caller.sendPrivateMessage(toUserId, fromUser.UserName, message, fromUser.EmailID, toUser.EmailID, status, fromUserId);

        //    }
        //}


        public void SendPrivateMessage(string message, string send_connectionID, string Sender_ConnectionId, string status)
        {
            var Send_to_User_Details = dc.ChatUserDetails.Where(x => x.EmailID == send_connectionID).FirstOrDefault();
            var sender_From_userChatDetails = dc.ChatUserDetails.Where(x => x.EmailID == Sender_ConnectionId).FirstOrDefault();

            if (Send_to_User_Details != null && sender_From_userChatDetails != null)
            {
                var Send_To_User_connectionID = Send_to_User_Details.ConnectionId;
                var sender_connectionID = sender_From_userChatDetails.ConnectionId;
                var toUser = dc.ChatUserDetails.FirstOrDefault(x => x.ConnectionId == Send_To_User_connectionID);
                var fromUser = dc.ChatUserDetails.FirstOrDefault(x => x.ConnectionId == sender_connectionID);
                if (fromUser != null)
                {
                    if (status == "Click")
                    {
                        AddPrivateMessageinCacheForMobile(fromUser.EmailID, toUser.EmailID, fromUser.UserName, message, true);
                    }
                    // send to 
                    Clients.Client(Send_To_User_connectionID).sendPrivateMessage(sender_connectionID, fromUser.UserName, message, fromUser.EmailID, toUser.EmailID, status, sender_connectionID);
                    // send to caller user
                    Clients.Caller.sendPrivateMessage(Send_To_User_connectionID, fromUser.UserName, message, fromUser.EmailID, toUser.EmailID, status, sender_connectionID);
                    //  Clients.Client(send_connectionID).SendMessage(message, fromUser.UserName);
                }
            }
            else 
            if (sender_From_userChatDetails != null)
            {
                var sender_connectionID = sender_From_userChatDetails.ConnectionId;
      
                var fromUser = dc.ChatUserDetails.FirstOrDefault(x => x.ConnectionId == sender_connectionID);
                if (status == "Click")
                {
                    AddPrivateMessageinCacheForMobile(Sender_ConnectionId, send_connectionID, sender_From_userChatDetails.UserName, message, false);
                    Notification objNotif = new Notification();
                    objNotif.SentTo = send_connectionID;
                    dc.Configuration.ProxyCreationEnabled = false;
                    dc.Notifications.Add(objNotif);
                    dc.SaveChanges();

                  Clients.Caller.sendPrivateMessage(send_connectionID, fromUser.UserName, message, fromUser.EmailID, send_connectionID, status, sender_connectionID);

                }
            }

        }
        public List<PrivateChatMessage> GetPrivateMessage(string fromid, string toid, int take)
        {

            List<PrivateChatMessage> msg = new List<PrivateChatMessage>();

            var v = (from a in dc.ChatPrivateMessageMasters
                     join b in dc.ChatPrivateMessageDetails on a.EmailID equals b.MasterEmailID into cc
                     from c in cc
                     where (c.MasterEmailID.Equals(fromid) && c.ChatToEmailID.Equals(toid)) || (c.MasterEmailID.Equals(toid) && c.ChatToEmailID.Equals(fromid))
                     orderby c.ID descending
                     select new
                     {
                         UserName = a.UserName,
                         Message = c.Message,
                         ID = c.ID,
                         CreatedDate = c.ChatDateTime,
                         isread = c.IsRead,
                         MasterEmailID=c.MasterEmailID
                     }).Take(take).ToList();
            v = v.OrderBy(s => s.ID).ToList();

            foreach (var a in v)
            {
                var res = new PrivateChatMessage()
                {
                    userName = a.UserName,
                    message = a.Message,
                    CreateDatetime=a.CreatedDate.ToString(),
                    MasterEmailID=a.MasterEmailID
                };
                msg.Add(res);
            }

            return msg;

        }

        public void GetAllPrivateMessageByUserID(string fromid, string toid, int take)
        {
           
            List<PrivateChatMessage> msg = new List<PrivateChatMessage>();
            var changeMessageStatus = dc.Update_Chat_Message_IsRead_Status(fromid, toid);
            var v = (from a in dc.ChatPrivateMessageMasters
                     join b in dc.ChatPrivateMessageDetails on a.EmailID equals b.MasterEmailID into cc
                     from c in cc
                     where (c.MasterEmailID.Equals(fromid) && c.ChatToEmailID.Equals(toid)) || (c.MasterEmailID.Equals(toid) && c.ChatToEmailID.Equals(fromid))
                     orderby c.ID descending
                     select new
                     {
                         UserName = a.UserName,
                         Message = c.Message,
                         ID = c.ID,
                         CreatedDate = c.ChatDateTime,
                         isread = c.IsRead
                     }).Take(take).ToList();
            v = v.OrderBy(s => s.ID).ToList();
            DateTime dt1 = DateTime.Now.Date;
            string CreatedDateTime;
            foreach (var a in v)
            {
                if (a.CreatedDate < dt1)
                {
                    DateTime comeingTime = DateTime.Parse(Convert.ToString(a.CreatedDate));
                    CreatedDateTime = comeingTime.ToString("dd MMMM yyyy");

                }
                else
                {
                    DateTime comeingTime = DateTime.Parse(Convert.ToString(a.CreatedDate));
                    CreatedDateTime = comeingTime.ToString("hh:mm tt");
                }
                var res = new PrivateChatMessage()
                {
                    userName = a.UserName,
                    message = a.Message,
                    CreateDatetime = CreatedDateTime,
                    Isread = a.isread.ToString()
                };
                msg.Add(res);
            }

            string json = JsonConvert.SerializeObject(msg);
            Clients.All.GetPrivateMessageListByUserID(json);

        }

        private int takeCounter = 0;
        private int skipCounter = 0;
        public List<PrivateChatMessage> GetScrollingChatData(string fromid, string toid, int start = 10, int length = 1)
        {
            takeCounter = (length * start); // 20
            skipCounter = ((length - 1) * start); // 10


            List<PrivateChatMessage> msg = new List<PrivateChatMessage>();
            var v = (from a in dc.ChatPrivateMessageMasters
                     join b in dc.ChatPrivateMessageDetails on a.EmailID equals b.MasterEmailID into cc
                     from c in cc
                     where (c.MasterEmailID.Equals(fromid) && c.ChatToEmailID.Equals(toid)) || (c.MasterEmailID.Equals(toid) && c.ChatToEmailID.Equals(fromid))
                     orderby c.ID descending
                     select new
                     {
                         UserName = a.UserName,
                         Message = c.Message,
                         ID = c.ID
                     }).Take(takeCounter).Skip(skipCounter).ToList();

            foreach (var a in v)
            {
                var res = new PrivateChatMessage()
                {
                    userName = a.UserName,
                    message = a.Message
                };
                msg.Add(res);
            }
            return msg;

        }
        #endregion

        #region Save_Cache
        private void AddAllMessageinCache(string userName, string message)
        {

            var messageDetail = new ChatMessageDetail
            {
                UserName = userName,
                Message = message,
                EmailID = emailIDLoaded
            };
            dc.ChatMessageDetails.Add(messageDetail);
            dc.SaveChanges();

        }

        private void AddPrivateMessageinCache(string fromEmail, string chatToEmail, string userName, string message)
        {

            // Save master
            var master = dc.ChatPrivateMessageMasters.ToList().Where(a => a.EmailID.Equals(fromEmail)).ToList();
            if (master.Count == 0)
            {
                var result = new ChatPrivateMessageMaster
                {
                    EmailID = fromEmail,
                    UserName = userName,
                  
                };
                dc.ChatPrivateMessageMasters.Add(result);
                dc.SaveChanges();
            }

            // Save details
            var resultDetails = new ChatPrivateMessageDetail
            {
                MasterEmailID = fromEmail,
                ChatToEmailID = chatToEmail,
                Message = message,
                ChatDateTime = DateTime.Now
            };
            dc.ChatPrivateMessageDetails.Add(resultDetails);
            dc.SaveChanges();
        }

        private void AddPrivateMessageinCacheForMobile(string fromEmail, string chatToEmail, string userName, string message,bool Isread)
        {

            // Save master
            var master = dc.ChatPrivateMessageMasters.ToList().Where(a => a.EmailID.Equals(fromEmail)).ToList();
            if (master.Count == 0)
            {
                var result = new ChatPrivateMessageMaster
                {
                    EmailID = fromEmail,
                    UserName = userName,

                };
                dc.ChatPrivateMessageMasters.Add(result);
                dc.SaveChanges();
            }

            // Save details
            var resultDetails = new ChatPrivateMessageDetail
            {
                MasterEmailID = fromEmail,
                ChatToEmailID = chatToEmail,
                Message = message,
                ChatDateTime = DateTime.Now,
                IsRead= Isread
            };
            dc.ChatPrivateMessageDetails.Add(resultDetails);
            dc.SaveChanges();
        }

        #endregion



        public void GetNotification()
        {
            try
            {
                string loggedUser = Context.ConnectionId;

                //Get TotalNotification
                string totalNotif = LoadNotifData(loggedUser);

                //Send To
                UserHubModels receiver;
                if (Users.TryGetValue(loggedUser, out receiver))
                {
                    var cid = receiver.ConnectionIds.FirstOrDefault();
                    var context = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
                    context.Clients.Client(cid).broadcaastNotif(totalNotif);
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
        }

        //Specific User Call
        public void SendNotification(string SentTo)
        {
            try
            {
                //Get TotalNotification
                string totalNotif = LoadNotifData(SentTo);

                //Send To
                UserHubModels receiver;
                if (Users.TryGetValue(SentTo, out receiver))
                {
                    var cid = receiver.ConnectionIds.FirstOrDefault();
                    var context = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
                    context.Clients.Client(cid).broadcaastNotif(totalNotif);
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
        }

        private string LoadNotifData(string userId)
        {
            int total = 0;
            var query = (from t in dc.Notifications
                         where t.SentTo == userId
                         select t)
                        .ToList();
            total = query.Count;
            return total.ToString();
        }




    }

    public class PrivateChatMessage
    {
        public string userName { get; set; }
        public string message { get; set; }
        public string CreateDatetime { get; set; }
        public string Isread { get; set; }
        public string MasterEmailID { get; set; }
    }
}