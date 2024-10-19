using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REScan.Data
{
    /* No C++ format
     * 
     * ASCII Representation version 1
     *  Time,Longitude,Latitude,CW Channel,CW RSSI (dB)
     * 
     */
    public class Pctel : Measurement
    {
		public Pctel() {

		}
        public override bool Uninitialized() {
            return base.Uninitialized();
        }

        public int Channel;
    }
}
