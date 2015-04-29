using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using REScan.IO;
using Xunit;

namespace REScan.IO.Tests {
    public class DasIOTests {
        [Fact]
        public void VerifyParameters() {
            var io = new DasIO();
            Assert.Equal(io.DataType(), "DAS");
            Assert.Equal("dmm", io.Extension());
        }

        [Fact]
        public void VerifyFileReadV1() {
            var fileName = "../../../../test_files/test_io_v1.dmm";

            var io = new DasIO();
            var dasList = io.ReadFile(fileName);

            Assert.Equal(1706, dasList.Count);

            // Verify first row.
            var row1 = dasList[0];
            Assert.Equal(1, row1.CollectionRound);
            Assert.Equal(2137500000, row1.Frequency);
            Assert.Equal("99999", row1.TransmitterCode);
            Assert.Equal(71777229556820, row1.Nanoseconds);
            Assert.Equal(-999, row1.Ecio);
            Assert.Equal(-999, row1.TransmitterSignalLevel, 1);
            Assert.Equal(-75.113640440886, row1.CarrierSignalLevel, 1);
            Assert.Equal(4000000, row1.CarrierBandwidth);

            Assert.Equal("13001", row1.ScannerID);
            Assert.Equal(-97.7407531738281, row1.Lon, 5);
            Assert.Equal(30.2816543579102, row1.Lat, 5);
            Assert.True(Data.Utility.Uninitialized(row1.Height));
            Assert.True(Data.Utility.Uninitialized(row1.Height));
            Assert.Equal(1401219909, row1.Time);
            Assert.True(Data.Utility.Uninitialized(row1.Height));
            Assert.Equal(false, row1.IsGpsLocked);

            // Verify last row.
            var rowEnd = dasList[dasList.Count - 1];
            Assert.Equal(723, rowEnd.CollectionRound);
            Assert.Equal(2137500000, rowEnd.Frequency);
            Assert.Equal("235", rowEnd.TransmitterCode);
            Assert.Equal(72225528557025, rowEnd.Nanoseconds);
            Assert.Equal(-11.6488990607976, rowEnd.Ecio);
            Assert.Equal(-97.0531118677734, rowEnd.TransmitterSignalLevel, 1);
            Assert.Equal(-85.4042128069758, rowEnd.CarrierSignalLevel, 1);
            Assert.Equal(4000000, rowEnd.CarrierBandwidth);

            Assert.Equal("13001", rowEnd.ScannerID);
            Assert.Equal(-97.7407531738281, rowEnd.Lon, 5);
            Assert.Equal(30.2816543579102, rowEnd.Lat, 5);
            Assert.True(Data.Utility.Uninitialized(rowEnd.PixelX));
            Assert.True(Data.Utility.Uninitialized(rowEnd.PixelY));
            Assert.Equal(1401220133, rowEnd.Time);
            Assert.True(Data.Utility.Uninitialized(rowEnd.Height));
            Assert.Equal(false, rowEnd.IsGpsLocked);
        }

        [Fact]
        public void VerifyExceptions() {
            var noFile = "../../../../test_files/not_there";
            var badHeader = "../../../../test_files/bad_header.txt";
            var io = new DasIO();
            var exNoFile = Assert.Throws<FileNotFoundException>(() => { io.ReadFile(noFile); });
            var exbadHeader = Assert.Throws<FormatException>(() => { io.ReadFile(badHeader); });
        }
        [Fact]
        public void VerifyBadSizeExceptions() {
            var badSizeV1 = "../../../../test_files/bad_size_v1.dmm";
            var badSizeV2 = "../../../../test_files/bad_size_v2.dmm";
            //var badSizeV3 = "../../../../test_files/bad_size_v3.dmm";
            var io = new DasIO();
            Assert.Throws<FormatException>(() => { io.ReadFile(badSizeV1); });
            Assert.Throws<FormatException>(() => { io.ReadFile(badSizeV2); });
            //Assert.Throws<FormatException>(() => { io.ReadFile(badSizeV3); });
        }
    }
}
