using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using REScan.Data;
using REScan.Common;

namespace REScan.IO {
    public class LteIO : DataIO<Lte> {
        public override string DataType() {
            return "LTE";
        }
        public override string Extension() {
            return FileUtility.LteExtension();
        }
        protected override string Header() {
            return "lte_raw_1.1.0.10|WIND3GVer1.2";
        }
        protected override bool IsText() {
            return false;
        }
        public override List<Lte> ReadFile(string fileName) {
            var list = base.ReadFile(fileName);
            
            // The bandwidth and antenna port number are two parameters that 
            // are not decoded for every measurement.  Therefore we need to go 
            // thru and populate them.
            PopulateBandwidthAntennaPort(list);

            return list;
        }
        protected void PopulateBandwidthAntennaPort(List<Lte> list) {
            // For each frequency find the measurement with the highest sync quality and 
            // that has the bandwidth and antenna port number decoded.  Use these parameters
            // to populate all the other measurements on that frequency.
            // Note bandwidth and antenna port number are both decoded or neither are decoded!
            var decodedMeasurements = list.FindAll((lte) => !lte.NumAntennaPort.Equals(0));
            decodedMeasurements.Sort((lte1, lte2) => lte1.SyncQuality.CompareTo(lte2.SyncQuality));

            foreach(var lte in list) {
                var decodedLte = decodedMeasurements.Find((tmp) => lte.Frequency.Equals(tmp.Frequency));
                if(decodedLte != null) {
                    lte.CarrierBandwidth = decodedLte.CarrierBandwidth;
                    lte.NumAntennaPort = decodedLte.NumAntennaPort;
                }
            }
        }
        protected override Lte Parse(byte[] binary) {
            if(binary.Length != BinaryStructByteSize())
                throw new FormatException("Cannot parse " + DataType() + " data.  It is not fully constructed.");

            // Read every element within the file structure but ignore elements that we
            // do not currently use.
            var index = 0;
            Lte lte = new Lte();
            lte.Lon = BitConverter.ToSingle(binary, index); index += 4; //float Lon;
            lte.Lat = BitConverter.ToSingle(binary, index); index += 4; //float Lat;
            index += 4; //float DRLon;
            index += 4; //float DRLat;
            index +=4; //float Speed;
            lte.ScannerID = Convert.ToString(BitConverter.ToUInt16(binary, index)); index += 2; //uint16_t HWID;
            lte.CollectionRound = (long)BitConverter.ToUInt32(binary, index); index += 4; //uint32_t RecordNum;
            lte.Channel = BitConverter.ToUInt16(binary, index); index += 2; //uint16_t EARFCN;
            lte.Frequency = BitConverter.ToUInt32(binary, index); index += 4; //uint32_t CarrierFreqHz;
            lte.CarrierSignalLevel = BitConverter.ToSingle(binary, index); index +=4; //float CarrierSL;
            lte.Microframes = (long)BitConverter.ToUInt64(binary, index); index += 8; //uint64_t MicroFramesRX;
            index +=8; //double FrameBoundry;	
            lte.PhysicalCellid = BitConverter.ToUInt16(binary, index); index +=2; //uint16_t PhysicalCellId;
            lte.Rsrp = BitConverter.ToDouble(binary, index); index +=8; //double Rsrp;
            lte.Rsrq = BitConverter.ToDouble(binary, index); index +=8; //double Rsrq;
            lte.Rssi = BitConverter.ToDouble(binary, index); index +=8; //double Rssi;
            index +=4; //unsigned int NumResourceBlocks;
            lte.CyclicPrefix = BitConverter.ToUInt16(binary, index); index += 4; //int32_t CyclicPrefix;
            lte.SyncSignalLevel = BitConverter.ToSingle(binary, index); index +=4; //float SyncSL;
            lte.SyncQuality = BitConverter.ToSingle(binary, index); index +=4; //float SyncQuality;
            index +=1; //uint8_t PschId;
            index +=8; //double PschRMSCorr;
            index +=8; //double PschNormCorr;
            index +=8; //double Psrq;
            index +=1; //uint8_t SschId;
            index +=8; //double SschRMSCorr;	 
            index +=8; //double SschNormCorr;
            index +=8; //double Ssrq;
            index +=4; //unsigned int RsSampleNum;
            index +=4; //unsigned int PschSampleNum;
            index +=4; //unsigned int SschSampleNum;
            lte.CarrierBandwidth = (int)BitConverter.ToUInt32(binary, index); index +=4; //uint32_t CarrierBandwidthHz;
            lte.NumAntennaPort = (int)BitConverter.ToUInt32(binary, index); index += 4; //unsigned int NumAntennaPort;
            index +=2; //uint16_t DelaySpread;
            index += 2; //uint16_t SubcarrierSpacing;
            var statusFlag = BitConverter.ToUInt32(binary, index); index += 4; // uint32_t StatusFlags
            lte.IsGpsLocked = (statusFlag & 0x0001) == 0x0; // bit 0 of status_flags is gps_lock; 0 => gps lock; 1 => NOT gps lock .
            index += 2; //uint16_t MCC;
            index +=2; //uint16_t MNC;
            index +=2; //uint16_t LAC;
            index +=4; //uint32_t CELLID;
      
            // Conversions
            lte.Time = (long)Math.Round(lte.Microframes / (100e6));

            return lte;
        }
        protected override int BinaryStructByteSize() {
            return 182;
        }
        protected override void outputDerivedRedeyeAnalysisHeader(TextWriter writer) {
            writer.Write("\tRSRP\tRSRQ\tRSSI\tSyncSL\tSyncQuality");
        }
        protected override void outputRedEyeAnalysisFormat(TextWriter writer, Lte meas, Meta meta) {
            base.outputRedEyeAnalysisFormat(writer, meas, meta);
            string s = "";
            s += RedeyeDelimiter;
            s += meas.CollectionRound; s += RedeyeDelimiter;
            s += meas.Channel; s += RedeyeDelimiter;
            s += meas.Frequency / 1e6; s += RedeyeDelimiter;
            s += meas.CarrierSignalLevel; s += RedeyeDelimiter;
            s += meas.PhysicalCellid; s += RedeyeDelimiter;
            s += meas.Rsrp; s += RedeyeDelimiter;
            s += meas.Rsrq; s += RedeyeDelimiter;
            s += "l_"; s += meas.PhysicalCellid; s += "_"; s += (meas.Frequency / 1e5).ToString("00000"); s = s.Insert(s.Length - 1, "_"); s += RedeyeDelimiter;
            s += meas.Rsrp; s += RedeyeDelimiter;
            s += meas.Rsrq; s += RedeyeDelimiter;
            s += meas.Rssi; s += RedeyeDelimiter;
            s += meas.SyncSignalLevel; s += RedeyeDelimiter;
            s += meas.SyncQuality; s += RedeyeDelimiter;
            writer.WriteLine(s);
        }

    }
}
