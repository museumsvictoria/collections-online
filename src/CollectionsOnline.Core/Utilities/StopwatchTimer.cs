using System;
using System.Diagnostics;
using NLog;

namespace CollectionsOnline.Core.Utilities
{
    public class StopwatchTimer : IDisposable
    {
        private readonly Stopwatch _stopwatch;
        private readonly string _message;
        private readonly LogLevel _logLevel;
        private readonly Logger _log;

        public StopwatchTimer(string format, Logger log = null, LogLevel logLevel = null, params object[] args)
        {
            _message = string.Format(format, args);
            _stopwatch = Stopwatch.StartNew();            
            _log = log ?? LogManager.GetCurrentClassLogger();
            _logLevel = logLevel ?? LogLevel.Trace;
        }

        public void Dispose()
        {
            _stopwatch.Stop();
            _log.Log(_logLevel, string.Format("{0} = {1} ms", _message, _stopwatch.ElapsedMilliseconds));
        }
    }
}
