using System.Collections.Concurrent;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Enterprise.Configuration.Utils
{
    public static class JsonHelper
    {
        public static readonly JsonSerializerOptions DefaultJsonOpts = new()
        {
            // 原有配置
            ReadCommentHandling = JsonCommentHandling.Skip,    // 跳过 JSON 注释
            AllowTrailingCommas = true,                        // 允许 JSON 最后一个字段带尾逗号
            PropertyNameCaseInsensitive = true,                // 实体属性名大小写不敏感
            // 项目常用，按需开启
            // DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull, // 序列化时忽略 null 字段
            // WriteIndented = true,                                        // 格式化缩进输出（调试用，生产建议关闭）
            // Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,        // 不转义中文/特殊字符

            // ========== 核心：原生支持 字符串 <=> 数字（.NET6+ 专属） ==========
            NumberHandling = JsonNumberHandling.AllowReadingFromString,

            // ========== 转换器集合：补全 字符串 <=> 布尔 转换 ==========
            Converters =
            {
                new JsonStringBoolConverter()
            },
        };


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

        /// <summary>修复：路径清洗 + 不区分大小写匹配</summary>
        public static string RebuildJson(string sectionPath, Dictionary<string, string?> allData)
        {
            // 1. 清洗路径：去除首尾冒号，容错用户传入不规范路径
            string cleanSection = sectionPath.Trim(':');
            if (string.IsNullOrEmpty(cleanSection))
                return "{}";

            // 2. 拼接匹配前缀，不区分大小写查找
            string matchPrefix = $"{cleanSection}:";
            var sectionData = allData
                .Where(kv => kv.Key.StartsWith(matchPrefix, StringComparison.OrdinalIgnoreCase))
                .ToDictionary(
                    kv => kv.Key[matchPrefix.Length..], // 截取前缀后的子Key
                    kv => kv.Value
                );

            // 无匹配数据直接返回空对象
            if (sectionData.Count == 0)
                return "{}";

            var root = new Dictionary<string, object?>();
            foreach (var (key, value) in sectionData)
            {
                BuildJsonTree(root, key.Split(':'), 0, value);
            }
            return JsonSerializer.Serialize(root, DefaultJsonOpts);
        }

        private static void BuildJsonTree(Dictionary<string, object?> node, string[] keys, int idx, string? value)
        {
            if (idx >= keys.Length) return;
            var currentKey = keys[idx];
            var nextIdx = idx + 1;

            if (nextIdx >= keys.Length)
            {
                node[currentKey] = value;
                return;
            }

            if (int.TryParse(keys[nextIdx], out _))
            {
                if (!node.TryGetValue(currentKey, out var arrObj) || arrObj is not List<object?> arr)
                {
                    arr = [];
                    node[currentKey] = arr;
                }
                BuildJsonTree(arr, keys, nextIdx, value);
            }
            else
            {
                if (!node.TryGetValue(currentKey, out var childObj) || childObj is not Dictionary<string, object?> child)
                {
                    child = [];
                    node[currentKey] = child;
                }
                BuildJsonTree(child, keys, nextIdx, value);
            }
        }

        private static void BuildJsonTree(List<object?> arr, string[] keys, int idx, string? value)
        {
            if (idx >= keys.Length) return;
            var currentKey = keys[idx];
            var nextIdx = idx + 1;
            var index = int.Parse(currentKey);

            while (arr.Count <= index) arr.Add(null);

            if (nextIdx >= keys.Length)
            {
                arr[index] = value;
                return;
            }

            if (int.TryParse(keys[nextIdx], out _))
            {
                if (arr[index] is not List<object?> subArr)
                {
                    subArr = [];
                    arr[index] = subArr;
                }
                BuildJsonTree(subArr, keys, nextIdx, value);
            }
            else
            {
                if (arr[index] is not Dictionary<string, object?> subObj)
                {
                    subObj = [];
                    arr[index] = subObj;
                }
                BuildJsonTree(subObj, keys, nextIdx, value);
            }
        }

        /// <summary>
        /// 字符串 <=> 布尔 双向转换转换器
        /// 支持："true"/"false" 字符串转 bool，bool 序列化为字符串
        /// 忽略大小写："TRUE" / "False" 均可正常解析
        /// </summary>
        public class JsonStringBoolConverter : JsonConverter<bool>
        {
            public override bool Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                // 场景1：JSON 是布尔值，直接返回
                if (reader.TokenType == JsonTokenType.True) return true;
                if (reader.TokenType == JsonTokenType.False) return false;

                // 场景2：JSON 是字符串，转布尔
                if (reader.TokenType == JsonTokenType.String)
                {
                    string str = reader.GetString()?.Trim() ?? string.Empty;
                    return str.Equals("true", StringComparison.OrdinalIgnoreCase);
                }

                // 非法类型，抛出标准 Json 异常
                throw new JsonException($"无法将 {reader.TokenType} 转换为布尔值");
            }

            public override void Write(Utf8JsonWriter writer, bool value, JsonSerializerOptions options)
            {
                // 序列化时：bool → 字符串（如需保留原生布尔，改为 writer.WriteBooleanValue(value)）
                writer.WriteStringValue(value.ToString().ToLower());
            }
        }

    }
}
