using Enterprise.Configuration.Abstractions;

namespace Enterprise.Configuration.Providers.Memory
{
    /// <summary>内存配置源（运行时临时键值）</summary>
    public sealed class MemoryConfigSource : IConfigSource
    {
        /// <summary>初始化内存键值集合</summary>
        public Dictionary<string, string?> InitialData { get; set; } = [];

        public IConfigProvider Build(IConfigBuilder builder)
        {
            return new MemoryConfigProvider(this);
        }
    }
}
