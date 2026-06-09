using Enterprise.Configuration.Abstractions;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;

namespace Enterprise.Configuration.Providers.CommandLine
{
    internal sealed partial class CommandLineConfigProvider(CommandLineConfigSource source) : IConfigProvider
    {
        private readonly CommandLineConfigSource _source = source;
        private bool _disposed;

        public ConcurrentDictionary<string, string?> Data { get; } = new();
        public event Action? Reloaded;

        public void Load()
        {
            Data.Clear();
            var args = _source.Args;
            if (args == null || args.Length == 0)
                return;

            // 正则匹配：--key=value  /key:value 格式
            var regex = KeyValueRegex();

            for (int i = 0; i < args.Length; i++)
            {
                var arg = args[i].Trim();
                if (string.IsNullOrEmpty(arg)) continue;

                // 格式1: --key=value  /key:value
                var match = regex.Match(arg);
                if (match.Success)
                {
                    string key = match.Groups["key"].Value.Trim();
                    string value = match.Groups["value"].Value.Trim();
                    AddKeyValue(key, value);
                    continue;
                }

                // 格式2: --key 后跟空格值 （--port 8080）
                if (arg.StartsWith("--") && i + 1 < args.Length)
                {
                    string key = arg.TrimStart('-').Trim();
                    string value = args[++i].Trim();
                    AddKeyValue(key, value);
                }
            }
        }

        /// <summary>添加键值，统一格式 + 覆盖控制</summary>
        private void AddKeyValue(string key, string value)
        {
            // 命令行下划线/点 统一转为 冒号，和全局配置规范对齐
            string configKey = key.Replace('_', ':').Replace('.', ':');

            if (_source.AllowOverwrite)
                Data[configKey] = value;
            else
                Data.TryAdd(configKey, value);
        }

        #region IDisposable 标准实现
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                // 手动释放阶段：安全销毁文件监听对象
            }

            // 命令行无文件句柄/非托管资源，仅标记状态
            _disposed = true;
        }

        ~CommandLineConfigProvider() => Dispose(false);

        [GeneratedRegex(@"^(--|/)(?<key>[^=:]+)(=|:)(?<value>.+)$", RegexOptions.Compiled)]
        private static partial Regex KeyValueRegex();
        #endregion
    }
}
