using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace OpenBlog.UnitTest.Testing
{
    public class XunitTestLogger<T> : XunitTestLogger, ILogger<T>
    {
        public XunitTestLogger(ITestOutputHelper outputHelper) : base(outputHelper)
        {

        }
    }
}
