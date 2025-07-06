using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace TreeVisualizer.Services
{
    public class EmailService
    {
        private string smtpServer { get; set; } = "smtp.gmail.com";
        private int smtpPort { get; set; } = 587;
        private string senderEmail { get; set; } = "dthung6604@gmail.com";
        private string senderPassword { get; set; } = "racp djum gjzf rhyi";

        public EmailService()
        {

        }
        public EmailService(string smtpServer, int smtpPort, string senderEmail, string senderPassword)
        {
            this.smtpServer = smtpServer;
            this.smtpPort = smtpPort;
            this.senderEmail = senderEmail;
            this.senderPassword = senderPassword;
        }

        public bool SendEmail(string recipientEmail, string subject, string body)
        {
            try
            {
                // Cấu hình đối tượng SmtpClient
                SmtpClient smtpClient = new SmtpClient(smtpServer)
                {
                    Port = smtpPort,
                    Credentials = new NetworkCredential(senderEmail, senderPassword),
                    EnableSsl = true // Kích hoạt SSL
                };

                // Cấu hình thông tin email
                MailMessage mailMessage = new MailMessage
                {
                    From = new MailAddress(senderEmail, "MC Application"), // Thêm tên người gửi ở đây
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };
                mailMessage.To.Add(recipientEmail);

                // Gửi email
                smtpClient.Send(mailMessage);

                return true; // Gửi thành công
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending email: {ex.Message}");
                return false; // Gửi thất bại
            }
        }
    }
}
