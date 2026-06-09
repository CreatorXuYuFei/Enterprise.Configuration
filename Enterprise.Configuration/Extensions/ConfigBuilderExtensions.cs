using Enterprise.Configuration.Abstractions;
using Enterprise.Configuration.Providers.CommandLine;
using Enterprise.Configuration.Providers.Env;
using Enterprise.Configuration.Providers.Json;
using Enterprise.Configuration.Providers.Memory;

namespace Enterprise.Configuration.Extensions
{
    public static class ConfigBuilderExtensions
    {
        #region 原有方法
        public static IConfigBuilder AddJsonFile(this IConfigBuilder builder, string path, bool optional = false, bool reloadOnChange = true)
        {
            return builder.AddSource(new JsonConfigSource
            {
                FilePath = path,
                Optional = optional,
                ReloadOnChange = reloadOnChange
            });
        }

        public static IConfigBuilder AddEnvironmentVariables(this IConfigBuilder builder, string? prefix = null)
        {
            return builder.AddSource(new EnvConfigSource { Prefix = prefix });
        }
        #endregion

        #region 命令行配置
        /// <summary>添加命令行参数配置源</summary>
        public static IConfigBuilder AddCommandLine(this IConfigBuilder builder, string[] args)
        {
            return builder.AddSource(new CommandLineConfigSource
            {
                Args = args,
                AllowOverwrite = true
            });
        }
        #endregion

        #region 内存配置（修复重载Bug）
        /// <summary>添加内存临时配置源（字典形式）</summary>
        public static IConfigBuilder AddMemory(this IConfigBuilder builder, Dictionary<string, string?> data)
        {
            return builder.AddSource(new MemoryConfigSource
            {
                InitialData = data
            });
        }

        /// <summary>重载：快速添加单个键值对</summary>
        public static IConfigBuilder AddMemory(this IConfigBuilder builder, string key, string? value)
        {
            return builder.AddMemory(new Dictionary<string, string?> { { key, value } });
        }
        #endregion
    }
}
