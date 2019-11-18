using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace PMS.Models
{
    public class EmailServices
    {
        public string SendEMail(string subject, string body,string to)
        {

            try { 
                 MailMessage mailMessage = new MailMessage();
            
                SmtpClient smtp = new SmtpClient();
                System.Net.NetworkCredential NetworkCred = new System.Net.NetworkCredential();
                mailMessage.From = new MailAddress(System.Configuration.ConfigurationManager.AppSettings["FromEmailAddress"]);//reading from web.config  
                mailMessage.Subject = subject;
                mailMessage.Body = body;
                mailMessage.IsBodyHtml = true;
                mailMessage.To.Add(new MailAddress(to));

                NetworkCred.UserName = System.Configuration.ConfigurationManager.AppSettings["FromEmailAddress"]; //reading from web.config  
                                                                                                                  // NetworkCred.Password = StringCipher.Decrypt(System.Configuration.ConfigurationManager.AppSettings["sendEmailPassword"], "password");// "Password@pg01"; //reading from web.config  
                NetworkCred.Password = System.Configuration.ConfigurationManager.AppSettings["sendEmailPassword"];// "Password@pg01"; //reading from web.config  

                smtp.Host = System.Configuration.ConfigurationManager.AppSettings["EmailHostAddress"];//reading from web.config  
                smtp.EnableSsl = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["EnableSsl"]);//reading from web.config  
                smtp.Port = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["EmailPort"]);  //reading from web.config  
                smtp.UseDefaultCredentials = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["UseDefaultCredentials"]); ;
                smtp.Credentials = NetworkCred;
                smtp.Send(mailMessage);
            
        }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return "ok";
        }

    }
}