using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using REScan.Data;

namespace REScan.MCode
{
    public class Interpolator
    {
        // Interpolation will assign a position to each measurement using time, based on the waypoint before and after the measurement.
        // Measurements that exist outside the first and last waypoint are removed.  
        // Note - We currently sort both the measurement list and waypoint list.
        public void Interpolate<T>(List<T> measurements, List<Waypoint> waypoints) where T : Measurement {
            if(measurements.Count < 1)
                throw new ArgumentException("Unable to interpolate.  Measurement list is empty.");
            if(waypoints.Count < 1)
                throw new ArgumentException("Unable to interpolate.  Waypoint list is empty.");
            var wptScannerIDs = waypoints.GroupBy(wpt => wpt.ScannerID).Select(wpt => wpt.First()).ToList();
            if(wptScannerIDs.Count() > 1)
                throw new ArgumentException("Unable to interpolate.  Too many Scanner IDs within waypoints.");
            var measScannerIDs = measurements.GroupBy(meas => meas.ScannerID).Select(meas=> meas.First()).ToList();
            if(measScannerIDs.Count() > 1)
                throw new ArgumentException("Unable to interpolate.  Too many Scanner IDs within measurements.");

            measurements.Sort((meas1, meas2) => meas1.Time.CompareTo(meas2.Time));
            waypoints.Sort((wpt1, wpt2) => wpt1.Time.CompareTo(wpt2.Time));

            // Apply bug fix.  Columbus (dascontrol) sometimes outputs old measurements that are cached when starting a new collection file.
            RemoveOldMeasurementsUsingCollectionRound(measurements);

            // We make sure all measurements within X collection rounds have the same time.  This ensures they receive the same interpolated
            // position when interpolating.
            BinTimeUsingCollectionRound(measurements);

            // Fix DAS discrepancy dealing with high / calculated gain.
            RemoveBadEcIos(measurements);

            PerformInterpolation(measurements, waypoints);
        }
        private void RemoveOldMeasurementsUsingCollectionRound<T>(List<T> measurements) where T : Measurement {
            // Bug only affects DAS measurements outputted from Columbus.
             if(measurements[0] is Das) {
                // Collection round (CR) is incremented everytime we have collected on every frequency in the collection list.
                // It should never decrement.  If we find a place where the CR decrements then those measurements are most likely
                // caused by the collection bug specified above.  Remove all buggy measurements.
                var idx = 0;
                var cr = measurements[0].CollectionRound;
                while(idx < measurements.Count) {
                    if(cr <= measurements[idx].CollectionRound) {
                        cr = measurements[idx].CollectionRound;
                        ++idx;
                        continue;
                    }
                    else {
                        // Assume this only happens once.
                        measurements.RemoveRange(0, idx);
                        break;
                    }
                } 
            }
        }
        private void BinTimeUsingCollectionRound<T>(List<T> measurements) where T : Measurement  {
            // Due to our DAS gain algorithm during collection, to properly compare measurements 
            // during analysis we need to group by every 2 collection rounds % 2.
            // So every two collection round average the Time value and assign it to all
            // measurements.  
            if(measurements[0] is Das) {
                var i = 0;
                // Adjust any measurement that appear at that may appear at the beginning
                while(i < measurements.Count) {
                   if(measurements[i].CollectionRound % 2 == 0)
                        break;
                   i = CalculateAverageTime<T>(measurements, i);
                }
                while(i < measurements.Count) {
                    if(measurements[i].CollectionRound % 2 == 0) {
                        i = CalculateAverageTime<T>(measurements, i);
                    }
                    else
                        ++i;
                }
            }
        }
        private static int CalculateAverageTime<T>(List<T> measurements, int i) where T : Measurement {
            int j = i;
            double total = 0;
            long sum = 0;
            do {
                sum += measurements[j].Time;
                ++total;
                if(++j >= measurements.Count)
                    break;
            } while(measurements[i].CollectionRound == measurements[j].CollectionRound || measurements[j].CollectionRound % 2 != 0);

            var averageTime = (long)Math.Round(sum / total, 0);

            for(var k = i; k < j; ++k) {
                measurements[k].Time = averageTime;
            }

            i = j;
            return i;
        }
        public void RemoveBadEcIos<T>(List<T> measurements) where T : Measurement {
            // For the algorithm to work correctly BinTimeUsingCollectionRound must be run first.
            // Due to EcIo discrepancies that can occur in DAS when using Wider's gain algorithm.  We throw away the lowest EcIo for a Tx if
            // the gap is too large.
            if(measurements[0] is Das) {
                List<Das> dasList = measurements.Cast<Das>().ToList();
                var newList = dasList.GroupBy(meas => meas.Time).Select(g => g.GroupBy(meas => meas.TransmitterCode).Select(gp => gp.OrderBy(meas => meas.Ecio).Last())).SelectMany(meas => meas).ToList();
                measurements.RemoveAll(meas => !newList.Exists(d => d == meas));
           } 
        }
        private void PerformInterpolation<T>(List<T> measurements, List<Waypoint> waypoints) where T : Measurement {
            var before = 0;
            var equalOrAfter = 0;

            foreach(var meas in measurements) {
                equalOrAfter = FindEqualUpperBoundWaypoint(meas, waypoints, equalOrAfter);
                    
                if(equalOrAfter == -1)
                    break;

                if(meas.Time == waypoints[equalOrAfter].Time) {
                    AssignPosition(meas, waypoints[equalOrAfter]);
                    continue;
                }

                before = equalOrAfter - 1;

                if(before < 0 || meas.Time < waypoints[before].Time) {
                    continue;
                }

                Debug.Assert(waypoints[before].Time != meas.Time);

                CalculatePosition(meas, waypoints[before], waypoints[equalOrAfter]);
            }
        }
        private int FindEqualUpperBoundWaypoint(Measurement meas, List<Waypoint> waypoints, int idx) {
            while(idx < waypoints.Count && waypoints[idx].Time < meas.Time)
                idx++;

            if(idx >= waypoints.Count)
                return -1;
            else
                return idx;
        }
        private void AssignPosition(Measurement meas, Waypoint waypoint) {
            meas.PixelX = waypoint.PixelX;
            meas.PixelY = waypoint.PixelY;
            meas.Lat = waypoint.Lat;
            meas.Lon = waypoint.Lon;
            meas.Height = waypoint.Height;
            meas.IsInterpolated = true;
        }
        private void CalculatePosition(Measurement meas, Waypoint wpt1, Waypoint wpt2) {
            if(wpt1.ImageFileName != wpt2.ImageFileName)
                throw new ArgumentException("The two waypoints used for interpolating the measurement's position do not match image filenames.");

            double ratio = (double)(meas.Time - wpt1.Time) / (double)(wpt2.Time - wpt1.Time);

            meas.PixelX = wpt1.PixelX + (wpt2.PixelX - wpt1.PixelX) * ratio;
            meas.PixelY = wpt1.PixelY + (wpt2.PixelY - wpt1.PixelY) * ratio;
            meas.Lat = wpt1.Lat + (wpt2.Lat - wpt1.Lat) * ratio;
            meas.Lon = wpt1.Lon + (wpt2.Lon - wpt1.Lon) * ratio;
            meas.Height = (int)Math.Round(wpt1.Height + (wpt2.Height - wpt1.Height) * ratio, 0);
            meas.IsInterpolated = true;
        }
 
    }
}
