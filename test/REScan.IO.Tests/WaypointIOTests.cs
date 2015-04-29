using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using REScan.IO;
using Xunit;

namespace REScan.IO.Tests {
    public class WaypointIOTests {
        [Fact]
        public void VerifyParameters() {
            var io = new WaypointIO();
            Assert.Equal(io.DataType(), "WAYPOINT");
            Assert.Equal("wpt", io.Extension());
        }

        [Fact]
        public void VerifyFileRead() {
            var fileName = "../../../../test_files/test_io.wpt";

            var io = new WaypointIO();
            var wptList = io.ReadFile(fileName);

            Assert.Equal(48, wptList.Count);

            // Verify first row.
            var row1 = wptList[0];
            Assert.Equal(-97.7407531738281, row1.ScannerLon, 5);
            Assert.Equal(30.2816543579102, row1.ScannerLat, 5);
            Assert.Equal(false, row1.IsGpsLocked);
            Assert.Equal(0, row1.Height);
            Assert.Equal(".jpg", row1.ImageFileName);

            Assert.Equal("13001", row1.ScannerID);
            Assert.Equal(-97.7405254553449, row1.Lon, 5);
            Assert.Equal(30.2823340609158, row1.Lat, 5);
            Assert.Equal(559, row1.PixelX);
            Assert.Equal(249, row1.PixelY);
            Assert.Equal(1401221305, row1.Time);
            Assert.Equal(0, row1.Height);

            // Verify last row.
            var rowEnd = wptList[wptList.Count - 1];
            Assert.Equal(-97.7407531738281, rowEnd.ScannerLon, 5);
            Assert.Equal(30.2816543579102, rowEnd.ScannerLat, 5);
            Assert.Equal(false, rowEnd.IsGpsLocked);
            Assert.Equal(".jpg", rowEnd.ImageFileName);

            Assert.Equal("13001", rowEnd.ScannerID);
            Assert.Equal(-97.7405944635416, rowEnd.Lon, 5);
            Assert.Equal(30.2817372063662, rowEnd.Lat, 5);
            Assert.Equal(541, rowEnd.PixelX);
            Assert.Equal(1500, rowEnd.PixelY);
            Assert.Equal(1401221591, rowEnd.Time);
            Assert.Equal(0, rowEnd.Height);
        }

        [Fact]
        public void VerifyExceptions() {
            var noFile = "../../../../test_files/not_there";
            var badSize = "../../../../test_files/bad_size.wpt";
            var badHeader = "../../../../test_files/bad_header.txt";
            var io = new WaypointIO();
            var exNoFile = Assert.Throws<FileNotFoundException>(() => { io.ReadFile(noFile); });
            var exBadSize = Assert.Throws<FormatException>(() => { io.ReadFile(badSize); });
            var exbadHeader = Assert.Throws<FormatException>(() => { io.ReadFile(badHeader); });
        }
    }
}
