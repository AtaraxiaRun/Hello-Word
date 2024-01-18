using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace RabbitMQ1
{
    public class RabbitMQHelper
    {
        private static ConnectionFactory connectionFactory = new ConnectionFactory
        {
            HostName = ApplicationConfig.Configuration["RabbitMQ:Host"],
            UserName = ApplicationConfig.Configuration["RabbitMQ:UserName"],
            Password = ApplicationConfig.Configuration["RabbitMQ:Password"],
        };

        private static IConnection connection;

        private RabbitMQHelper()
        {
        }

        private static void CreateConn()
        {
            connection = connectionFactory.CreateConnection();
        }

        public static bool SendMsg<T>(string exchangeName, string queName, T msg) where T : class
        {
            if (msg == null)
            {
                return false;
            }

            try
            {
                if (connection == null || !connection.IsOpen)
                {
                    CreateConn();
                }

                using (IModel model = connection.CreateModel())
                {
                    if (!string.IsNullOrEmpty(exchangeName))
                    {
                        model.ExchangeDeclare(exchangeName, "direct", durable: true);
                        model.QueueDeclare(queName, durable: true, exclusive: false, autoDelete: false, null);
                        model.QueueBind(queName, exchangeName, queName);
                    }
                    else
                    {
                        model.QueueDeclare(queName, durable: true, exclusive: false, autoDelete: false, null);
                    }

                    IBasicProperties basicProperties = model.CreateBasicProperties();
                    basicProperties.DeliveryMode = 2;
                    byte[] bytes = Encoding.UTF8.GetBytes(msg.ToJson());
                    PublicationAddress addr = new PublicationAddress("direct", exchangeName, queName);
                    model.BasicPublish(addr, basicProperties, bytes);
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool SendMessages<T>(string exchangeName, string queName, List<T> msgs) where T : class
        {
            if (msgs == null && !msgs.Any())
            {
                return false;
            }

            try
            {
                if (connection == null || !connection.IsOpen)
                {
                    CreateConn();
                }

                IModel channel = connection.CreateModel();
                try
                {
                    if (!string.IsNullOrEmpty(exchangeName))
                    {
                        channel.ExchangeDeclare(exchangeName, "direct", durable: true);
                        channel.QueueDeclare(queName, durable: true, exclusive: false, autoDelete: false, null);
                        channel.QueueBind(queName, exchangeName, queName);
                    }
                    else
                    {
                        channel.QueueDeclare(queName, durable: true, exclusive: false, autoDelete: false, null);
                    }

                    IBasicProperties basicProperties = channel.CreateBasicProperties();
                    basicProperties.DeliveryMode = 2;
                    PublicationAddress address = new PublicationAddress("direct", exchangeName, queName);
                    msgs.ForEach(delegate (T msg)
                    {
                        byte[] bytes = Encoding.UTF8.GetBytes(msg.ToJson());
                        channel.BasicPublish(address, basicProperties, bytes);
                    });
                }
                finally
                {
                    if (channel != null)
                    {
                        channel.Dispose();
                    }
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static IModel GetChannel()
        {
            if (connection == null || !connection.IsOpen)
            {
                CreateConn();
            }

            return connection.CreateModel();
        }

        public static void Receive<T>(string exchangeName, string queName, IModel channel, Action<T> received) where T : class
        {
            try
            {
                if (!string.IsNullOrEmpty(exchangeName))
                {
                    channel.ExchangeDeclare(exchangeName, "direct", durable: true);
                    channel.QueueDeclare(queName, durable: true, exclusive: false, autoDelete: false, null);
                    channel.QueueBind(queName, exchangeName, queName);
                }
                else
                {
                    channel.QueueDeclare(queName, durable: true, exclusive: false, autoDelete: false, null);
                }

                channel.BasicQos(0u, 1, global: false);
                EventingBasicConsumer eventingBasicConsumer = new EventingBasicConsumer(channel);
                eventingBasicConsumer.Received += delegate (object ch, BasicDeliverEventArgs ea)
                {
                    string @string = Encoding.UTF8.GetString(ea.Body);
                    T obj = @string.ToObject<T>();
                    DateTime now = DateTime.Now;
                    received(obj);
                    TimeSpan timeSpan = DateTime.Now - now;
                    if (!channel.IsClosed)
                    {
                        Console.WriteLine(string.Format("任务执行完成，用时 {0}s {1} 队列剩余任务数量： {2}", timeSpan.TotalSeconds.ToString("0.00"), queName, channel.MessageCount(queName)));
                        channel.BasicAck(ea.DeliveryTag, multiple: false);
                    }
                };
                channel.BasicConsume(queName, autoAck: false, eventingBasicConsumer);
            }
            catch (Exception)
            {
            }
        }

        public void Dispose()
        {
            connection.Close();
        }

        public void ReceiveClose()
        {
        }

        public static int GetMessageCount(string exchangeName, string queName)
        {
            try
            {
                if (connection == null || !connection.IsOpen)
                {
                    CreateConn();
                }

                using IModel model = connection.CreateModel();
                if (!string.IsNullOrEmpty(exchangeName))
                {
                    model.ExchangeDeclare(exchangeName, "direct", durable: true);
                    model.QueueBind(queName, exchangeName, queName);
                }

                return model.MessageCount(queName).ToInt32();
            }
            catch
            {
                return -1;
            }
        }
    }

}
