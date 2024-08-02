namespace CrystalMessagerWPF
{
    internal class Message
    {
        public string UserName { get; set; }
        public string UserMessage { get; set; }

        public Message(string userName, string userMessage)
        {
            UserName = userName;
            UserMessage = userMessage;
        }
    }
}
