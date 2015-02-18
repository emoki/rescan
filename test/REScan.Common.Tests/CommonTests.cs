using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using REScan.Common;
using Xunit;

namespace REScan.Common.Tests
{
    public class ConversionsTests
    {
        [Fact]
        public void TestDbmToWatts() {
            Assert.Equal(5.0118723363e-8, Conversions.DbmToWatts(-43), 8);
        }
        public void TestWattsToDbm() {
            Assert.Equal(43.0103, Conversions.WattsToDbm(20), 4);
        }
    }
}
