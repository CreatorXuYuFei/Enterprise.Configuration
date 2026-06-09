using Enterprise.Configuration.Abstractions;
using Enterprise.Configuration.Utils;
using System.Text.Json;

namespace Enterprise.Configuration.Exceptions
{
    public static class ModelBindExtensions
    {
        /// <summary>配置分段绑定实体（纯公有接口调用，无私有字段访问）</summary>
        public static TOptions BindModel<TOptions>(this IConfigSection section) where TOptions : class, new()
        {
            // 1. 通过公有 Root 属性拿到根配置
            IConfigRoot root = section.Root;
            // 2. 通过公有方法拿到【已按优先级合并完成】的全量配置
            Dictionary<string, string?> allData = root.GetAllFlattenData();

            // 3. 清洗分段路径
            string cleanSectionPath = section.Path.Trim(':');
            // 4. 反向重建 JSON 并反序列化为实体
            string json = JsonHelper.RebuildJson(cleanSectionPath, allData);
            TOptions? model = JsonSerializer.Deserialize<TOptions>(json, JsonHelper.DefaultJsonOpts);

            // 兜底：反序列化失败返回空实例
            return model ?? new TOptions();
        }
    }
}
