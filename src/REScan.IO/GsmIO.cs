using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using REScan.Data;
using REScan.Common;

namespace REScan.IO {
    public class GsmIO : DataIO<Gsm> {
        public GsmIO() {
            Headers = new Dictionary<Version, string>();
            Headers.Add(Version.Us, "GsmVersion3.2.0.10|WIND3GVer1.2|US");
            Headers.Add(Version.Euro, "GsmVersion3.2.0.10|WIND3GVer1.2|EURO");
            CurrentVersion = Version.Unknown;
        }
        public override string DataType() {
            return "GSM";
        }
        public override string Extension() {
            return "wnd";
        }
        protected override string Header() {
            if(CurrentVersion.Equals(Version.Unknown))
                throw new FormatException("GsmIO version is unknown.");
            return Headers[CurrentVersion];
        }
        protected override bool IsText() {
            return false;
        }
        public override List<Gsm> ReadFile(string fileName) {
            return base.ReadFile(fileName);
        }
        protected override void DetermineHeader(string header) {
            if(header.Contains(Headers[Version.Us]))
                CurrentVersion = Version.Us;
            else if(header.Contains(Headers[Version.Euro]))
                CurrentVersion = Version.Euro;
            else {
                CurrentVersion = Version.Unknown;
                throw new FormatException("Unable to determine GsmIO version.");
            }
        }
        protected override Gsm Parse(byte[] binary) {
            if(binary.Length != BinaryStructByteSize())
                throw new FormatException("Cannot parse " + DataType() + " data.  It is not fully constructed.");

            // Read every element within the file structure but ignore elements that we
            // do not currently use.
            var index = 0;
            Gsm gsm = new Gsm();
            gsm.CollectionRound = (long)BitConverter.ToUInt32(binary, index); index += 4; //unsigned __int32 CollectionRound;
            index += 2; //unsigned __int16 MCC;
            index += 2; //unsigned __int16 MNC;
            index += 2; //unsigned __int16 LAC;
            index += 2; //unsigned __int16 CELLID;
            gsm.Lon = BitConverter.ToSingle(binary, index); index += 4; //float Lon;
            gsm.Lat = BitConverter.ToSingle(binary, index); index += 4; //float Lat;
            index += 4; //float DRLon;
            index += 4; //float DRLat;
            gsm.SignalLevel =  BitConverter.ToInt16(binary, index); index += 2; //__int16 SL;
            gsm.Channel = BitConverter.ToUInt16(binary, index); index += 2; //unsigned __int16 BCCH;
            gsm.Bsic = ((binary[index] >> 4) * 10) + (binary[index] & 0x0F); index += 1; //unsigned __int8 BSIC;
            gsm.ScannerID = Convert.ToString(BitConverter.ToUInt16(binary, index)); index += 2; //unsigned __int16 HardwareID;
            gsm.Microframes = (long)BitConverter.ToUInt64(binary, index); index += 8; //unsigned __int64 MicroFramesRX;
            index += 4; //unsigned __int32 TDMAFrameNum;
            index += 4; //float SynchCorr;
            index += 4; //unsigned __int32 StatusFlags;
            gsm.CToI = BitConverter.ToSingle(binary, index); index += 4; //float CtoI;

            // Conversions.
            gsm.Time = (long)Math.Round(gsm.Microframes / (5200e6 / 24));
            
            // You must first convert to watts before adding signal levels.
            gsm.CarrierSignalLevel = Conversions.WattsToDbm(Conversions.DbmToWatts(gsm.CToI) + Conversions.DbmToWatts(gsm.SignalLevel));

            switch(CurrentVersion) {
                case Version.Us:
                    gsm.Frequency = ConvertUsChannel(gsm.Channel);
                    break;
                case Version.Euro:
                    gsm.Frequency = ConvertEuroChannel(gsm.Channel);
                    break;
                default:
                    throw new InvalidOperationException("GsmIO version is unknown.");
            }

            // Known quantities.
            gsm.CarrierBandwidth = 200000;

            return gsm;
        }
        private long ConvertUsChannel(int c) {
            long f = 0;
            if(c >= 128 && c <= 251)
                f = (long)(decimal.Round(869.2m + .2m * (c - 128), 1) * 1000000);
            else if(c >= 512 && c <= 810)
                f = (long)(decimal.Round(1930.2m + .2m * (c - 512), 1) * 1000000);
            else
                throw new InvalidOperationException("Unable to convert GSM US channel to frequency.");
            return f;
        }
        private long ConvertEuroChannel(int c) {
            long f = 0;
            if(c >= 0 && c <= 124)
                f = (long)(decimal.Round(935m + .2m * c, 1) * 1000000);
            else if(c >= 955 && c <= 1023)
                f = (long)(decimal.Round(935m + .2m * (c - 1024), 1) * 1000000);
            else if(c >= 512 && c <= 885)
                f = (long)(decimal.Round(1805.2m + .2m * (c - 512), 1) * 1000000);
            else
                throw new InvalidOperationException("Unable to convert GSM US channel to frequency.");
            return f;
        }
        protected override int BinaryStructByteSize() {
            return 59;
        }
        protected enum Version {
            Unknown,
            Us,
            Euro
        };
        private Dictionary<Version, string> Headers;
        Version CurrentVersion;

    }
}
