using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using REScan.Data;

namespace REScan.IO {
    public class WcdmaIO : DataIO<Wcdma> {
        public override string DataType() {
            return "WCDMA";
        }
        public override string Extension() {
            return "wnu";
        }
        protected override string Header() {
            return "UmtsVersion3.0.0.10|WIND3GVer1.2";
        }
        protected override bool IsText() {
            return false;
        }
        public override List<Wcdma> ReadFile(string fileName) {
            return base.ReadFile(fileName);
        }
        protected override Wcdma Parse(byte[] binary) {
            if(binary.Length != BinaryStructByteSize())
                throw new FormatException("Cannot parse " + DataType() + " data.  It is not fully constructed.");

            // Read every element within the file structure but ignore elements that we
            // do not currently use.
            var index = 0;
            Wcdma wcdma = new Wcdma();

            wcdma.Lat = BitConverter.ToSingle(binary, index); index += 4; //float Lat;
            wcdma.Lon = BitConverter.ToSingle(binary, index); index += 4; //float Lon;
            index += 4; //float DRLat;
            index += 4; //float DRLon;
            wcdma.ScannerID = Convert.ToString(BitConverter.ToUInt16(binary, index)); index += 2; //uint16_t HWID;
            wcdma.CollectionRound = (long)BitConverter.ToUInt32(binary, index); index += 4; //uint32_t RecordNum;
            wcdma.Channel = BitConverter.ToUInt16(binary, index); index += 2; //uint16_t UARFCN;
            wcdma.Frequency = (long)Math.Round(BitConverter.ToSingle(binary, index) * 10) * 100000; index += 4; //float CarrierFreq;
            wcdma.CarrierSignalLevel = BitConverter.ToSingle(binary, index); index += 4; //float CarrierSL;
            wcdma.Microframes = (long)BitConverter.ToUInt64(binary, index); index += 8; //uint64_t MicroFramesRX;
            index += 8; //uint64_t ChipsRX;
            index += 4; //float PSCH_EcIo;
            wcdma.Cpich = BitConverter.ToUInt16(binary, index); index += 2; //uint16_t CPICH;
            wcdma.Rscp = (sbyte)binary[index]; index += 1; //uint8_t CPICH_RSCP;
            wcdma.Ecio = BitConverter.ToSingle(binary, index); index += 4; //float CPICH_EcIo;
            index += 1; //bool STTD;
            index += 4; //uint32_t StatusFlags;
            index += 2; //uint16_t SystemFrameNumber;
            index += 2; //uint16_t MCC;
            index += 2; //uint16_t MNC;
            index += 2; //uint16_t LAC;
            index += 4; //uint32_t utran_cellid_;


            // Conversions.
            wcdma.Time = (long)Math.Round(wcdma.Microframes / (100e6));

            // Known quantities.
            wcdma.CarrierBandwidth = 5000000;

            return wcdma;
        }
        protected override int BinaryStructByteSize() {
            return 76;
        }
    }
}
