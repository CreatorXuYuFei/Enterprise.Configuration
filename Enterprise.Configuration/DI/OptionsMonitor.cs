using Enterprise.Configuration.Abstractions;
using Enterprise.Configuration.Core;
using Enterprise.Configuration.Exceptions;
using Enterprise.Configuration.Utils;
using static Enterprise.Configuration.Abstractions.OptionsInterfaces;

namespace Enterprise.Configuration.DI
{
    internal sealed class OptionsMonitor<TOptions> : IOptionsMonitor<TOptions> where TOptions : class, new()
    {
        private readonly IConfigRoot _configRoot;
        private readonly string _section;
        private readonly List<Action<TOptions, string>> _listeners = new();

        public OptionsMonitor(IConfigRoot configRoot, string section)
        {
            _configRoot = configRoot;
            _section = section;

            // 直接订阅根配置的公有全局事件 =========
            _configRoot.Reloaded += OnConfigReloaded;
        }

        public TOptions Value => Get();

        public TOptions Get(string? name = null)
        {
            var section = string.IsNullOrEmpty(name) ? _section : $"{_section}:{name}";
            var model = _configRoot.GetSection(section).BindModel<TOptions>();
            ModelValidator.Validate(model);
            return model;
        }

        public IDisposable OnChange(Action<TOptions, string> listener)
        {
            _listeners.Add(listener);
            return new Unsubscriber(() => _listeners.Remove(listener));
        }

        // 配置全局重载触发后，回调外部监听
        private void OnConfigReloaded()
        {
            var model = Get();
            foreach (var listener in _listeners)
            {
                listener.Invoke(model, string.Empty);
            }
        }

        // 外部回调解绑器
        private sealed class Unsubscriber : IDisposable
        {
            private readonly Action _unsubscribe;
            public Unsubscriber(Action unsubscribe) => _unsubscribe = unsubscribe;
            public void Dispose() => _unsubscribe();
        }
    }
}
