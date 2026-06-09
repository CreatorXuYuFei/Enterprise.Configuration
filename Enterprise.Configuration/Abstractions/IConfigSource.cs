namespace Enterprise.Configuration.Abstractions
{
    /// <summary>配置源：描述配置来源参数</summary>
    public interface IConfigSource
    {
        IConfigProvider Build(IConfigBuilder builder);
    }
}
