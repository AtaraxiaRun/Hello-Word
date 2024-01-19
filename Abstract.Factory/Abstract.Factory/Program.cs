using Abstract.Factory;

class Program
{
    static void Main(string[] args)
    {
        ILoggerFactory loggerFactory;
        ILogger logger;

        // 使用控制台日志记录器
        loggerFactory = new ConsoleLoggerFactory();
        logger = loggerFactory.CreateLogger();
        logger.Log("This is a console log.");

        // 使用文件日志记录器
        loggerFactory = new FileLoggerFactory();
        logger = loggerFactory.CreateLogger();
        logger.Log("This is a file log.");
    }
}
