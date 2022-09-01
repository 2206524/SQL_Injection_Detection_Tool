using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Mail;

namespace SQLIDetector
{
    class Email
    {
        private string smtpAddress { get; set; }
        private int portNumber { get; set; }
        private bool enableSSL { get; set; }
        private string emailFromAddress { get; set; }
        private string password { get; set; }
        public string emailToAddress { get; set; }
        public string subject { get; set; }
        public string body { get; set; }


        public void SendEmail()
        {
            smtpAddress = "smtp.gmail.com";
            portNumber = 587;
            enableSSL = true;
            using (MailMessage mail = new MailMessage())
            {
                mail.From = new MailAddress(emailFromAddress);
                mail.To.Add(emailToAddress);
                mail.Subject = subject;
                mail.Body = body;
                mail.IsBodyHtml = true;
                //mail.Attachments.Add(new Attachment("D:\\TestFile.txt"));//--Uncomment this to send any attachment  
                using (SmtpClient smtp = new SmtpClient(smtpAddress, portNumber))
                {
                    smtp.Credentials = new NetworkCredential(emailFromAddress, password);
                    smtp.EnableSsl = enableSSL;
                    smtp.Send(mail);
                }
            }
        }


    }
}
