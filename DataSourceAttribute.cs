namespace SqlSugar2
{
    /// <summary>
    /// 数据源
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class DataSourceAttribute : Attribute
    {
        /// <summary>
        /// 数据源名称
        /// </summary>
        public string Name { get; private set; }

        public DataSourceAttribute(string name)
        {
            Name = name;
        }
    }
}
