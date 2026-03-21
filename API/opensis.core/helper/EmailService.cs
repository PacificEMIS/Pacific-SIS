using System.Net;
using System.Net.Mail;
using opensis.data.ViewModels;

namespace opensis.core.helper
{
    public static class EmailService
    {
        public static void SendPasswordResetEmail(string toEmail, string resetLink, SmtpSettings smtpSettings)
        {
            using var message = new MailMessage();
            message.From = new MailAddress(smtpSettings.FromAddress);
            message.To.Add(toEmail);
            message.Subject = "Password Reset Request";
            message.IsBodyHtml = true;
            message.Body = $@"
                <p>You have requested to reset your password.</p>
                <p>Click the link below to set a new password. This link will expire in 30 minutes.</p>
                <p><a href=""{resetLink}"">Reset Password</a></p>
                <p>If you did not request this, please ignore this email.</p>";

            using var client = new SmtpClient(smtpSettings.Host, smtpSettings.Port);
            client.EnableSsl = smtpSettings.EnableSsl;

            if (!string.IsNullOrEmpty(smtpSettings.Username))
            {
                client.Credentials = new NetworkCredential(smtpSettings.Username, smtpSettings.Password);
            }

            client.Send(message);
        }
    }
}
