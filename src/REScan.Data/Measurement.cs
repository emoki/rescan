using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REScan.Data {
    public class Measurement :  Coordinate {
        public Measurement() {
            CollectionRound = long.MinValue;
            Frequency = long.MinValue;
            CarrierSignalLevel = double.NaN;
            CarrierBandwidth = int.MinValue;
            IsInterpolated = false;
        }
        public override bool Uninitialized() {
            return base.Uninitialized() ||
                        Utility.Uninitialized(CollectionRound) ||
                        Utility.Uninitialized(Frequency) ||
                        Utility.Uninitialized(CarrierSignalLevel) ||
                        Utility.Uninitialized(CarrierBandwidth);
        }

        public long CollectionRound;
        public long Frequency;
        public double CarrierSignalLevel;
        public int CarrierBandwidth;
        public bool IsInterpolated;
    }
}
