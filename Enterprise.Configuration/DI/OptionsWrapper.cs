using static Enterprise.Configuration.Abstractions.OptionsInterfaces;

namespace Enterprise.Configuration.DI
{
    internal sealed class OptionsWrapper<TOptions> : IOptions<TOptions> where TOptions : class, new()
    {
        public TOptions Value { get; }
        public OptionsWrapper(TOptions value) => Value = value;
    }
}
