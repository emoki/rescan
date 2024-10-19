using System;using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using REScan.IO;
using REScan.Data;
using Xunit;

namespace REScan.IO.Tests {
    public class PctellIOTests {
        [Fact]
        public void VerifyParameters() {
            var io = new PctelIO();
            Assert.Equal(io.DataType(), "Pctel");
            Assert.Equal("pctel.csv", io.Extension());
        }

        [Fact]
        public void VerifyFileRead() {
            var fileName = "../../../../test_files/test_io.pctel.csv";

            var io = new PctelIO();
            var pctelList = io.ReadFile(fileName);

            Assert.Equal(15, pctelList.Count);

            // Verify first row.
            var row1 = pctelList[0];
            Assert.True(Data.Utility.Uninitialized(row1.CollectionRound));
            Assert.Equal(-1, row1.Frequency);
            Assert.Equal(43140, row1.Channel);
            Assert.Equal(-114.49, row1.CarrierSignalLevel, 1);
            Assert.Equal(100000, row1.CarrierBandwidth);

            Assert.Equal("", row1.ScannerID);
            Assert.Equal(-121.969073, row1.Lon, 5);
            Assert.Equal(37.403948, row1.Lat, 5);
            Assert.True(Data.Utility.Uninitialized(row1.PixelX));
            Assert.Equal(1717700090, row1.Time); 
            Assert.True(Data.Utility.Uninitialized(row1.Height));
            Assert.Equal(true, row1.IsGpsLocked);

            // Verify last row.
            var rowEnd = pctelList[pctelList.Count - 1];
            Assert.True(Data.Utility.Uninitialized(rowEnd.CollectionRound));
            Assert.Equal(-1, rowEnd.Frequency);
            Assert.Equal(42040, rowEnd.Channel);
            Assert.Equal(-88.49, rowEnd.CarrierSignalLevel, 1);
            Assert.Equal(100000, rowEnd.CarrierBandwidth);

            Assert.Equal("", rowEnd.ScannerID);
            Assert.Equal(-121.968347, rowEnd.Lon, 5);
            Assert.Equal(37.403483, rowEnd.Lat, 5);
            Assert.True(Data.Utility.Uninitialized(rowEnd.PixelX));
            Assert.True(Data.Utility.Uninitialized(rowEnd.PixelY));
            Assert.Equal(1717700880, rowEnd.Time); 
            Assert.True(Data.Utility.Uninitialized(rowEnd.Height));
            Assert.Equal(true, rowEnd.IsGpsLocked);
        }

        [Fact]
        public void VerifyRedeyeFormatOutput() {
            var fileName = "../../../../test_files/test_io.pctel.csv";

            var io = new PctelIO();
            var list = io.ReadFile(fileName);

            var meta = new Meta(fileName);
            io.OutputRedeyeAnalysisFile("../../../../test_files/RedeyeFormat.pctel.wna", list, meta);
        }
        
        [Fact]
        public void VerifyExceptions() {
            var noFile = "../../../../test_files/not_there";
            var badSize = "../../../../test_files/bad_size.wnu";
            var badHeader = "../../../../test_files/bad_header.txt";
            var io = new PctelIO();
            var exNoFile = Assert.Throws<FileNotFoundException>(() => { io.ReadFile(noFile); });
            //var exBadSize = Assert.Throws<FormatException>(() => { io.ReadFile(badSize); });
            var exbadHeader = Assert.Throws<FormatException>(() => { io.ReadFile(badHeader); });
        }
    }
}
