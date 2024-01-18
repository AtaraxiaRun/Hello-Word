using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQ2
{
    public class ProductService : IProductService
    {
        public async Task UpdateProduct(Product product)
        {
            Console.WriteLine($"产品【{product.Name}】修改成功");
        }
    }
}
