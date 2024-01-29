using System.Text.Json;


namespace LessonFive.Models
{
    public enum Command
    {
        Register,
        Message,
        Confirmation
    }
    public class MessageUdp
    {
        public Command Command { get; set; }
        public int? Id { get; set; }
        public string FromName { get; set; }
        public string ToName { get; set; }
        public string TextMessage { get; set; }
        public DateTime TimeSendingMessage { get; set; }
       
        public string GetJsonFromMessage()
        {
            return JsonSerializer.Serialize(this);
        }

        public static MessageUdp? GetMessageForomJson(string message)
        {
            return JsonSerializer.Deserialize<MessageUdp>(message);
        }

        public override string ToString()
        {
            return $"\n({TimeSendingMessage.ToShortTimeString()}) Получено сообщение.\nОтправитель {FromName}: {TextMessage}\n";
        }
    }
}
