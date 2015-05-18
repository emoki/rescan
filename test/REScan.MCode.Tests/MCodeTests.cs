using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using REScan.MCode;
using REScan.Data;
using Xunit;

namespace REScan.MCode.Tests
{
    public class MCodeTests
    {
        [Fact]
        void TestMeasurementsEmpty() {
            Reset();
            Measurements.Clear();
            Interpolator interpolator = new Interpolator();
            var e = Assert.Throws<ArgumentException>(() => { interpolator.Interpolate(ref Measurements, Waypoints); });
            Assert.Equal("Unable to interpolate.  Measurement list is empty.", e.Message);
        }
        [Fact]
        void TestWaypointsEmpty() {
            Reset();
            Waypoints.Clear();
            Interpolator interpolator = new Interpolator();
            var e = Assert.Throws<ArgumentException>(() => { interpolator.Interpolate(ref Measurements, Waypoints); });
            Assert.Equal("Unable to interpolate.  Waypoint list is empty.", e.Message);
        }
        [Fact]
        void TestWaypointsScannerIDMismatch() {
            Reset();
            Waypoints[0].ScannerID = "badID";
            Interpolator interpolator = new Interpolator();
            var e = Assert.Throws<ArgumentException>(() => { interpolator.Interpolate(ref Measurements, Waypoints); });
            Assert.Equal("Unable to interpolate.  Too many Scanner IDs within waypoints.", e.Message);
        }
        [Fact]
        void TestMeasurementsScannerIDMismatch() {
            Reset();
            Measurements[0].ScannerID = "badID";
            Interpolator interpolator = new Interpolator();
            var e = Assert.Throws<ArgumentException>(() => { interpolator.Interpolate(ref Measurements, Waypoints); });
            Assert.Equal("Unable to interpolate.  Too many Scanner IDs within measurements.", e.Message);
        }
        [Fact]
        void TestImageFileNameMismatch() {
            Reset();
            Waypoints[0].ImageFileName = "badImageFile";
            Interpolator interpolator = new Interpolator();
            var e = Assert.Throws<ArgumentException>(() => { interpolator.Interpolate(ref Measurements, Waypoints); });
            Assert.Equal("The two waypoints used for interpolating the measurement's position do not match image filenames.", e.Message);
        }
        [Fact]
        void TestDasMeasurementRemoval() {
            Reset();
            DasMeasurements[0].CollectionRound = 100;
            DasMeasurements[1].CollectionRound = 100;
            DasMeasurements[2].CollectionRound = 100;
            DasMeasurements[3].CollectionRound = 100;
            DasMeasurements[4].CollectionRound = 100;
            long sum = 0;
            for(int i = 5; i < 10; ++i) {
                sum += DasMeasurements[i].Time;
            }
            var avg = (long)Math.Round(sum / 5.0, 0);

            Interpolator interpolator = new Interpolator();
            interpolator.Interpolate(ref DasMeasurements, Waypoints);

            for(int i = 0; i < 2; ++i) {
                Assert.Equal(avg, DasMeasurements[i].Time);
            }
        }
        [Fact]
        void TestDasTimeBinning() {
            Reset();
            Interpolator interpolator = new Interpolator();
            long sum = 0;
            for(int i = 0; i < 10; ++i) {
                sum += DasMeasurements[i].Time;
            }
            var avg = (long)Math.Round(sum / 10.0, 0);

            interpolator.Interpolate(ref DasMeasurements, Waypoints);

            for(int i = 0; i < 2; ++i) {
                Assert.Equal(avg, DasMeasurements[i].Time);
            }
        }
        [Fact]
        void TestDasEcioRemoval() {   
            Reset();

            DasMeasurements[29].TransmitterCode = "A";

            var size = DasMeasurements.Count;

            Interpolator interpolator = new Interpolator();
            interpolator.Interpolate(ref DasMeasurements, Waypoints);

            Assert.Equal(12, DasMeasurements.Count);

            Assert.Equal("A", DasMeasurements[0].TransmitterCode);
            Assert.Equal(8, DasMeasurements[0].Ecio);
            Assert.Equal("B", DasMeasurements[1].TransmitterCode);
            Assert.Equal(9, DasMeasurements[1].Ecio);
            Assert.Equal("B", DasMeasurements[3].TransmitterCode);
            Assert.Equal(27, DasMeasurements[3].Ecio);
            Assert.Equal("A", DasMeasurements[2].TransmitterCode);
            Assert.Equal(29, DasMeasurements[2].Ecio);
        }
       [Fact]
        void TestInterpolation() {
            Reset();
            Interpolator interpolator = new Interpolator();
            interpolator.Interpolate(ref Measurements, Waypoints);
            int i = 0;
            foreach(var meas in Measurements) {
                Assert.Equal(100 + i, meas.Lat);
                Assert.Equal(50 + i, meas.Lon);
                Assert.Equal(10 + i, meas.PixelX);
                Assert.Equal(200 + i, meas.PixelY);
                Assert.Equal(300 + i, meas.Height);
                ++i;
            }
        }

        void Reset() {
            Measurements = new List<Measurement>();
            Waypoints = new List<Waypoint>();
            DasMeasurements = new List<Das>();
            int cr = 0;
            for(int i = 0; i <= 100; ++i) {
                if(i % 10 == 0)
                    ++cr;
                Measurement meas = new Measurement();
                meas.ScannerID = "test";
                meas.Time = i + 1000;
                meas.CollectionRound = cr;
                meas.CarrierBandwidth = 10000;
                meas.CarrierSignalLevel = -70;
                meas.Frequency = 1960000000;
                Measurements.Add(meas);
            }
            cr = 0;
            var ecio = 0;
            for(int i = 0; i <= 100; ++i) {
                if(i % 10 == 0)
                    ++cr;
                Das meas = new Das();
                meas.ScannerID = "test";
                meas.Time = i + 1000;
                meas.CollectionRound = cr;
                meas.CarrierBandwidth = 10000;
                meas.CarrierSignalLevel = -70;
                meas.Frequency = 1960000000;
                meas.TransmitterCode = i % 2 == 0 ? "A" : "B";
                meas.Ecio = ecio++;

                DasMeasurements.Add(meas);
            }
            for(int i = 0; i <= 10; ++i) {
                Waypoint wpt = new Waypoint();
                wpt.Lat = i * 10 + 100;
                wpt.Lon = i * 10 + 50;
                wpt.PixelX = i * 10 + 10;
                wpt.PixelY = i * 10 + 200;
                wpt.Height = i * 10 + 300;
                wpt.Time = i * 10 + 1000;
                wpt.ScannerID = "test";
                wpt.ImageFileName = "image.jpg";
                Waypoints.Add(wpt);
            }
        }
        List<Measurement> Measurements;
        List<Waypoint> Waypoints;
        List<Das> DasMeasurements;
    }
}
