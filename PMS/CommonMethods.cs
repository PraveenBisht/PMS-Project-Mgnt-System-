using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading;
using System.Web;

namespace PMS
{
    public class CommonMethods
    {
        public static void SendMail(string Toemailaddress, string Fromemailaddress, string subject, string body, bool IsBodyHtml)
        {
            try
            {

                MailMessage mailMessage = new MailMessage();

                SmtpClient smtp = new SmtpClient();
                System.Net.NetworkCredential NetworkCred = new System.Net.NetworkCredential();
                mailMessage.From = new MailAddress(System.Configuration.ConfigurationManager.AppSettings["FromEmailAddress"]);//reading from web.config  
                mailMessage.Subject = subject;
                mailMessage.Body = body;
                mailMessage.IsBodyHtml = true;
                mailMessage.To.Add(new MailAddress(Toemailaddress));

                NetworkCred.UserName = System.Configuration.ConfigurationManager.AppSettings["FromEmailAddress"]; //
                NetworkCred.Password = System.Configuration.ConfigurationManager.AppSettings["sendEmailPassword"];// 

                smtp.Host = System.Configuration.ConfigurationManager.AppSettings["EmailHostAddress"];
                smtp.EnableSsl = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["EnableSsl"]); 
                smtp.Port = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["EmailPort"]);  //
                smtp.UseDefaultCredentials = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["UseDefaultCredentials"]); ;
                smtp.Credentials = NetworkCred;
                smtp.Send(mailMessage);

                smtp.Dispose();
            }
            catch (Exception e)
            {
                throw;
            }
            
        }
    }
}