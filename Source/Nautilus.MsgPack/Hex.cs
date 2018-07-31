// -------------------------------------------------------------------------------------------------
// <copyright file="Hex.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.MsgPack
{
    using System;
    using System.IO;
    using System.Text;
    using Nautilus.Core.Annotations;

    /// <summary>
    /// Provides hexadecimal encoding and decoding operations.
    /// </summary>
    /// <remarks>
    /// In mathematics and computing, hexadecimal is a positional numeral system with a radix, or
    /// base, of 16. It uses sixteen distinct symbols, most often the symbols 0–9 to represent
    /// values zero to nine, and A–F to represent values ten to fifteen.
    /// </remarks>
    [PerformanceOptimized]
    public static class Hex
    {
        private static readonly HexEncoder Encoder = new HexEncoder();

        /// <summary>
        /// Returns hex representation of the byte array.
        /// </summary>
        /// <param name="data">bytes to encode</param>
        /// <returns>The hex string.</returns>
        public static string ToHexString(byte[] data)
        {
            return ToHexString(data, 0, data.Length);
        }

        /// <summary>
        /// Returns hex representation of the byte array.
        /// </summary>
        /// <param name="data">bytes to encode</param>
        /// <param name="off">offset</param>
        /// <param name="length">number of bytes to encode</param>
        /// <returns>The hex string.</returns>
        public static string ToHexString(byte[] data, int off, int length)
        {
            return Encoding.ASCII.GetString(Encode(data, off, length));
        }

        /// <summary>
        /// Decodes hex representation to a byte array.
        /// </summary>
        /// <param name="hex">hex string to decode</param>
        /// <returns>The decoded byte array.</returns>
        public static byte[] FromHexString(string hex)
        {
            if (hex != null && hex.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
            {
                hex = hex.Substring(2);
            }
            else if (hex != null && hex.EndsWith("h", StringComparison.OrdinalIgnoreCase))
            {
                hex = hex.Substring(0, hex.Length - 1);
            }

            if (string.IsNullOrEmpty(hex))
            {
                throw new ArgumentException();
            }

            using (var stream = new MemoryStream())
            {
                Encoder.DecodeString(hex, stream);
                return stream.ToArray();
            }
        }

        /// <summary>
        /// Returns a string containing a nice representation of the byte array
        /// (similarly to the binary editors).
        /// </summary>
        /// <param name="bytes">array of bytes to pretty print</param>
        /// <returns>The pretty printed string.</returns>
        public static string PrettyPrint(byte[] bytes)
        {
            return PrettyPrint(bytes, 0, bytes.Length);
        }

        /// <summary>
        /// Returns a string containing a nice representation  of the byte array
        /// (similarly to the binary editors).
        ///
        /// Example output:
        ///
        ///        0  1  2  3  4  5  6  7  8  9  A  B  C  D  E  F
        /// 0000: c8 83 93 8f b0 cb cb d3 d1 e5 7c ff 52 dc ea 92  E....ËËÓNa.yRÜe.
        /// 0010: 5b af 30 ca d8 7a 35 e9 2e 46 fa 85 b7 38 3f 4e  [.0EOz5é.Fú.8?N
        /// 0020: 8d 60 af 4a 00 00 00 00 57 4d a4 29 35 9e c2 6f  ...J....WM.)5.Âo
        /// 0030: 30 7b 92 40 33 6d 55 43 46 fe d6 8d ef 67 99 9c  0{.@3mUCF?Ö.ig..
        /// </summary>
        /// <param name="bytes">The array of bytes to pretty print.</param>
        /// <param name="offset">The offset in the array.</param>
        /// <param name="length">The number of bytes to print.</param>
        /// <returns>The pretty printed string.</returns>
        public static string PrettyPrint(byte[] bytes, int offset, int length)
        {
            if (bytes.Length == 0)
            {
                return string.Empty;
            }

            var buffer = new StringBuilder();
            var maxLength = offset + length;
            if (offset < 0 || offset >= bytes.Length || maxLength > bytes.Length)
            {
                throw new ArgumentException();
            }

            var end = Math.Min(offset + 16, maxLength);
            var start = offset;

            // Don't reformat the whitespace.
            buffer.Append("       0  1  2  3  4  5  6  7  8  9  A  B  C  D  E  F").AppendLine();
            while (end <= maxLength)
            {
                // Print offset.
                buffer.Append($"{start - offset:x4}:");

                // Print hex bytes.
                for (var i = start; i < end; i++)
                {
                    buffer.Append($" {bytes[i] :x2}");
                }

                for (var i = 0; i < 16 - (end - start); i++)
                {
                    buffer.Append("   ");
                }

                buffer.Append("  ");

                // Print ascii characters.
                for (var i = start; i < end; i++)
                {
                    var c = (char)bytes[i];
                    if (char.IsLetterOrDigit(c) || char.IsPunctuation(c))
                    {
                        buffer.Append($"{c}");
                    }
                    else
                    {
                        buffer.Append(".");
                    }
                }

                if (end == maxLength)
                {
                    break;
                }

                start = end;
                end = Math.Min(end + 16, maxLength);
                buffer.AppendLine();
            }

            return buffer.ToString();
        }

        private static byte[] Encode(byte[] data, int off, int length)
        {
            using (var stream = new MemoryStream())
            {
                Encoder.Encode(data, off, length, stream);
                return stream.ToArray();
            }
        }
    }
}
