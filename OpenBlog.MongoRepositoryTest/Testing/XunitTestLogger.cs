using Microsoft.Extensions.Logging;
using System;
using Xunit.Abstractions;

namespace OpenBlog.MongoRepositoryTest.Testing
{
    public class XunitTestLogger : ILogger
    {
        private ITestOutputHelper _outputHelper;

        public XunitTestLogger(ITestOutputHelper outputHelper)
        {
            _outputHelper = outputHelper;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            _outputHelper.WriteLine($"{DateTime.Now.ToString("HH:mm:ss:ffff")} [{logLevel}] {formatter(state, exception)}");
        }
    }
}
