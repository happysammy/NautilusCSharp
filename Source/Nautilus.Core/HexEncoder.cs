// -------------------------------------------------------------------------------------------------
// <copyright file="HexEncoder.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Core
{
    using System.IO;
    using Nautilus.Core.Annotations;

    /// <summary>
    /// Provides a performant hex encoder.
    /// </summary>
    [PerformanceOptimized]
    internal class HexEncoder
    {
        private readonly byte[] encodingTable =
        {
            (byte)'0', (byte)'1', (byte)'2', (byte)'3', (byte)'4', (byte)'5', (byte)'6', (byte)'7',
            (byte)'8', (byte)'9', (byte)'a', (byte)'b', (byte)'c', (byte)'d', (byte)'e', (byte)'f',
        };

        private readonly byte[] decodingTable = new byte[128];

        /// <summary>
        /// Initializes a new instance of the <see cref="HexEncoder"/> class.
        /// </summary>
        internal HexEncoder()
        {
            this.InitialiseDecodingTable();
        }

        /// <summary>
        /// Encode the given Hex encoded byte data producing a Hex output stream.
        /// </summary>
        /// <param name="data">The bytes to encode.</param>
        /// <param name="off">The offset in the array.</param>
        /// <param name="length">The length of the array.</param>
        /// <param name="outStream">The output stream.</param>
        /// <returns>The number of bytes produced.</returns>
        internal int Encode(byte[] data, int off, int length, Stream outStream)
        {
            for (var i = off; i < (off + length); i++)
            {
                var v = data[i];

                outStream.WriteByte(this.encodingTable[v >> 4]);
                outStream.WriteByte(this.encodingTable[v & 0xf]);
            }

            return length * 2;
        }

        /// <summary>
        /// Decode the given Hex encoded byte data writing it to the given output stream, white
        /// space characters will be ignored.
        /// </summary>
        /// <param name="data">The bytes to decode.</param>
        /// <param name="off">The offset in the array.</param>
        /// <param name="length">The length of the array.</param>
        /// <param name="outStream">The output stream.</param>
        /// <returns>The number of bytes produced.</returns>
        /// <exception cref="IOException">If invalid characters are encountered.</exception>
        internal int Decode(byte[] data, int off, int length, Stream outStream)
        {
            var outLen = 0;
            var end = off + length;

            while (end > off)
            {
                if (!Ignore((char)data[end - 1]))
                {
                    break;
                }

                end--;
            }

            var i = off;
            while (i < end)
            {
                while (i < end && Ignore((char)data[i]))
                {
                    i++;
                }

                var b1 = this.decodingTable[data[i++]];

                while (i < end && Ignore((char)data[i]))
                {
                    i++;
                }

                var b2 = this.decodingTable[data[i++]];

                if ((b1 | b2) >= 0x80)
                {
                    throw new IOException("Invalid characters encountered in Hex data.");
                }

                outStream.WriteByte((byte)((b1 << 4) | b2));

                outLen++;
            }

            return outLen;
        }

        /// <summary>
        /// Decode the Hex encoded string data writing it to the given output stream, white space
        /// characters will be ignored.
        /// </summary>
        /// <param name="data">The string data to decode.</param>
        /// <param name="outStream">The output stream.</param>
        /// <returns>The number of bytes produced.</returns>
        /// <exception cref="IOException">If invalid characters encountered in Hex data.</exception>
        internal int DecodeString(string data, Stream outStream)
        {
            var length = 0;
            var end = data.Length;

            while (end > 0)
            {
                if (!Ignore(data[end - 1]))
                {
                    break;
                }

                end--;
            }

            var i = 0;
            while (i < end)
            {
                while (i < end && Ignore(data[i]))
                {
                    i++;
                }

                var b1 = this.decodingTable[data[i++]];

                while (i < end && Ignore(data[i]))
                {
                    i++;
                }

                var b2 = this.decodingTable[data[i++]];

                if ((b1 | b2) >= 0x80)
                {
                    throw new IOException("Invalid characters encountered in Hex data.");
                }

                outStream.WriteByte((byte)((b1 << 4) | b2));

                length++;
            }

            return length;
        }

        private static void FillArray(byte[] buf, byte b)
        {
            var i = buf.Length;
            while (i > 0)
            {
                buf[--i] = b;
            }
        }

        private static bool Ignore(char c)
        {
            return c == '\n' || c == '\r' || c == '\t' || c == ' ';
        }

        private void InitialiseDecodingTable()
        {
            FillArray(this.decodingTable, 0xff);

            for (var i = 0; i < this.encodingTable.Length; i++)
            {
                this.decodingTable[this.encodingTable[i]] = (byte)i;
            }

            this.decodingTable['A'] = this.decodingTable['a'];
            this.decodingTable['B'] = this.decodingTable['b'];
            this.decodingTable['C'] = this.decodingTable['c'];
            this.decodingTable['D'] = this.decodingTable['d'];
            this.decodingTable['E'] = this.decodingTable['e'];
            this.decodingTable['F'] = this.decodingTable['f'];
        }
    }
}
