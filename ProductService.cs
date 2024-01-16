
using Microsoft.AspNetCore.Http.HttpResults;

namespace SqlSugar2
{
    [DataSource("Db")]
    public class ProductService : BaseService<Product>, IProductService
    {
        public ProductService(IServiceProvider serviceProvider) : base(serviceProvider)
        {

        }

        public Product GetProductById(int id)
        {
            var product = GetById(id);
            return product;
        }

        public List<Product> GetProducts()
        {
            var products = DBClient.Queryable<Product>().ToList();

            return products;
        }
    }
}
