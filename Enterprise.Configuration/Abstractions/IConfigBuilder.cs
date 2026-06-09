namespace Enterprise.Configuration.Abstractions
{
    /// <summary>配置构建器</summary>
    public interface IConfigBuilder
    {
        IDictionary<string, object> Properties { get; }
        IList<IConfigSource> Sources { get; }
        IConfigBuilder AddSource(IConfigSource source);
        IConfigRoot Build();
    }
}
