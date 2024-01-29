using System.Net;
using System.Net.Sockets;
using System.Text;
using LessonFive.Models;



namespace LessonFive.Services
{
    class Server
    {
        Dictionary<string, IPEndPoint> clients = new Dictionary<string, IPEndPoint>();
        UdpClient udpClient;

        void Register(MessageUdp message, IPEndPoint fromep)
        {
            Console.WriteLine("Message Register, name = " + message.FromName);
            clients.Add(message.FromName, fromep);


            using (var ctx = new Context())
            {
                if (ctx.Users.FirstOrDefault(x => x.Name == message.FromName) != null) return;

                ctx.Add(new User { Name = message.FromName });

                ctx.SaveChanges();
            }
        }

        void ConfirmMessageReceived(int? id)
        {
            Console.WriteLine("Message confirmation id=" + id);

            using (var ctx = new Context())
            {
                var msg = ctx.Messages.FirstOrDefault(x => x.Id == id);

                if (msg != null)
                {
                    msg.Received = true;
                    ctx.SaveChanges();
                }
            }
        }

        void RelyMessage(MessageUdp message)
        {
            int? id = null;
            if (clients.TryGetValue(message.ToName, out IPEndPoint ep))
            {
                using (var ctx = new Context())
                {
                    var fromUser = ctx.Users.First(x => x.Name == message.FromName);
                    var toUser = ctx.Users.First(x => x.Name == message.ToName);
                    var msg = new Message { FromUser = fromUser, ToUser = toUser, Received = false, Text = message.TextMessage };
                    ctx.Messages.Add(msg);

                    ctx.SaveChanges();

                    id = msg.Id;
                }


                var forwardMessageJson = new MessageUdp() { Id = id, Command = Command.Message, ToName = message.ToName, FromName = message.FromName, TextMessage = message.TextMessage }.GetJsonFromMessage();

                byte[] forwardBytes = Encoding.ASCII.GetBytes(forwardMessageJson);

                udpClient.Send(forwardBytes, forwardBytes.Length, ep);
                Console.WriteLine($"Message Relied, from = {message.FromName} to = {message.ToName}");
            }
            else
            {
                Console.WriteLine("Пользователь не найден.");
            }
        }

        void ProcessMessage(MessageUdp message, IPEndPoint fromep)
        {
            Console.WriteLine($"Получено сообщение от {message.FromName} для {message.ToName} с командой {message.Command}:");
            Console.WriteLine(message.TextMessage);


            if (message.Command == Command.Register)
            {
                Register(message, new IPEndPoint(fromep.Address, fromep.Port));

            }
            if (message.Command == Command.Confirmation)
            {
                Console.WriteLine("Confirmation receiver");
                ConfirmMessageReceived(message.Id);
            }
            if (message.Command == Command.Message)
            {
                RelyMessage(message);
            }
        }
        public void Work()
        {

            IPEndPoint remoteEndPoint;

            udpClient = new UdpClient(12345);
            remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);

            Console.WriteLine("UDP Клиент ожидает сообщений...");

            while (true)
            {
                byte[] receiveBytes = udpClient.Receive(ref remoteEndPoint);
                string receivedData = Encoding.ASCII.GetString(receiveBytes);

                Console.WriteLine(receivedData);

                try
                {
                    var message = MessageUdp.GetMessageForomJson(receivedData);

                    ProcessMessage(message, remoteEndPoint);

                }
                catch (Exception ex)
                {
                    Console.WriteLine("Ошибка при обработке сообщения: " + ex.Message);
                }
            }

        }
    }
}


