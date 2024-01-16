using SqlSugar;

namespace SqlSugar2
{
    public interface IDBContext
    {
        /// <summary>
        /// 默认数据源
        /// </summary>
        ISqlSugarClient Client { get; }

        /// <summary>
        /// 获取数据源
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        ISqlSugarClient GetClient(string name);
    }
}
