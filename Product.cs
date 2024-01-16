using SqlSugar;

namespace SqlSugar2
{
    [SugarTable("Product")]

    public class Product
    {
        /// <summary>
        /// 主键
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }
        public string Title { get; set; }
    }
}
