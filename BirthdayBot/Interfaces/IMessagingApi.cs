namespace BirthdayBot.Interfaces
{
    public interface IMessagingApi
    {
        bool Send(string s);
        void AddHandler(IMessageHandler handler);
        void Fetch();
    }
}