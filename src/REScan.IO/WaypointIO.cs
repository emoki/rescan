using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using REScan.Data;
using REScan.Common;

namespace REScan.IO {
    public class WaypointIO : DataIO<Waypoint> {
        public override string DataType() {
            return "WAYPOINT";
        }
        public override string Extension() {
            return FileUtility.WaypointExtension();
        }
        public override string REAnalysisExtension() {
            throw new NotSupportedException(DataType() + " does not support REAnalysis format.");
        }
        protected override string Header() {
            return "point_longitude";
        }
        protected override bool IsText() {
            return true;
        }
        public override List<Waypoint> ReadFile(string fileName) {
            return base.ReadFile(fileName);
        }
        protected override Waypoint Parse(string txt) {
            var row = txt.Split('\t');
            if(row.Count() < 11)
                throw new FormatException("Unable to parse " + DataType() + " file.  Not enough columns.");

            var i = 0;
            Waypoint wpt = new Waypoint();
            wpt.Lon = double.Parse(row[i++]);
            wpt.Lat = double.Parse(row[i++]);
            wpt.PixelX = int.Parse(row[i++]);
            wpt.PixelY = int.Parse(row[i++]);
            wpt.Time = long.Parse(row[i++]);
            wpt.ScannerID = (row[i++]).Trim();
            wpt.ScannerLat = double.Parse(row[i++]);
            wpt.ScannerLon = double.Parse(row[i++]);
            wpt.IsGpsLocked = bool.Parse(row[i++]);
            wpt.ImageFileName = (row[i++]).Trim();
            wpt.Height = int.Parse(row[i++]);

            return wpt;
        }
    }
}
