namespace opensis.data.ViewModels
{
    public class SmtpSettings
    {
        public string FromAddress { get; set; } = "";
        public string Host { get; set; } = "localhost";
        public int Port { get; set; } = 25;
        public bool EnableSsl { get; set; }
        public string Username { get; set; } = "";
        public string Password { get; set; } = "";
    }
}
