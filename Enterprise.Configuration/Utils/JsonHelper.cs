using System.Collections.Concurrent;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Enterprise.Configuration.Utils
{
    public static class JsonHelper
    {
        public static readonly JsonSerializerOptions DefaultJsonOpts = new()
        {
            ReadCommentHandling = JsonCommentHandling.Skip,
            AllowTrailingCommas = true,
            PropertyNameCaseInsensitive = true,
            NumberHandling = JsonNumberHandling.AllowReadingFromString,
            Converters =
            {
                new JsonStringBoolConverter()
            },
        };

        #region 【原有逻辑 无需修改】JSON 扁平化（JSON → 扁平键值对）
        public static void FlattenJson(JsonElement element, string prefix, ConcurrentDictionary<string, string?> target)
        {
            switch (element.ValueKind)
            {
                case JsonValueKind.Object:
                    foreach (var prop in element.EnumerateObject())
                    {
                        var newKey = string.IsNullOrEmpty(prefix) ? prop.Name : $"{prefix}:{prop.Name}";
                        FlattenJson(prop.Value, newKey, target);
                    }
                    break;
                case JsonValueKind.Array:
                    var index = 0;
                    foreach (var item in element.EnumerateArray())
                    {
                        FlattenJson(item, $"{prefix}:{index++}", target);
                    }
                    break;
                default:
                    target[prefix] = element.ToString();
                    break;
            }
        }
        #endregion

        #region 【重点改造】扁平键值对 → 重建标准 JSON（支持集合/嵌套集合）
        /// <summary>
        /// 根据配置分段路径 + 扁平配置数据，反向重建标准 JSON
        /// 支持：对象、一维数组、对象集合、嵌套集合、多层数组
        /// </summary>
        public static string RebuildJson(string sectionPath, Dictionary<string, string?> allData)
        {
            string cleanSection = sectionPath.Trim(':');
            if (string.IsNullOrEmpty(cleanSection))
                return "{}";

            string matchPrefix = $"{cleanSection}:";
            // 筛选当前分段下的所有子键（大小写不敏感）
            var sectionData = allData
                .Where(kv => kv.Key.StartsWith(matchPrefix, StringComparison.OrdinalIgnoreCase))
                .ToDictionary(
                    kv => kv.Key[matchPrefix.Length..],
                    kv => kv.Value,
                    StringComparer.OrdinalIgnoreCase
                );

            // 无数据：默认返回空对象
            if (sectionData.Count == 0)
                return "{}";

            // 判定：当前节点 根类型 是【数组】还是【对象】
            bool isRootArray = IsRootNodeArray(sectionData.Keys);

            object rootNode;
            if (isRootArray)
            {
                // 根节点 = 数组（对应 List<T> / T[]）
                rootNode = new List<object?>();
                foreach (var (key, value) in sectionData)
                {
                    var keys = key.Split(':', StringSplitOptions.RemoveEmptyEntries);
                    BuildJsonTree((List<object?>)rootNode, keys, 0, value);
                }
            }
            else
            {
                // 根节点 = 对象（普通实体）
                rootNode = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
                foreach (var (key, value) in sectionData)
                {
                    var keys = key.Split(':', StringSplitOptions.RemoveEmptyEntries);
                    BuildJsonTree((Dictionary<string, object?>)rootNode, keys, 0, value);
                }
            }

            return JsonSerializer.Serialize(rootNode, DefaultJsonOpts);
        }

        /// <summary>
        /// 判断当前分段的根节点是否为数组（首段全部是数字索引）
        /// 规则：微软配置规范 数组格式 = 前缀:0、前缀:1、前缀:2...
        /// </summary>
        private static bool IsRootNodeArray(IEnumerable<string> keys)
        {
            foreach (var key in keys)
            {
                var firstSeg = key.Split(':', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
                if (!int.TryParse(firstSeg, out _))
                    return false;
            }
            return true;
        }

        #region 树形构建 - 对象节点（Dictionary）
        /// <summary>
        /// 构建对象节点树形结构（普通实体、对象下的子节点）
        /// </summary>
        private static void BuildJsonTree(Dictionary<string, object?> node, string[] keys, int idx, string? value)
        {
            if (idx >= keys.Length) return;
            string currentKey = keys[idx];
            int nextIdx = idx + 1;

            // 到达最后一级 Key：直接赋值
            if (nextIdx >= keys.Length)
            {
                node[currentKey] = value;
                return;
            }

            // 下一级是数字索引 → 子节点 = 数组
            if (int.TryParse(keys[nextIdx], out _))
            {
                if (!node.TryGetValue(currentKey, out var childObj) || childObj is not List<object?> childArray)
                {
                    childArray = new List<object?>();
                    node[currentKey] = childArray;
                }
                BuildJsonTree(childArray, keys, nextIdx, value);
            }
            // 下一级是普通字符串 → 子节点 = 对象
            else
            {
                if (!node.TryGetValue(currentKey, out var childObj) || childObj is not Dictionary<string, object?> childObjDict)
                {
                    childObjDict = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
                    node[currentKey] = childObjDict;
                }
                BuildJsonTree(childObjDict, keys, nextIdx, value);
            }
        }
        #endregion

        #region 树形构建 - 数组节点（List）【核心修复：支持数组套对象/数组/值】
        /// <summary>
        /// 构建数组节点树形结构（集合、数组、嵌套数组）
        /// 修复：稀疏索引、数组内对象、多层数组嵌套
        /// </summary>
        private static void BuildJsonTree(List<object?> arr, string[] keys, int idx, string? value)
        {
            if (idx >= keys.Length) return;

            // 当前层级一定是数组索引（数字）
            if (!int.TryParse(keys[idx], out int arrayIndex))
                return;

            int nextIdx = idx + 1;
            // 补全稀疏索引（跳过的索引填充 null，保证数组下标连续）
            while (arr.Count <= arrayIndex)
                arr.Add(null);

            // 数组最后一级：直接赋值
            if (nextIdx >= keys.Length)
            {
                arr[arrayIndex] = value;
                return;
            }

            string nextKey = keys[nextIdx];
            // 下一级还是数字 → 当前数组元素 = 子数组
            if (int.TryParse(nextKey, out _))
            {
                if (arr[arrayIndex] is not List<object?> subArray)
                {
                    subArray = new List<object?>();
                    arr[arrayIndex] = subArray;
                }
                BuildJsonTree(subArray, keys, nextIdx, value);
            }
            // 下一级是字符串 → 当前数组元素 = 对象（最常用：对象集合）
            else
            {
                if (arr[arrayIndex] is not Dictionary<string, object?> subObj)
                {
                    subObj = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
                    arr[arrayIndex] = subObj;
                }
                BuildJsonTree(subObj, keys, nextIdx, value);
            }
        }
        #endregion
        #endregion

        #region 【原有逻辑 无需修改】字符串 <=> 布尔 转换器
        public class JsonStringBoolConverter : JsonConverter<bool>
        {
            public override bool Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                if (reader.TokenType == JsonTokenType.True) return true;
                if (reader.TokenType == JsonTokenType.False) return false;

                if (reader.TokenType == JsonTokenType.String)
                {
                    string str = reader.GetString()?.Trim() ?? string.Empty;
                    return str.Equals("true", StringComparison.OrdinalIgnoreCase);
                }
                throw new JsonException($"无法将 {reader.TokenType} 转换为布尔值");
            }

            public override void Write(Utf8JsonWriter writer, bool value, JsonSerializerOptions options)
            {
                writer.WriteStringValue(value.ToString().ToLower());
            }
        }
        #endregion
    }
}