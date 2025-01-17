using System.Net;
using System.Net.Mail;
using AdminServer.Interface.Sending;

namespace AdminServer.Sending
{
    internal class EmailSeding : IEmailSeding
    {
        SmtpClient smtpClient;
        string SenderEmail;

        public EmailSeding()
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
            SenderEmail = builder.GetSection("Mailhog:SenderEmail").Value;

            smtpClient = new SmtpClient(builder.GetSection("Mailhog:Host").Value)
            {
                Port = 1025,
                EnableSsl = false,
                Credentials = new NetworkCredential(SenderEmail, string.Empty)
            };
        }

        public async Task PasswordCheckEmailAsync(string EmailTo, string url, string scheme, string host)
        {
            try
            {
                var mailMessage = new MailMessage
                {
                    From = new MailAddress(SenderEmail, "HomeBook"),
                    Subject = "Test Email",
                    Body = $"Please confirm your password by clicking here: <a href='{scheme}://{host}/reset-password/{url}/action'>link</a>",
                    IsBodyHtml = true,
                };
                mailMessage.To.Add(EmailTo);

                await smtpClient.SendMailAsync(mailMessage);
            }
            catch (SmtpException smtpEx)
            {
                throw new Exception($"SMTP помилка: {smtpEx.StatusCode}, {smtpEx.Message}, Додаткові дані: {smtpEx.InnerException?.Message}");

            }
            catch (Exception ex)
            {
                throw new Exception("Не вдалося підключитися до SMTP", ex);
            }
        }

        public async Task Writing(string EmailTo, string text)
        {
            try
            {
                var mailMessage = new MailMessage
                {
                    From = new MailAddress(SenderEmail, "HomeBook"),
                    Subject = "Support",
                    Body = $"{text}",
                    IsBodyHtml = true,
                };
                mailMessage.To.Add(EmailTo);

                await smtpClient.SendMailAsync(mailMessage);
            }
            catch (SmtpException smtpEx)
            {
                throw new Exception($"SMTP помилка: {smtpEx.StatusCode}, {smtpEx.Message}, Додаткові дані: {smtpEx.InnerException?.Message}");

            }
            catch (Exception ex)
            {
                throw new Exception("Не вдалося підключитися до SMTP", ex);
            }
        }
    }
}