using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using REScan.IO;
using Xunit;

namespace REScan.IO.Tests {
    public class LteIOTests
    {
        [Fact]
        public void VerifyParameters() {
            var io = new LteIO();
            Assert.Equal(io.DataType(), "LTE");
            Assert.Equal("wnl", io.Extension());
        }

        [Fact]
        public void VerifyFileRead() {
            var fileName = "../../../../test_files/test_io.wnl";

            var io = new LteIO();
            var lteList = io.ReadFile(fileName);

            Assert.Equal(179, lteList.Count);

            // Verify first row.
            var row1 = lteList[0];
            Assert.Equal(1, row1.CollectionRound);
            Assert.Equal(1935000000, row1.Frequency);
            Assert.Equal(650, row1.Channel);
            Assert.Equal(297, row1.PhysicalCellid);
            Assert.Equal(142383808462551989, row1.Microframes);
            Assert.Equal(-73.0, row1.SyncSignalLevel, 1);
            Assert.Equal(-2.270527, row1.SyncQuality, 5);
            Assert.Equal(-90.0, row1.Rsrp, 1);
            Assert.Equal(-7.29838, row1.Rsrq, 5);
            Assert.Equal(-71.0, row1.Rssi, 1);
            Assert.Equal(-70.0, row1.CarrierSignalLevel, 1);
            Assert.Equal(10000000, row1.CarrierBandwidth);
            Assert.Equal(2, row1.NumAntennaPort);
            Assert.Equal(1, row1.CyclicPrefix);

            Assert.Equal("10017", row1.ScannerID);
            Assert.Equal(-73.994766, row1.Lon, 5);
            Assert.Equal(40.717346, row1.Lat, 5);
            Assert.True(Data.Utility.Uninitialized(row1.Height));
            Assert.True(Data.Utility.Uninitialized(row1.Height));
            Assert.Equal((long)Math.Round(row1.Microframes / (100e6)), row1.Time);
            Assert.True(Data.Utility.Uninitialized(row1.Height));
            Assert.Equal(true, row1.IsGpsLocked);

            // Verify last row.
            var rowEnd = lteList[lteList.Count - 1];
            Assert.Equal(53, rowEnd.CollectionRound);
            Assert.Equal(2138600000, rowEnd.Frequency);
            Assert.Equal(286, rowEnd.Channel);
            Assert.Equal(0, rowEnd.PhysicalCellid);
            Assert.Equal(142383809721384324, rowEnd.Microframes);
            Assert.Equal(-82.0, rowEnd.SyncSignalLevel, 1);
            Assert.Equal(-7.116156, rowEnd.SyncQuality, 5);
            Assert.Equal(-94.0, rowEnd.Rsrp, 1);
            Assert.Equal(-11.43003086, rowEnd.Rsrq, 5);
            Assert.Equal(-71.0, rowEnd.Rssi, 1);
            Assert.Equal(-70.0, rowEnd.CarrierSignalLevel, 1);
            Assert.Equal(15000000, rowEnd.CarrierBandwidth);
            Assert.Equal(2, rowEnd.NumAntennaPort);
            Assert.Equal(1, rowEnd.CyclicPrefix);

            Assert.Equal("10017", rowEnd.ScannerID);
            Assert.Equal(-73.994759, rowEnd.Lon, 5);
            Assert.Equal(40.717068, rowEnd.Lat, 5);
            Assert.True(Data.Utility.Uninitialized(rowEnd.PixelX));
            Assert.True(Data.Utility.Uninitialized(rowEnd.PixelY));
            Assert.Equal((long)Math.Round(rowEnd.Microframes / (100e6)), rowEnd.Time);
            Assert.True(Data.Utility.Uninitialized(rowEnd.Height));
            Assert.Equal(true, rowEnd.IsGpsLocked);
        }
        }

        [Fact]
        public void VerifyExceptions() {
            var noFile = "../../../../test_files/not_there";
            var badSize = "../../../../test_files/bad_size.wnl";
            var badHeader = "../../../../test_files/bad_header.txt";
            var io = new LteIO();
            var exNoFile = Assert.Throws<FileNotFoundException>(() => { io.ReadFile(noFile); });
            var exBadSize = Assert.Throws<FormatException>(() => { io.ReadFile(badSize); });
            var exbadHeader = Assert.Throws<FormatException>(() => { io.ReadFile(badHeader); });
        }
    }
}
