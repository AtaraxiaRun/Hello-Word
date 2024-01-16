namespace SqlSugar2
{
    public interface IProductService
    {
        Product GetProductById(int id);

        List<Product> GetProducts();
    }
}
