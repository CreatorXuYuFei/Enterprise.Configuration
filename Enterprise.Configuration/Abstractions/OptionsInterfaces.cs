namespace Enterprise.Configuration.Abstractions
{
    public class OptionsInterfaces
    {
        /// <summary>基础配置选项（启动只读）</summary>
        public interface IOptions<out TOptions> where TOptions : class, new()
        {
            TOptions Value { get; }
        }

        /// <summary>请求级快照（每次读取刷新）</summary>
        public interface IOptionsSnapshot<out TOptions> : IOptions<TOptions> where TOptions : class, new()
        {
            TOptions Get(string? name = null);
        }

        /// <summary>全局监听（配置变更自动刷新）</summary>
        public interface IOptionsMonitor<out TOptions> : IOptionsSnapshot<TOptions> where TOptions : class, new()
        {
            IDisposable OnChange(Action<TOptions, string> listener);
        }
    }
}
