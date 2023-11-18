using System;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Net.Mime;
using ADCOnline.Utils;
using Website.Utils;
using MimeKit;
using MailKit.Net.Smtp;
using MimeKit.Text;
using MailKit.Security;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Numeric;

namespace Website.Models
{
    public class SendEmailModels
    {
        public string Subject { get; set; }
        public string EmailBody { get; set; }
        public string EmailSend { get; set; }
        public string Password { get; set; }
        public string To { get; set; }
        public string Cc { get; set; }
        public string Bcc { get; set; }
        public string DisName { get; set; }
        public int Port { get; set; } = 587;
        public string Servername { get; set; } = "smtp.gmail.com";
        public bool EnableSSL { get; set; } = false;
        public List<string> Files { get; set; }
        public static void SendmailKit(object objSendMailModels)
        {
            SendEmailModels obj = objSendMailModels as SendEmailModels;            
            try
            {
                MimeMessage message = new();
                MailboxAddress from = new(obj.DisName, obj.EmailSend);
                message.From.Add(from);
                List<string> listSendTo = Utility.StringToListString(obj.To);
                foreach (string item in listSendTo)
                {
                    MailboxAddress to = new(item, item);
                    message.To.Add(to);
                }
                message.Subject = obj.Subject;
                BodyBuilder bodyBuilder = new()
                {
                    HtmlBody = obj.EmailBody,
                    TextBody = obj.Subject
                };
                if (obj.Files != null)
                {
                    foreach (string item in obj.Files)
                    {
                        bodyBuilder.Attachments.Add(item);
                    }
                }
                message.Body = bodyBuilder.ToMessageBody();
                MailKit.Net.Smtp.SmtpClient client = new();
                client.CheckCertificateRevocation = false;
                client.Connect(obj.Servername, obj.Port, obj.EnableSSL);
                client.Authenticate(obj.EmailSend, obj.Password);
                client.Send(message);
                client.Disconnect(true);
                client.Dispose();
            }
            catch(Exception e)
            {
                //Console.WriteLine(e.Message);
                Common.AddLogError(e);
            }
        }
        public static string SendmailKitTest(object objSendMailModels)
        {
            SendEmailModels obj = objSendMailModels as SendEmailModels;
            try
            {
                MimeMessage mailMessage = new();
                mailMessage.From.Add(new MailboxAddress(obj.EmailSend, obj.EmailSend));
                mailMessage.To.Add(new MailboxAddress(obj.To, obj.To));
                mailMessage.Subject = obj.Subject;
                BodyBuilder bodyBuilder = new()
                {
                    HtmlBody = obj.EmailBody,
                    TextBody = obj.Subject
                };
                if (obj.Files != null)
                {
                    foreach (string item in obj.Files)
                    {
                        bodyBuilder.Attachments.Add(item);
                    }
                }
                mailMessage.Body = bodyBuilder.ToMessageBody();
                using (SmtpClient smtpClient = new())
                {
                    smtpClient.CheckCertificateRevocation = false;
                    smtpClient.Connect("smtp.gmail.com", obj.Port, obj.EnableSSL);
                    smtpClient.Authenticate(obj.EmailSend, obj.Password);
                    smtpClient.Send(mailMessage);
                    smtpClient.Disconnect(true);
                }
                return string.Empty;
            }
            catch (Exception e)
            {
                //Console.WriteLine(e.Message);
                Common.AddLogError(e);
                return e.Message;
            }
        }     
        public string GetDataTemplate(string PathTemplate)
        {
            StreamReader sr = File.OpenText(PathTemplate);
            string Content = sr.ReadToEnd();
            sr.Close();
            sr.Dispose();
            return Content;
        }
    }
}