using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Text;
using Xunit.Abstractions;

namespace OpenBlog.MongoRepositoryTest.Testing
{
    public class XunitLoggerFactory : ILoggerFactory
    {
        private ITestOutputHelper _outputHelper;

        public XunitLoggerFactory(ITestOutputHelper outputHelper)
        {
            _outputHelper = outputHelper;
        }

        public void AddProvider(ILoggerProvider provider)
        {

        }

        public ILogger CreateLogger(string categoryName)
        {
            return new XunitTestLogger(_outputHelper);
        }

        public void Dispose()
        {

        }
    }
}
