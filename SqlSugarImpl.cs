using Microsoft.AspNetCore.SignalR;
using SqlSugar;
using System.Data;
using DbType = SqlSugar.DbType;

namespace SqlSugar2
{
    public class SqlSugarImpl : IDBContext
    {
        private Dictionary<string, ConnectionConfig> _connectionConfig { get; }

        public SqlSugarImpl(IConfiguration config)
        {
            //数据库配置节点
            var section = config.GetSection("DataSource");

            _connectionConfig = section.GetChildren().ToDictionary(
                s => s.Key,
                s => new ConnectionConfig()
                {

                    ConnectionString = s.Value ?? section[$"{s.Key}:Value"], //必填, 数据库连接字符串
                    DbType = DbType.MySql, //必填, 数据库类型：默认sqlserver
                    IsAutoCloseConnection = true, //默认false, 时候知道关闭数据库连接, 设置为true无需使用using或者Close操作
                    InitKeyType = InitKeyType.Attribute, //默认SystemTable, 字段信息读取, 如：该属性是不是主键，是不是标识列等等信息
                    //IsShardSameThread = true, //设为true相同线程是同一个SqlSugarClient
                    MoreSettings = new ConnMoreSettings { IsWithNoLockQuery = false } //nolock
                }
            );
        }

        /// <summary>
        /// 默认数据源
        /// </summary>
        ISqlSugarClient IDBContext.Client { get => GetClient(string.Empty); }

        /// <summary>
        /// 获取数据源
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public ISqlSugarClient GetClient(string name)
        {
            var key = string.Format("{0}ConnectionString", name);
            var db = new SqlSugarClient(_connectionConfig[key]);
            return db;
        }

    }
}
