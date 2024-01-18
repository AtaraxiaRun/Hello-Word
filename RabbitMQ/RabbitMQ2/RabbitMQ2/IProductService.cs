using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQ2
{
    public interface IProductService
    {
        Task UpdateProduct(Product product);
    }
}
