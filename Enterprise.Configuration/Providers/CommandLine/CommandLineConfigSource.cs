using Enterprise.Configuration.Abstractions;

namespace Enterprise.Configuration.Providers.CommandLine
{
    /// <summary>命令行配置源</summary>
    public sealed class CommandLineConfigSource : IConfigSource
    {
        /// <summary>应用启动命令行参数数组</summary>
        public string[] Args { get; set; } = Array.Empty<string>();

        /// <summary>是否允许重复Key覆盖</summary>
        public bool AllowOverwrite { get; set; } = true;

        public IConfigProvider Build(IConfigBuilder builder)
        {
            return new CommandLineConfigProvider(this);
        }
    }
}
