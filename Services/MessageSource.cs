using LessonFive.Abstraction;
using LessonFive.Models;
using System.Net;
using System.Net.Sockets;
using System.Text;


namespace LessonFive.Services
{
    internal class MessageSource : IMessageSource
    {
        private readonly UdpClient udpClient;
        public MessageSource(int port)
        {
            udpClient = new UdpClient(port);
        }


        public MessageUdp ReceiveMessage(ref IPEndPoint ipEndPoint)
        {
            byte[] buffer = udpClient.Receive(ref ipEndPoint);
            string messageJson = Encoding.UTF8.GetString(buffer);
            return MessageUdp.GetMessageForomJson(messageJson);
        }

        public void SendMessage(MessageUdp message, IPEndPoint iPEndPoint)
        {
            string messageJson = message.GetJsonFromMessage();
            byte[] buffer = Encoding.UTF8.GetBytes(messageJson);
            udpClient.Send(buffer, iPEndPoint);
        }
    }
}
