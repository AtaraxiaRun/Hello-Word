using Microsoft.AspNetCore.Mvc;

namespace RabbitMQ1.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class RabbitMQSendController : ControllerBase
    {
        [HttpGet]
        public void SendMessage()
        {
            new Product().SendMessage1();
            new Product().SendMessage2();
        }

    }

    public class Product
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public async Task SendMessage1()
        {
            await Task.Run(() =>
            {
                int i = 1;
                Product product;
                string exchangeName = "ProductExchange";
                string queName = "ProductQueue";
                while (true)
                {
                    product = new Product() { Id = i, Name = $"产品{i}" };
                    Thread.Sleep(500);
                    RabbitMQHelper.SendMsg(exchangeName, queName, product);
                    Console.WriteLine($"发送{product.Name} 产品信息成功");
                    i++;
                }
            });

        }
        public async Task SendMessage2()
        {
            await Task.Run(() =>
            {
                int i = 1;
                Product product;
                string exchangeName = "ProductExchange2";
                string queName = "ProductQueue2";
                while (true)
                {
                    product = new Product() { Id = i, Name = $"产品{i}" };
                    Thread.Sleep(500);
                    RabbitMQHelper.SendMsg(exchangeName, queName, product);
                    Console.WriteLine($"发送{product.Name} 产品信息成功");
                    i++;
                }
            });

        }
    }
}
