using System.Net;
using LessonFive.Models;

namespace LessonFive.Abstraction
{
    public interface IMessageSource
    {
        void SendMessage(MessageUdp message, IPEndPoint ipEndPoint);

        MessageUdp ReceiveMessage(ref IPEndPoint ipEndPoint);
    }
}
