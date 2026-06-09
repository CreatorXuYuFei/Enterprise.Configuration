using Enterprise.Configuration.Abstractions;

namespace Enterprise.Configuration.Core
{
    internal sealed class ConfigRoot : IConfigRoot
    {
        public ConfigRoot(List<IConfigProvider> providers)
        {
            _providers = providers;

            // 构造时：订阅每一个 Provider 的重载事件，统一转发到根事件
            foreach (var provider in _providers)
            {
                provider.Reloaded += OnProviderReloaded;
            }
        }

        // 私有集合
        private readonly List<IConfigProvider> _providers;

        // 单个配置源重载 → 触发全局重载事件
        private void OnProviderReloaded()
        {
            Reloaded?.Invoke();
        }

        // 实现接口：全局重载事件
        public event Action? Reloaded;

        public string? this[string key]
        {
            get
            {
                foreach (var provider in _providers.AsEnumerable().Reverse())
                {
                    if (provider.Data.TryGetValue(key, out var val))
                        return val;
                }
                return null;
            }
        }

        public bool TryGetValue(string key, out string? value)
        {
            value = this[key];
            return value != null;
        }

        public IConfigSection GetSection(string path)
        {
            return new ConfigSection(this, path.Trim(':', ' '));
        }

        public void ReloadAll()
        {
            foreach (var provider in _providers)
            {
                provider.Load();
            }
        }

        // 合并全量配置数据
        public Dictionary<string, string?> GetAllFlattenData()
        {
            var allData = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
            // 倒序遍历：后添加的配置源优先级更高，覆盖前面数据
            foreach (var provider in _providers.AsEnumerable().Reverse())
            {
                foreach (var kv in provider.Data)
                {
                    allData[kv.Key] = kv.Value;
                }
            }
            return allData;
        }
    }
}
