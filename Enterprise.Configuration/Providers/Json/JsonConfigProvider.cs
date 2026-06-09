using Enterprise.Configuration.Abstractions;
using Enterprise.Configuration.Exceptions;
using Enterprise.Configuration.Utils;
using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;

namespace Enterprise.Configuration.Providers.Json
{
    internal sealed class JsonConfigProvider : IConfigProvider
    {
        private readonly JsonConfigSource _source;
        private FileSystemWatcher? _watcher;
        private bool _disposed;

        public ConcurrentDictionary<string, string?> Data { get; } = new();
        public event Action? Reloaded;

        public JsonConfigProvider(JsonConfigSource source)
        {
            _source = source;
            if (_source.ReloadOnChange)
                StartFileWatch();
        }

        public void Load()
        {
            Data.Clear();
            var fullPath = Path.GetFullPath(_source.FilePath);

            if (!File.Exists(fullPath))
            {
                if (!_source.Optional)
                    throw new ConfigFileNotFoundException(fullPath);
                return;
            }

            // 统一 UTF8 编码，跨平台不乱码
            var jsonText = File.ReadAllText(fullPath, Encoding.UTF8);
            var doc = JsonSerializer.Deserialize<JsonElement>(jsonText, JsonHelper.DefaultJsonOpts);
            JsonHelper.FlattenJson(doc, string.Empty, Data);
        }

        private void StartFileWatch()
        {
            var dir = Path.GetDirectoryName(Path.GetFullPath(_source.FilePath))!;
            var fileName = Path.GetFileName(_source.FilePath);

            _watcher = new FileSystemWatcher(dir, fileName)
            {
                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size,
                EnableRaisingEvents = true,
                IncludeSubdirectories = false
            };
            _watcher.Changed += OnFileChanged;
        }

        // 防抖：避免多次触发加载
        private void OnFileChanged(object sender, FileSystemEventArgs e)
        {
            DebounceHelper.Debounce(() =>
            {
                Load();
                Reloaded?.Invoke();
            });
        }

        #region IDisposable
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
                _watcher?.Dispose();
                _watcher = null;
            }
            _disposed = true;
        }

        ~JsonConfigProvider() => Dispose(false);
        #endregion
    }
}
