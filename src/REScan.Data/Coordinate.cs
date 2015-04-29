using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REScan.Data
{
    public class Coordinate
    {
        public Coordinate() {
            ScannerID = "";
            Lon = double.NaN;
            Lat = double.NaN;
            PixelX = double.NaN;
            PixelY = double.NaN;
            Time = long.MinValue;
            Height = int.MinValue;
            IsGpsLocked = false;
        }
        public virtual bool Uninitialized() {
            return Utility.Uninitialized(ScannerID) ||
                Utility.Uninitialized(Lon) ||
                Utility.Uninitialized(Lat) ||
                Utility.Uninitialized(PixelX) ||
                Utility.Uninitialized(PixelY) ||
                Utility.Uninitialized(Time) ||
                Utility.Uninitialized(Height);
        }
        public string ScannerID;
        public double Lon;
        public double Lat;
        public double PixelX;
        public double PixelY;
        public long Time;
        public int Height;
        public bool IsGpsLocked;
    }
}


