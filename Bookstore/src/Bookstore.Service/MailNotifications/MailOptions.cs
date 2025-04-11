namespace Bookstore
{
    [Rhetos.Options("Bookstore:Mail")]
    public class MailOptions
    {

        public string SmtpHost { get; set; }
        public string FromMailAddress { get; set; }
    }
}