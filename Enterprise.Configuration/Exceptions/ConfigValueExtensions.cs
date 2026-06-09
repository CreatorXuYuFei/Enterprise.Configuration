using Enterprise.Configuration.Abstractions;

namespace Enterprise.Configuration.Exceptions
{
    /// <summary>配置实体校验失败异常</summary>
    public class ConfigValidateException(string message) : ConfigurationException(message)
    {
    }

    public static class ConfigValueExtensions
    {
        public static T? Get<T>(this IConfigRoot root, string key, T? defaultValue = default)
        {
            var str = root[key];
            if (string.IsNullOrEmpty(str)) return defaultValue;

            try
            {
                return (T)Convert.ChangeType(str, typeof(T));
            }
            catch
            {
                return defaultValue;
            }
        }

        public static int GetInt32(this IConfigRoot root, string key, int defaultValue = 0)
            => root.Get(key, defaultValue);

        public static bool GetBoolean(this IConfigRoot root, string key, bool defaultValue = false)
            => root.Get(key, defaultValue);
    }
}
