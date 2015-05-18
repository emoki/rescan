using System;using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using REScan.IO;
using REScan.Data;
using Xunit;

namespace REScan.IO.Tests {
    public class WcdmaIOTests {
        [Fact]
        public void VerifyParameters() {
            var io = new WcdmaIO();
            Assert.Equal(io.DataType(), "WCDMA");
            Assert.Equal("wnu", io.Extension());
        }

        [Fact]
        public void VerifyFileRead() {
            var fileName = "../../../../test_files/test_io.wnu";

            var io = new WcdmaIO();
            var wcdmaList = io.ReadFile(fileName);

            Assert.Equal(275, wcdmaList.Count);

            // Verify first row.
            var row1 = wcdmaList[0];
            Assert.Equal(2, row1.CollectionRound);
            Assert.Equal(876800000, row1.Frequency);
            Assert.Equal(4384, row1.Channel);
            Assert.Equal(14, row1.Cpich);
            Assert.Equal(142383801596837756, row1.Microframes);
            Assert.Equal(-64.0, row1.Rscp, 1);
            Assert.Equal(-8.497819, row1.Ecio, 5);
            Assert.Equal(-56.0, row1.CarrierSignalLevel, 1);
            Assert.Equal(5000000, row1.CarrierBandwidth);

            Assert.Equal("10017", row1.ScannerID);
            Assert.Equal(-73.994797, row1.Lon, 5);
            Assert.Equal(40.717537, row1.Lat, 5);
            Assert.True(Data.Utility.Uninitialized(row1.PixelX));
            Assert.True(Data.Utility.Uninitialized(row1.PixelY));
            Assert.Equal((long)Math.Round(row1.Microframes / (100e6)), row1.Time);
            Assert.True(Data.Utility.Uninitialized(row1.Height));
            Assert.Equal(true, row1.IsGpsLocked);

            // Verify last row.
            var rowEnd = wcdmaList[wcdmaList.Count - 1];
            Assert.Equal(52, rowEnd.CollectionRound);
            Assert.Equal(876800000, rowEnd.Frequency);
            Assert.Equal(4384, rowEnd.Channel);
            Assert.Equal(190, rowEnd.Cpich);
            Assert.Equal(142383802664531776, rowEnd.Microframes);
            Assert.Equal(-78.0, rowEnd.Rscp, 1);
            Assert.Equal(-23.296532, rowEnd.Ecio, 5);
            Assert.Equal(-55.0, rowEnd.CarrierSignalLevel, 1);
            Assert.Equal(5000000, rowEnd.CarrierBandwidth);

            Assert.Equal("10017", rowEnd.ScannerID);
            Assert.Equal(-73.994789, rowEnd.Lon, 5);
            Assert.Equal(40.717514, rowEnd.Lat, 5);
            Assert.True(Data.Utility.Uninitialized(rowEnd.Height));
            Assert.True(Data.Utility.Uninitialized(rowEnd.Height));
            Assert.Equal((long)Math.Round(rowEnd.Microframes / (100e6)), rowEnd.Time);
            Assert.True(Data.Utility.Uninitialized(rowEnd.Height));
            Assert.Equal(true, rowEnd.IsGpsLocked);
        }

        [Fact]
        public void VerifyRedeyeFormatOutput() {
            var fileName = "../../../../test_files/test_io.wnu";

            var io = new WcdmaIO();
            var list = io.ReadFile(fileName);

            var meta = new Meta(fileName);
            io.OutputRedeyeAnalysisFile("../../../../test_files/RedeyeFormat.wdma.wna", list, meta);
        }
        
        [Fact]
        public void VerifyExceptions() {
            var noFile = "../../../../test_files/not_there";
            var badSize = "../../../../test_files/bad_size.wnu";
            var badHeader = "../../../../test_files/bad_header.txt";
            var io = new WcdmaIO();
            var exNoFile = Assert.Throws<FileNotFoundException>(() => { io.ReadFile(noFile); });
            //var exBadSize = Assert.Throws<FormatException>(() => { io.ReadFile(badSize); });
            var exbadHeader = Assert.Throws<FormatException>(() => { io.ReadFile(badHeader); });
        }
    }
}
