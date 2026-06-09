using Enterprise.Configuration.Abstractions;
using System.Collections.Concurrent;

namespace Enterprise.Configuration.Providers.Memory
{
    internal sealed class MemoryConfigProvider(MemoryConfigSource source) : IConfigProvider
    {
        private readonly MemoryConfigSource _source = source;
        private bool _disposed;

        public ConcurrentDictionary<string, string?> Data { get; } = new();
        public event Action? Reloaded;

        public void Load()
        {
            Data.Clear();
            // 加载内存初始数据
            foreach (var kv in _source.InitialData)
            {
                Data[kv.Key] = kv.Value;
            }
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
            _disposed = true;
        }

        ~MemoryConfigProvider() => Dispose(false);
        #endregion
    }
}
