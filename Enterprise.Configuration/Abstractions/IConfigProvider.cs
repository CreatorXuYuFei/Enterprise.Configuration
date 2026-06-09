using System.Collections.Concurrent;

namespace Enterprise.Configuration.Abstractions
{
    /// <summary>配置供给器：真正加载、存储配置数据，支持热重载</summary>
    public interface IConfigProvider : IDisposable
    {
        ConcurrentDictionary<string, string?> Data { get; }
        void Load();
        event Action Reloaded;
    }
}
