using LessonFive.Abstraction;
using LessonFive.Models;
using System.Net;
using System.Net.Sockets;


namespace LessonFive.Services
{
    internal class Client
    {
        private readonly string nickname;
        private readonly IMessageSource messageSource;
        private UdpClient udpClient;
        private IPEndPoint ipEndPoint;

        public Client(IMessageSource messageSource, IPEndPoint ipEndPoint, string nickname)
        {
            this.messageSource = messageSource;
            this.ipEndPoint = ipEndPoint;
            this.nickname = nickname;
        }
        private void Registed()
        {
            var messageJson = new MessageUdp()
            {
                Command = Command.Register,
                FromName = nickname
            };
            messageSource.SendMessage(messageJson, ipEndPoint);
        }
        public void ClientSendler()
        {
            while (true)
            {
                Console.Write("Введите сообщение: ");
                string? inputMessage = Console.ReadLine();
                if (String.IsNullOrEmpty(inputMessage))
                {
                    continue;
                }
                Console.Write("Введите никнейм, кому хотите отправить сообщение: ");
                string? inputName = Console.ReadLine();
                var messageJson = new MessageUdp()
                {
                    TextMessage = inputMessage,
                    FromName = nickname
                };
                messageSource.SendMessage(messageJson, ipEndPoint);
            }
        }
        public void ClientListener()
        {
            Registed();
            IPEndPoint endPoint = new IPEndPoint(ipEndPoint.Address, ipEndPoint.Port);
            while (true)
            {                
                MessageUdp messageUdp = messageSource.ReceiveMessage(ref endPoint);
                Console.WriteLine(messageUdp);
            }
        }
    }
}
