using Enterprise.Configuration.Abstractions;

namespace Enterprise.Configuration.Core
{
    public sealed class ConfigSection(IConfigRoot root, string path) : IConfigSection
    {
        // 私有字段，保留封装
        private readonly IConfigRoot _root = root;

        /// <summary>分段路径</summary>
        public string Path { get; } = path.Trim(':', ' ');

        /// <summary>所属根配置对象</summary>
        public IConfigRoot Root => _root;

        //实现接口：Reloaded 事件（仅满足接口契约，不做转发）
        public event Action? Reloaded;

        #region 实现 IConfigRoot 接口全部成员
        public string? this[string subKey]
        {
            get
            {
                // 拼接完整路径：分段路径 + 子键
                string fullKey = $"{Path}:{subKey}";
                return _root[fullKey];
            }
        }

        public IConfigSection GetSection(string path)
        {
            string combinedPath = $"{Path}:{path}";
            return new ConfigSection(_root, combinedPath);
        }

        public bool TryGetValue(string key, out string? value)
        {
            string fullKey = $"{Path}:{key}";
            return _root.TryGetValue(fullKey, out value);
        }

        public void ReloadAll()
        {
            // 分段重载 = 全局配置重载
            _root.ReloadAll();
        }

        /// <summary>获取全量扁平化配置数据（转发给根配置）</summary>
        public Dictionary<string, string?> GetAllFlattenData()
        {
            return _root.GetAllFlattenData();
        }
        #endregion
    }
}
