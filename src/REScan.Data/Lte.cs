using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/* C++ Format - This structure is outputted in binary via the WIND3G dll.
 * 
 * 
#pragma pack(push, 1) 
struct
{
	float Lon;
	float Lat;
	float DRLon;
	float DRLat;
	float Speed;
	uint16_t HWID;
	uint32_t RecordNum;
	uint16_t EARFCN;
	uint32_t CarrierFreqHz;
	float CarrierSL;
	uint64_t MicroFramesRX;
	double FrameBoundry;	
	uint16_t PhysicalCellId;
	double Rsrp;
	double Rsrq;
	double Rssi;
	unsigned int NumResourceBlocks;
	int32_t CyclicPrefix;
	float SyncSL;
	float SyncQuality;
	uint8_t PschId;
	double PschRMSCorr;
	double PschNormCorr;
	double Psrq;
	uint8_t SschId;
	double SschRMSCorr;	 
	double SschNormCorr;
	double Ssrq;
	unsigned int RsSampleNum;
	unsigned int PschSampleNum;
	unsigned int SschSampleNum;
	uint32_t CarrierBandwidthHz;
	unsigned int NumAntennaPort;
	uint16_t DelaySpread;
	uint16_t SubcarrierSpacing;
	uint32_t StatusFlags;
	uint16_t MCC;
	uint16_t MNC;
	uint16_t LAC;
	uint32_t CELLID;
};
#pragma pack(pop) 
*  
*/

namespace REScan.Data
{
    public class Lte : Measurement
    {
        public Lte() {
            Channel = int.MinValue;
            PhysicalCellid = int.MinValue;
            Microframes = long.MinValue;
            SyncSignalLevel = double.NaN;
            SyncQuality = double.NaN;
            Rsrp = double.NaN;
            Rsrq = double.NaN;
            Rssi = double.NaN;
            NumAntennaPort = int.MinValue;
            CyclicPrefix = int.MinValue;
        }
        public override bool Uninitialized() {
            return base.Uninitialized() ||
                        Utility.Uninitialized(Channel) ||
                        Utility.Uninitialized(PhysicalCellid) ||
                        Utility.Uninitialized(Microframes) ||
                        Utility.Uninitialized(SyncSignalLevel) ||
                        Utility.Uninitialized(SyncQuality) ||
                        Utility.Uninitialized(Rsrp) ||
                        Utility.Uninitialized(Rsrq) ||
                        Utility.Uninitialized(Rssi) ||
                        Utility.Uninitialized(NumAntennaPort) ||
                        Utility.Uninitialized(CyclicPrefix);
        }
        public int Channel;
        public int PhysicalCellid;
        public long Microframes;
        public double SyncSignalLevel;
        public double SyncQuality;
        public double Rsrp;
        public double Rsrq;
        public double Rssi;
        public int NumAntennaPort;
        public int CyclicPrefix;
    }
}
