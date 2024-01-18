﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQ2
{
    public class SyncProductWork : WorkerBase<Product>
    {
        private readonly IProductService _productService;

        public SyncProductWork(IProductService productService) : base(exchangeName: "ProductExchange", queueName: "ProductQueue")
        {

            _productService = productService;
        }

        protected override async Task ProcessMessageAsync(Product message)
        {
            Thread.Sleep(1000);
            Console.WriteLine("开始处理业务逻辑");
            Console.WriteLine($"<------------------ begin MQ  Product ------------------>");
            try
            {
                var pushKey = string.Format("{0}{1}", message.Id, message.Name);
                Console.WriteLine($"<------------------ begin consumer {pushKey} ------------------>");
                await _productService.UpdateProduct(message);
                Console.WriteLine($"<------------------ end   consumer {pushKey} ------------------>");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"【任务处理异常】，异常原因:{ex.Message},当前时间为:{DateTime.Now}");
                RabbitMQHelper.SendMsg(_exchangeName, _queueName, message);
            }
            finally
            {
                Console.WriteLine($"<------------------ end MQ  Product ------------------>");
            }
        }
    }

}
