using MVCTaskManager.Services.Interfaces;
using MimeKit;
using MailKit.Net.Smtp;


namespace MVCTaskManager.Services
{
    public class MailService : IMailService
    {
        private readonly string _smtpServer;
        private readonly int _smtpPort;
        private readonly string _smtpUser;
        private readonly string _smtpPass;
        private readonly IConfiguration _configuration;

        public MailService(IConfiguration configuration)
        {
            _configuration = configuration;
            _smtpServer = _configuration["Smtp:SmtpServer"];
            _smtpPort = (int)Convert.ToInt64(_configuration["Smtp:SmtpPort"]);
            _smtpUser = _configuration["Smtp:SmtpUser"];
            _smtpPass = _configuration["Smtp:SmtpPass"];
        }

        public async Task SendMail(string userName, string url, string email)
        {

            var mailMessage = new MimeMessage();

            //var toAdddressList = new InternetAddressList();
            //var toEmailList = "Hello@gmail.com;Bye@gamil.com".Split(';').ToList();
            //toEmailList.ForEach(mailId => toAdddressList.Add(MailboxAddress.Parse(mailId)));
            //mailMessage.To.AddRange(toAdddressList);

            var fromEmail = "PasswprdExpiry@gamil.com";
            mailMessage.Sender = MailboxAddress.Parse(fromEmail);
            mailMessage.To.Add(MailboxAddress.Parse(email));

            mailMessage.Subject = "Password Recovery Request";

            var builder = new BodyBuilder();
            builder.HtmlBody = $"<h2>Dear {userName},</h2> <p>We received a request to reset your Smartr account password! If this was you, click on the button below to create a new password and sign in.</p><p><a href=\"{url}\">Reset Password</a></p><p>If you did not make this request, please ignore this email.</p><div> <p>Thank you,<br>The Smartr Team</p></div>";
            mailMessage.Body = builder.ToMessageBody();

            using (var smtpClient = new SmtpClient())
            {
                // Connect to Gmail SMTP server
                await smtpClient.ConnectAsync(_smtpServer, _smtpPort, MailKit.Security.SecureSocketOptions.StartTls);

                // Authenticate with your email account
                await smtpClient.AuthenticateAsync(_smtpUser, _smtpPass);

                // Send the email
                await smtpClient.SendAsync(mailMessage);

                // Disconnect
                await smtpClient.DisconnectAsync(true);
            }


        }
    }
}
