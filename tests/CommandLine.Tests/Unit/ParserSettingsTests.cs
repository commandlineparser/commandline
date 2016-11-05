using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace CommandLine.Tests.Unit
{
    public class ParserSettingsTests
    {
        public class DisposeTrackingStringWriter : StringWriter
        {
            public DisposeTrackingStringWriter()
            {
                Disposed = false;
            }

            public bool Disposed { get; private set; }

            protected override void Dispose(bool disposing)
            {
                Disposed = true;
                base.Dispose(disposing);
            }
        }

        [Fact]
        public void Disposal_does_not_dispose_HelpWriter()
        {
            using (DisposeTrackingStringWriter textWriter = new DisposeTrackingStringWriter())
            {
                using (ParserSettings parserSettings = new ParserSettings())
                {
                    parserSettings.HelpWriter = textWriter;
                }

                textWriter.Disposed.Should().BeFalse("not disposed");
            }
        }
    }
}
