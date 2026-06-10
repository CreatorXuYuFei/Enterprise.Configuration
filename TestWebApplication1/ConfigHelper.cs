using Enterprise.Configuration.Abstractions;
using Enterprise.Configuration.Exceptions;

namespace TestWebApplication1
{
    /// <summary>全局静态配置工具（被动读取，零DI注入）</summary>
    public static class ConfigHelper
    {
        // 全局唯一配置根（启动时赋值一次）
        private static IConfigRoot? _configRoot;

        /// <summary>启动时初始化（仅赋值，不做任何绑定）</summary>
        public static void Init(IConfigRoot configRoot)
        {
            _configRoot = configRoot ?? throw new ArgumentNullException(nameof(configRoot));
        }

        #region 通用读取方法（被动触发解析）
        /// <summary>读取单个实体</summary>
        public static T GetConfig<T>(string sectionPath) where T : class, new()
        {
            if (_configRoot == null) throw new InvalidOperationException("配置未初始化");
            return _configRoot.GetSection(sectionPath).BindModel<T>();
        }

        /// <summary>读取集合 List<T></summary>
        public static List<T> GetConfigList<T>(string sectionPath) where T : class, new()
        {
            if (_configRoot == null) throw new InvalidOperationException("配置未初始化");
            return _configRoot.GetSection(sectionPath).BindModel<List<T>>();
        }

        /// <summary>直接读取原始字符串配置</summary>
        public static string? GetValue(string key)
        {
            return _configRoot?[key];
        }

        /// <summary>手动强制重载全量配置（按需调用）</summary>
        public static void ReloadAll()
        {
            _configRoot?.ReloadAll();
        }
        #endregion
    }
}