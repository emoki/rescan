using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/* C++ Format - This structure is outputted in binary via the WIND3G dll.
* 
#pragma pack(push, 1)
struct 
{
	unsigned __int32 CollectionRound;
	unsigned __int16 MCC;
	unsigned __int16 MNC;
	unsigned __int16 LAC;
	unsigned __int16 CELLID;
	float Lon;
	float Lat;
	float DRLon;
	float DRLat;
	__int16 SL;
	unsigned __int16 BCCH;
	unsigned __int8 BSIC;
	unsigned __int16 HardwareID;
	unsigned __int64 MicroFramesRX;
	unsigned __int32 TDMAFrameNum;
	float SynchCorr;
	unsigned __int32 StatusFlags;
	float CtoI;

};
#pragma pack(pop)
* 
*/

namespace REScan.Data
{
    public class Gsm : Measurement
    {
        public Gsm() {
            Channel = int.MinValue;
            Bsic = int.MinValue;
            Microframes = long.MinValue;
            SignalLevel = double.NaN;
            CToI = double.NaN;

        }
        public override bool Uninitialized() {
            return base.Uninitialized() ||
                Utility.Uninitialized(Channel) ||
                Utility.Uninitialized(Bsic) ||
                Utility.Uninitialized(Microframes) ||
                Utility.Uninitialized(SignalLevel) ||
                Utility.Uninitialized(CToI);

        }
        public int Channel;
        public int Bsic;
        public long Microframes;
        public double SignalLevel;
        public double CToI;
    }
}
