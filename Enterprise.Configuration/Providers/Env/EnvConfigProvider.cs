using Enterprise.Configuration.Abstractions;
using System.Collections;
using System.Collections.Concurrent;

namespace Enterprise.Configuration.Providers.Env
{
    internal sealed class EnvConfigProvider(EnvConfigSource source) : IConfigProvider
    {
        private readonly EnvConfigSource _source = source;
        public ConcurrentDictionary<string, string?> Data { get; } = new();
        public event Action? Reloaded;

        public void Load()
        {
            Data.Clear();
            IDictionary envVars = Environment.GetEnvironmentVariables();
            var prefix = _source.Prefix ?? string.Empty;

            foreach (DictionaryEntry item in envVars)
            {
                var key = item.Key.ToString()!;
                if (!string.IsNullOrEmpty(prefix) && !key.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                    continue;

                var configKey = string.IsNullOrEmpty(prefix) ? key : key.Substring(prefix.Length);
                // 环境变量下划线转为冒号，对齐配置规范
                configKey = configKey.Replace("_", ":");
                Data[configKey] = item.Value?.ToString();
            }
        }

        public void Dispose() { }
    }
}
