namespace Enterprise.Configuration.Utils
{
    public static class DebounceHelper
    {
        private static Timer? _timer;
        private static Action? _lastAction;

        /// <summary>防抖执行（默认 200ms）</summary>
        public static void Debounce(Action action, int delayMs = 200)
        {
            _lastAction = action;
            _timer?.Dispose();
            _timer = new Timer(_ => _lastAction?.Invoke(), null, delayMs, Timeout.Infinite);
        }
    }
}
