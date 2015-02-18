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
	float Lat;
	float Lon;
	float DRLat;
	float DRLon;
	uint16_t HWID;
	uint32_t RecordNum;
	uint16_t UARFCN;
	float CarrierFreq;
	float CarrierSL;
	uint64_t MicroFramesRX;
	uint64_t ChipsRX;
	float PSCH_EcIo;
	uint16_t CPICH;
	uint8_t CPICH_RSCP;
	float CPICH_EcIo;
	bool STTD;
	uint32_t StatusFlags;
	uint16_t SystemFrameNumber;
	uint16_t MCC;
	uint16_t MNC;
	uint16_t LAC;
	uint32_t utran_cellid_;
};
#pragma pack(pop)
* 
*/

namespace REScan.Data
{
    public class Wcdma : Measurement
    {
        public Wcdma() {
            Channel = int.MinValue;
            Cpich = int.MinValue;
            Microframes = long.MinValue;
            Rscp = double.NaN;
            Ecio = double.NaN;
        }
        public override bool Uninitialized() {
            return base.Uninitialized() ||
                Utility.Uninitialized(Channel) ||
                Utility.Uninitialized(Cpich) ||
                Utility.Uninitialized(Microframes) ||
                Utility.Uninitialized(Rscp) ||
                Utility.Uninitialized(Ecio);
        }
        public int Channel;
        public int Cpich;
        public long Microframes;
        public double Rscp;
        public double Ecio;
    }
}
