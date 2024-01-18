using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RabbitMQ2
{
    public class WorkerBase<T> : BackgroundService where T : class
    {
        public readonly string _exchangeName;
        public readonly string _queueName;
        private IModel _channel;
        private EventingBasicConsumer _consumer;

        public WorkerBase(string exchangeName, string queueName)
        {
            _exchangeName = exchangeName;
            _queueName = queueName;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            _channel = RabbitMQHelper.GetChannel();
            RabbitMQHelper.Receive<T>(_exchangeName, _queueName, _channel, message => ProcessMessageAsync(message));

            return Task.CompletedTask;
        }
  


        protected virtual async Task ProcessMessageAsync(T message)
        {
            // 在这里实现你的消息处理逻辑
            Console.WriteLine($"接收到消息: {message}");
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _channel?.Close();
            _channel?.Dispose();

            await base.StopAsync(cancellationToken);
        }
    }
}
