namespace Enterprise.Configuration.Abstractions
{
    /// <summary>配置根节点（对外统一读取入口）</summary>
    public interface IConfigRoot
    {
        string? this[string key] { get; }
        IConfigSection GetSection(string path);
        bool TryGetValue(string key, out string? value);
        void ReloadAll();

        /// <summary>获取所有配置源合并后的全量键值</summary>
        Dictionary<string, string?> GetAllFlattenData();

        // 全局配置重载事件（所有配置源重载都会触发） =========
        event Action Reloaded;
    }
}
