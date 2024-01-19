using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Factory.Method
{
    public class LoggerFactory
    {
        /// <summary>
        /// 创建日志打印工厂
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public ILogger CreateLogger(string type)
        {
            if (type.Equals("Console"))
            {
                return new ConsoleLogger();
            }
            else if (type.Equals("File"))
            {
                return new FileLogger();
            }
            else
            {
                throw new Exception("Invalid logger type");
            }
        }

        public ILogger CreateConsoleLogger()
        {
            return new ConsoleLogger();
        }

        public ILogger CreateFileLogger()
        {
            return new FileLogger();
        }
    }
}
