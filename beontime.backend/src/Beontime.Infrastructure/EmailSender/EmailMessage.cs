namespace Beontime.Infrastructure.EmailSender
{

    public sealed class EmailMessage
    {
        public string Sender { get; set; } = "";
        public string Reciever { get; set; } = "";
        public string Subject { get; set; } = "";
        public string Content { get; set; } = "";
    }
}
