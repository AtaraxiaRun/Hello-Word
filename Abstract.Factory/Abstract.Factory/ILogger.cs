using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abstract.Factory
{
    public interface ILogger
    {
        void Log(string message);
    }

    public class ConsoleLogger : ILogger
    {
        public void Log(string message)
        {
            Console.WriteLine("ConsoleLogger: " + message);
        }
    }

    public class FileLogger : ILogger
    {
        public void Log(string message)
        {
            Console.WriteLine("FileLogger: " + message);
            // 实现写入文件的代码
        }
    }

    /// <summary>
    /// 新增数据库日志
    /// </summary>
    public class DataBaseLogger : ILogger
    {
        public void Log(string message)
        {
            Console.WriteLine("DataBaseLogger: " + message);
            // 实现写入文件的代码
        }
    }
}
