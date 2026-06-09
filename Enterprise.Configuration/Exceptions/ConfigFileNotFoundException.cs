namespace Enterprise.Configuration.Exceptions
{
    /// <summary>配置文件缺失异常</summary>
    public class ConfigFileNotFoundException(string filePath) : ConfigurationException($"配置文件不存在：{filePath}")
    {
        public string FilePath { get; } = filePath;
    }
}
