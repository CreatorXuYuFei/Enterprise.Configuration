namespace Enterprise.Configuration.Exceptions
{
    /// <summary>配置框架统一异常</summary>
    public class ConfigurationException : Exception
    {
        public ConfigurationException(string message) : base(message) { }
        public ConfigurationException(string message, Exception innerException) : base(message, innerException) { }
    }
}
