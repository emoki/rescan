using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REScan.Data
{
    /* C++ format
     * 
      	struct basic_technology_data
	    {
		    scanner_id_type scanner_id_;
		    int64_t CollectionRound;
		    double carrier_signal_level_;
		    uint32_t carrier_frequency_;
		    int32_t carrier_bandwidth_;
		    int64_t time_;
		    int32_t status_flags_;
	    };
        struct das1 : scanner_api::basic_technology_data
        {
	        int64_t nanoseconds_since_start_of_day_;
	        double latitude_;
	        double longitude_;
	        int32_t group_code_;
	        double ecio_;
	        double rscp_;
	        double rms_signal_;
	        double rms_corr_;
	        double norm_corr_;
        };
     * 
     * ASCII Representation version 1
     * NANOSECONDS	Latitude	Longitude	TxCode	ECIO	RxLevel	RMS_SIGNAL	RMS_CORR	NORM_CORR	ScannerID	COLLECTION_ROUND	CARRIER_SIGNAL_LEVEL	CARRIER_FREQUENCY	CARRIER_BANDWIDTH	TimeInSec	STATUS_FLAGS
     * 
     */

    public class Das : Measurement
    {
        public Das() {
            TransmitterCode = "";
            Nanoseconds = long.MinValue;
            Ecio = double.NaN;
            TransmitterSignalLevel = double.NaN;
        }
        public override bool Uninitialized() {
            return base.Uninitialized() ||
                Utility.Uninitialized(TransmitterCode) ||
                Utility.Uninitialized(Nanoseconds) ||
                Utility.Uninitialized(Ecio) ||
                Utility.Uninitialized(TransmitterSignalLevel);
        }
        public string TransmitterCode; // Corresponds to group_code.
        public long Nanoseconds;
        public double Ecio;
        public double TransmitterSignalLevel;  // Corresponds to rscp. 
    }
}
