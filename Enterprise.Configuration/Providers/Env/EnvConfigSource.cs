using Enterprise.Configuration.Abstractions;

namespace Enterprise.Configuration.Providers.Env
{
    public sealed class EnvConfigSource : IConfigSource
    {
        public string? Prefix { get; set; }
        public IConfigProvider Build(IConfigBuilder builder) => new EnvConfigProvider(this);
    }
}
