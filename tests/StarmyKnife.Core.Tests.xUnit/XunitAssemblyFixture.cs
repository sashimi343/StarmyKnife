using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xunit.Abstractions;

[assembly: Xunit.TestFramework("StarmyKnife.Core.Tests.xUnit.XunitAssemblyFixture", "StarmyKnife.Core.Tests.xUnit")]

namespace StarmyKnife.Core.Tests.xUnit
{
    public class XunitAssemblyFixture : Xunit.Sdk.XunitTestFramework
    {
        public XunitAssemblyFixture(IMessageSink messageSink) : base(messageSink)
        {
            // Register Shift-JIS encoding provider
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        public new void Dispose()
        {
            base.Dispose();
        }
    }
}
