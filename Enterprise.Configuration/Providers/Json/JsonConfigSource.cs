using Enterprise.Configuration.Abstractions;

namespace Enterprise.Configuration.Providers.Json
{
    public sealed class JsonConfigSource : IConfigSource
    {
        public string FilePath { get; set; } = string.Empty;
        public bool Optional { get; set; }
        public bool ReloadOnChange { get; set; }

        public IConfigProvider Build(IConfigBuilder builder)
        {
            return new JsonConfigProvider(this);
        }
    }
}
