using Enterprise.Configuration.Abstractions;

namespace Enterprise.Configuration.Core
{
    public sealed class ConfigBuilder : IConfigBuilder
    {
        private readonly List<IConfigSource> _sources = new();
        public IDictionary<string, object> Properties { get; } = new Dictionary<string, object>();
        public IList<IConfigSource> Sources => _sources;

        public IConfigBuilder AddSource(IConfigSource source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            _sources.Add(source);
            return this;
        }

        public IConfigRoot Build()
        {
            var providers = new List<IConfigProvider>();
            foreach (var source in _sources)
            {
                var provider = source.Build(this);
                provider.Load();
                providers.Add(provider);
            }
            return new ConfigRoot(providers);
        }
    }
}
