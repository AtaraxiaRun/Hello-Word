using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abstract.Factory
{
    public interface ILoggerFactory
    {
        ILogger CreateLogger();
    }

    public class ConsoleLoggerFactory : ILoggerFactory
    {
        public ILogger CreateLogger()
        {
            // 返回 ConsoleLogger 实例
            return new ConsoleLogger();
        }
    }

    public class FileLoggerFactory : ILoggerFactory
    {
        public ILogger CreateLogger()
        {
            // 返回 FileLogger 实例
            return new FileLogger();
        }
    }

}
