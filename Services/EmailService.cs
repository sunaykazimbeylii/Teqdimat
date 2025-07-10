using System.Net;
using System.Net.Mail;
using TravelFinalProject.Interfaces;

namespace TravelFinalProject.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        public async Task SendMailAsync(string emailTo, string subject, string body, bool isHtml = true)
        {
            try
            {
                SmtpClient smpt = new SmtpClient(_configuration["Email:Host"], Convert.ToInt32(_configuration["Email:Port"]));
                smpt.EnableSsl = true;
                smpt.Credentials = new NetworkCredential(_configuration["Email:LoginEmail"], _configuration["Email:Password"]);
                smpt.DeliveryMethod = SmtpDeliveryMethod.Network;
                smpt.UseDefaultCredentials = false;

                MailAddress from = new MailAddress(_configuration["Email:LoginEmail"], "YatriiWorld");
                MailAddress to = new MailAddress(emailTo);

                MailMessage message = new MailMessage(from, to);
                message.Subject = subject;
                message.Body = body;
                message.IsBodyHtml = isHtml;

                await smpt.SendMailAsync(message);
            }
            catch (Exception ex)
            {

                Console.WriteLine("EMAIL ERROR: " + ex.Message);
                throw;
            }
        }

    }
}
