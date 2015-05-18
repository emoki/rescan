using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using REScan.IO;
using REScan.Data;
using Xunit;

namespace REScan.IO.Tests
{
    public class GsmIOTests
    {
        [Fact]
        public void VerifyParameters() {
            var io = new GsmIO();
            Assert.Equal(io.DataType(), "GSM");
            Assert.Equal("wnd", io.Extension());
        }

        [Fact]
        public void VerifyFileRead() {

            var io = new GsmIO();
            var fileName = "../../../../test_files/test_io.wnd";
            var gsmList = io.ReadFile(fileName);

            Assert.Equal(966, gsmList.Count);

            // Verify first row.
            var row1 = gsmList[0];
            Assert.Equal(1, row1.CollectionRound);
            Assert.Equal(879400000, row1.Frequency);
            Assert.Equal(179, row1.Channel);
            Assert.Equal(99, row1.Bsic);
            Assert.Equal(308485487234514816, row1.Microframes);
            Assert.Equal(-71, row1.SignalLevel, 0);
            Assert.Equal(1.706740, row1.CToI, 5);
            Assert.Equal("10017", row1.ScannerID);
            Assert.Equal(-73.994873, row1.Lon, 5);
            Assert.Equal(40.717529, row1.Lat, 5);
            Assert.True(Data.Utility.Uninitialized(row1.Height));
            Assert.True(Data.Utility.Uninitialized(row1.Height));
            Assert.Equal((long)Math.Round(row1.Microframes / (5200e6 / 24)), row1.Time);
            Assert.True(Data.Utility.Uninitialized(row1.Height));
            Assert.Equal(200000, row1.CarrierBandwidth);
            Assert.Equal(true, row1.IsGpsLocked);
            // TODO - Verify our method for calculating CarrierSL is correct.
            //Assert.Equal(, row1.CarrierSignalLevel);

            // Verify last row.
            var rowEnd = gsmList[gsmList.Count - 1];
            Assert.Equal(31, rowEnd.CollectionRound);
            Assert.Equal(879800000, rowEnd.Frequency);
            Assert.Equal(181, rowEnd.Channel);
            Assert.Equal(64, rowEnd.Bsic);
            Assert.Equal(308485489311691904, rowEnd.Microframes);
            Assert.Equal(-74, rowEnd.SignalLevel, 0);
            Assert.Equal(9.234529, rowEnd.CToI, 5);
            Assert.Equal("10017", rowEnd.ScannerID);
            Assert.Equal(-73.994888, rowEnd.Lon, 5);
            Assert.Equal(40.717541, rowEnd.Lat, 5);
            Assert.True(Data.Utility.Uninitialized(rowEnd.PixelX));
            Assert.True(Data.Utility.Uninitialized(rowEnd.PixelY));
            Assert.Equal((long)Math.Round(rowEnd.Microframes / (5200e6 / 24)), rowEnd.Time);
            Assert.True(Data.Utility.Uninitialized(rowEnd.Height));
            Assert.Equal(200000, rowEnd.CarrierBandwidth);
            Assert.Equal(true, rowEnd.IsGpsLocked);
            // TODO - Verify our method for calculating CarrierSL is correct.
            //Assert.Equal(, rowEnd.CarrierSignalLevel);
       }

        [Fact]
        public void VerifyRedeyeFormatOutput() {
            var fileName = "../../../../test_files/test_io.wnd";

            var io = new GsmIO();
            var list = io.ReadFile(fileName);

            var meta = new Meta(fileName);
            io.OutputRedeyeAnalysisFile("../../../../test_files/RedeyeFormat.gsm.wna", list, meta);
        }

        [Fact]
        public void VerifyExceptions() {
            var noFile = "../../../../test_files/not_there";
            var badSize = "../../../../test_files/bad_size.wnd";
            var badHeader = "../../../../test_files/bad_header.txt";
            var io = new GsmIO();
            var exNoFile = Assert.Throws<FileNotFoundException>(() => { io.ReadFile(noFile); });
            //var exBadSize = Assert.Throws<FormatException>(() => { io.ReadFile(badSize); });
            var exbadHeader = Assert.Throws<FormatException>(() => { io.ReadFile(badHeader); });
        }
    }
}
