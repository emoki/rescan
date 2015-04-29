using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using REScan.Data;

namespace REScan.IO {
    public class DasIO : DataIO<Das> {
        public DasIO() {
            Headers = new Dictionary<Version, string>();
            Headers.Add(Version.V1PlainText, "NANOSECONDS\tLatitude\tLongitude\tTxCode\tECIO\tRxLevel\tRMS_SIGNAL\tRMS_CORR");
            Headers.Add(Version.V2PlainText, "nanoseconds\tlatitude\tlongitude\tbroadcast_code\tecio\tbroadcast_code_signal_level\trms_signal\trms_corr");
            Headers.Add(Version.V3EncryptedText, "nanoseconds\tlatitude\tlongitude\tbroadcast_code\tsignal_levels\trms_signal\trms_corr");
            CurrentVersion = Version.Unknown;
        }
        public override string DataType() {
            return "DAS";
        }
        public override string Extension() {
            return "dmm";
        }
        protected override string Header() {
            if(CurrentVersion.Equals(Version.Unknown))
                throw new FormatException("DasIO version is unknown.");
            return Headers[CurrentVersion];
        }
        protected override bool IsText() {
            return true;
        }
        public override List<Das> ReadFile(string fileName) {
            return base.ReadFile(fileName);
        }
        protected override void DetermineHeader(string header) {
            if(header.Contains(Headers[Version.V1PlainText])) 
                CurrentVersion = Version.V1PlainText;
            else if(header.Contains(Headers[Version.V2PlainText])) 
                CurrentVersion = Version.V2PlainText;
            else if(header.Contains(Headers[Version.V3EncryptedText]))
                CurrentVersion = Version.V3EncryptedText;
            else {
                CurrentVersion = Version.Unknown;
                throw new FormatException("Unable to determine DasIO version.");
            }
        }
        protected override Das Parse(string txt) {
            switch(CurrentVersion) {
                case Version.V1PlainText:
                    return ParseV1(txt);
                case Version.V2PlainText:
                    return ParseV2(txt);
                case Version.V3EncryptedText:
                    return ParseV3(txt);
                default:
                    throw new InvalidOperationException("DasIO version is unknown.");
            }
        }
        private Das ParseV1(string txt) {
            var row = txt.Split('\t');
            if(!row.Count().Equals(16))
                ThrowParseException(Version.V1PlainText);

            var index = 0;
            Das das = new Das();
            das.Nanoseconds = long.Parse(row[index++]);
            das.Lat = double.Parse(row[index++]);
            das.Lon = double.Parse(row[index++]);
            das.TransmitterCode = (row[index++]).Trim();
            das.Ecio = double.Parse(row[index++]);
            das.TransmitterSignalLevel = double.Parse(row[index++]);
            index++; // rms_signal
            index++; // rms_corr
            index++; // norm_corr
            das.ScannerID = row[index++].Trim();
            das.CollectionRound = int.Parse(row[index++]);
            das.CarrierSignalLevel = double.Parse(row[index++]);
            das.Frequency = long.Parse(row[index++]);
            das.CarrierBandwidth = int.Parse(row[index++]);
            das.Time = long.Parse(row[index++]);
            var statusFlag = uint.Parse(row[index++]);
            das.IsGpsLocked = (statusFlag & 0x0001) == 0x0; // bit 0 of status_flags is gps_lock; 0 => gps lock; 1 => NOT gps lock .
            return das;
        }

        private void ThrowParseException(Version version) {
            throw new FormatException("Unable to parse " + System.Enum.GetName(typeof(Version), version) + ".  Wrong number of items.");
        }
        private Das ParseV2(string txt) {
            var row = txt.Split('\t');
            if(!row.Count().Equals(16))
                ThrowParseException(Version.V1PlainText);

            var index = 0;
            Das das = new Das();
            das.Nanoseconds = long.Parse(row[index++]);
            das.Lat = double.Parse(row[index++]);
            das.Lon = double.Parse(row[index++]);
            das.TransmitterCode = (row[index++]).Trim();
            das.Ecio = double.Parse(row[index++]);
            das.TransmitterSignalLevel = double.Parse(row[index++]);
            index++; // rms_signal
            index++; // rms_corr
            index++; // norm_corr
            das.ScannerID = row[index++].Trim();
            das.CollectionRound = int.Parse(row[index++]);
            das.CarrierSignalLevel = double.Parse(row[index++]);
            das.Frequency = long.Parse(row[index++]);
            das.CarrierBandwidth = int.Parse(row[index++]);
            das.Time = long.Parse(row[index++]);
            var statusFlag = uint.Parse(row[index++]);
            das.IsGpsLocked = (statusFlag & 0x0001) == 0x0; // bit 0 of status_flags is gps_lock; 0 => gps lock; 1 => NOT gps lock .
            return das;
        }
        private Das ParseV3(string txt) {
            // Implement once REScan is using the newest das_processor.dll.
            throw new NotImplementedException();
        }
        protected enum Version {
            Unknown,
            V1PlainText,
            V2PlainText,
            V3EncryptedText
        };
        private Dictionary<Version, string> Headers;
        Version CurrentVersion;
    }
}
