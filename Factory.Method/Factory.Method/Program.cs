using Factory.Method;

class Program
{
    /// <summary>
    /// 简单工厂模式
    /// </summary>
    /// <param name="args"></param>
    static void Main(string[] args)
    {
        
        LoggerFactory factory = new LoggerFactory();
        //通过不同的入参，创建不同的对象
        ILogger logger = factory.CreateLogger("Console");
        logger.Log("This is a console log.");

        logger = factory.CreateLogger("File");
        logger.Log("This is a file log.");
    }
}