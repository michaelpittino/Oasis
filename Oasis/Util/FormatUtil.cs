using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Oasis.Extensions;

namespace Oasis.Util
{

    public static class FormatUtil
    {

        public static void FormatAsHexToStream(byte[] data, Stream stream)
        {
            StreamWriter streamWriter = new StreamWriter(stream);

            int bytesPerRow = 16;
            int totalBytes = data.Length;
            int totalRows = (totalBytes / bytesPerRow) + 1;

            for (int i = 0; i < totalRows; i++)
            {
                int rowIndex = (i * bytesPerRow);
                int bytesRemaining = data.Length - rowIndex;
                byte[] row = data.Subbyte(rowIndex, (bytesRemaining > bytesPerRow) ? bytesPerRow : bytesRemaining);

                streamWriter.Write("0x{0:x8} ", (i * bytesPerRow));

                for (int j = 0; j < row.Length; j++)
                    streamWriter.Write("{0:x2} ", row[j]);

                int spacesRequired = bytesPerRow - row.Length;

                for (int j = 0; j < spacesRequired; j++)
                    streamWriter.Write("   ");

                for (int j = 0; j < row.Length; j++)
                {
                    if (row[j] > 0x1f && row[j] < 0x80)
                        streamWriter.Write(row[j].To<Char>());
                    else
                        streamWriter.Write(".");
                }

                streamWriter.WriteLine();
            }

            streamWriter.Flush();
            stream.Flush();
        }

    }

}
