using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REScan.Data
{
    /*
     * ASCII Representation version 1
     * point_longitude	 point_latitude	 pixelx	 pixely	 Time	 scannerID	 Scanner_latitude	 Scanner_longitude	 GPSlock	 file	 floor
     */
    public class Waypoint : Coordinate
    {
        public Waypoint() {
            ScannerLon = double.NaN;
            ScannerLat =  double.NaN;
            ImageFileName = "";
        }
        public override bool Uninitialized() {
            return base.Uninitialized() ||
                Utility.Uninitialized(ScannerLon) ||
                Utility.Uninitialized(ScannerLat) ||
                Utility.Uninitialized(ImageFileName);
        }
        public double ScannerLon;
        public double ScannerLat;
        public string ImageFileName;
    }
}
