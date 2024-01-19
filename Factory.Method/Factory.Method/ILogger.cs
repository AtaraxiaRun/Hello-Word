using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Factory.Method
{
    /// <summary>
    /// 日志打印接口
    /// </summary>
    public interface ILogger
    {
        void Log(string message);
    }

    /// <summary>
    /// 控制台输出日志
    /// </summary>
    public class ConsoleLogger : ILogger
    {
        public void Log(string message)
        {
            Console.WriteLine("ConsoleLogger: " + message);
        }
    }

    /// <summary>
    /// 文件输出日志
    /// </summary>
    public class FileLogger : ILogger
    {
        private string _filePath;

        public FileLogger(string filePath)
        {
            _filePath = filePath;
        }

        public void Log(string message)
        {
            // 将日志写入文件（这里简化为打印到控制台）
            Console.WriteLine("FileLogger: " + message);
        }
    }
}
