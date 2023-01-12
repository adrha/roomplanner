namespace RoomPlanner.Options
{
    public class SmtpServerOptions
    {
        public const string Position = "SmtpServer";

        /// <summary>
        /// Enables the SSL-Connection
        /// </summary>
        public bool EnableSsl { get; set; }

        /// <summary>
        /// Username of the SMTP access
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Password of the SMTP access
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Port of the SMTP server
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// Hostname of the SMTP server
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// Sender-mail-address (FROM)
        /// </summary>
        public string SenderMail { get; set; }

    }
}
