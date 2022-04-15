namespace MeAgendaAí.Infra.Notification
{
    public class Notification
    {
        public string Key { get; }
        public string Message { get; }
        public Notification(string key, string message) => (Key, Message) = (key, message);
    }
}