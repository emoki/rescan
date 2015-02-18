using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using REScan.Data;

namespace REScan.IO
{
    public abstract class DataIO<T> where T : Coordinate
    {
        public abstract string DataType();
        public abstract string Extension();
        protected abstract string Header();
        protected abstract bool IsText();
        public virtual List<T> ReadFile(string fileName) {
            SetAndValidateHeader(fileName);

            if(IsText()) {
                return ReadTextFile(fileName);
            }
            else {
                return ReadBinaryFile(fileName);
            }
        }
        protected virtual void SetAndValidateHeader(string fileName) {
            using(BinaryReader reader = new BinaryReader(File.OpenRead(fileName))) {
                var byteHeader = reader.ReadBytes(MaxHeaderLength);
                var stringHeader = System.Text.Encoding.Default.GetString(byteHeader);
                DetermineHeader(stringHeader);
                if(!stringHeader.Contains(Header())) {
                    throw new FormatException("File header for '" + fileName + "' does not match" + DataType() + " header.  Possibly wrong file format.");
                }
            }
        }
        protected virtual void DetermineHeader(string header) {
            // Using the header this function determines which file version we are reading in.  Therefore
            // it only needs to be overriden by classes that have multiple versions.
        }
        protected virtual List<T> ReadTextFile(string fileName) {
            List<T> records = new List<T>();

            using(TextReader reader = new StreamReader(fileName)) {

                // Skip header.
                var stringHeader = reader.ReadLine();

                // Begin populating records till end of file. 
                while(reader.Peek() != -1) {
                    var record = Parse(reader.ReadLine());
                    records.Add(record);
                }

                return records;
            }
        }
        protected virtual List<T> ReadBinaryFile(string fileName) {
            List<T> records = new List<T>();

            using(BinaryReader reader = new BinaryReader(File.OpenRead(fileName))) {

                // Skip ahead to the beginning of the binary data.  Every header has a carriage return appended
                // so there will at least be one byte to read.
                while(reader.BaseStream.Position != reader.BaseStream.Length && reader.ReadByte() != 0x0A) {} 

                // Begin populating records till end of file. 
                while(reader.BaseStream.Position != reader.BaseStream.Length) {
                    var record = Parse(reader.ReadBytes(BinaryStructByteSize()));
                    records.Add(record);
                }

                return records;
            }
        }
        protected virtual T Parse(string ascii) {
            throw new NotSupportedException("Text parse of " + DataType() + " is not supported.");
        }
        protected virtual T Parse(byte[] binary) {
            throw new NotSupportedException("Binary parse of " + DataType() + " is not supported.");
        }
        protected virtual int BinaryStructByteSize() {
            throw new NotSupportedException(DataType() + " does not have a binary struct size.");
        }

        protected int MaxHeaderLength = 10240;
    }
}
