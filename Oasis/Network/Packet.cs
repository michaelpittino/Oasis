using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Oasis.Network
{

    public class Packet
    {

        public byte[] Data { get { return this.data; } }

        public ushort Length { get { return this.length; } }
        public bool Encrypted { get { return this.encrypted; } }
        public ushort OpCode { get { return this.opCode; } }

        private byte[] data;

        private ushort length;
        private bool encrypted;
        private ushort opCode;

        public Packet(byte[] data)
        {
            this.data = data;

            using (MemoryStream memoryStream = new MemoryStream(this.data))
            using (BinaryReader binaryReader = new BinaryReader(memoryStream))
            {
                this.length = binaryReader.ReadUInt16();
                this.encrypted = binaryReader.ReadBoolean();
                this.opCode = binaryReader.ReadUInt16();
            }
        }

    }

}
