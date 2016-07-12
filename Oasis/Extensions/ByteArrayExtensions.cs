using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oasis.Extensions
{

    public static class ByteArrayExtensions
    {

        public static byte[] Subbyte(this byte[] value, int offset)
        {
            int length = value.Length - offset;

            return value.Subbyte(offset, length);
        }

        public static byte[] Subbyte(this byte[] value, int offset, int length)
        {
            byte[] buffer = new byte[length];

            Buffer.BlockCopy(value, offset, buffer, 0, length);

            return buffer;
        }

    }

}
