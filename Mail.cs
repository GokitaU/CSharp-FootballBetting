using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Net;
using System.Windows;



namespace FootballHedge
{
    class Mail
    {
        public static bool CheckIfSingleMailIsAllowed() {  return FootballHedge.Properties.Settings.Default.MailAllowSingle; }
        public static bool CheckIfEmailsAllowed() { return FootballHedge.Properties.Settings.Default.MailAllow;  }

        public static void SendMail(string subject, string body)
        {
            var fromAddress = new MailAddress("aa@gmail.com");
            string to = FootballHedge.Properties.Settings.Default.MailTo;
            var toAddress = new MailAddress(to);
            string pass = "***";
            try
            {
                var smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(fromAddress.Address, pass)
                };
                using (var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    Body = body,
                    BodyEncoding = Encoding.UTF8,
                    IsBodyHtml = true

                })
                {
                    smtp.Send(message);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Email send error "+ ex.Message.ToString());
            }



        }
    }




}
