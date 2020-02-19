// -------------------------------------------------------------------------------------------------
// <copyright file="Z85Encoder.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   https://nautechsystems.io
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Network.Encryption
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Text;

    /// <summary>
    /// Providers a Z85 encoder.
    /// </summary>
    public static class Z85Encoder
    {
        private static readonly char[] Chars =
        {
            '0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
            'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j',
            'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't',
            'u', 'v', 'w', 'x', 'y', 'z', 'A', 'B', 'C', 'D',
            'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N',
            'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X',
            'Y', 'Z', '.', '-', ':', '+', '=', '^', '!', '/',
            '*', '?', '&', '<', '>', '(', ')', '[', ']', '{',
            '}', '@', '%', '$', '#',
        };

        private static readonly byte[] Base256 =
        {
           0x00, 0x44, 0x00, 0x54, 0x53, 0x52, 0x48, 0x00,
           0x4B, 0x4C, 0x46, 0x41, 0x00, 0x3F, 0x3E, 0x45,
           0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07,
           0x08, 0x09, 0x40, 0x00, 0x49, 0x42, 0x4A, 0x47,
           0x51, 0x24, 0x25, 0x26, 0x27, 0x28, 0x29, 0x2A,
           0x2B, 0x2C, 0x2D, 0x2E, 0x2F, 0x30, 0x31, 0x32,
           0x33, 0x34, 0x35, 0x36, 0x37, 0x38, 0x39, 0x3A,
           0x3B, 0x3C, 0x3D, 0x4D, 0x00, 0x4E, 0x43, 0x00,
           0x00, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F, 0x10,
           0x11, 0x12, 0x13, 0x14, 0x15, 0x16, 0x17, 0x18,
           0x19, 0x1A, 0x1B, 0x1C, 0x1D, 0x1E, 0x1F, 0x20,
           0x21, 0x22, 0x23, 0x4F, 0x00, 0x50, 0x00, 0x00,
        };

        /// <summary>
        /// Converts an array of 8-bit unsigned integers to its equivalent string representation
        /// that is base-85 encoded with the Z85 character set.
        /// </summary>
        /// <param name="inArray">An array of 8-bit unsigned integers.</param>
        /// <param name="autoPad">If the string should be auto padded.</param>
        /// <returns>The string representation in Z85 of the elements in inArray.</returns>
        public static string ToZ85String(byte[] inArray, bool autoPad)
        {
            if (inArray == null)
            {
                throw new ArgumentNullException(nameof(inArray));
            }

            if (inArray.Length == 0)
            {
                return string.Empty;
            }

            var lengthMod4 = inArray.Length % 4;
            var paddingRequired = lengthMod4 != 0;

            if (!autoPad && paddingRequired)
            {
                throw new ArgumentException("Array length invalid for encoding. Must be a multiple of 4.");
            }

            var bytesToEncode = inArray;
            var bytesToPad = 0;
            if (autoPad && paddingRequired)
            {
                // Bytes must be divisible by 4, pad with zeros
                bytesToPad = 4 - lengthMod4;
                bytesToEncode = new byte[inArray.Length + bytesToPad];
                Array.Copy(inArray, 0, bytesToEncode, 0, inArray.Length);
            }

            var z85String = ToZ85String(bytesToEncode);

            if (paddingRequired)
            {
                z85String += bytesToPad;
            }

            return z85String;
        }

        /// <summary>
        /// Converts the specified string, which encodes binary data in the Z85 (base-85) character
        /// set, to an equivalent 8-bit unsigned integer array.
        /// </summary>
        /// <param name="string">The string to convert.</param>
        /// <returns>An array of 8-bit unsigned integers that is equivalent to s.</returns>
        [SuppressMessage("ReSharper", "SA1025", Justification = "Easier to read.")]
        [SuppressMessage("ReSharper", "SA1407", Justification = "Original implementation.")]
        public static byte[] FromZ85String(string @string)
        {
            var lengthMod5 = @string.Length % 5;
            if ((lengthMod5 != 0) && (@string.Length - 1) % 5 != 0)
            {
                throw new ArgumentException("Invalid length for a Z85 string.");
            }

            var paddingBytes = 0;
            if (lengthMod5 != 0)
            {
                if (!int.TryParse(@string[^1].ToString(), out paddingBytes)
                    || paddingBytes < 1
                    || paddingBytes > 3)
                {
                    throw new ArgumentException("Invalid padding character for a Z85 string.");
                }
            }

            var outputLength = (@string.Length / 5) * 4;
            var output = new List<byte>(outputLength);
            var byteIndex = 0;
            for (var i = 0; i < @string.Length - 1; i += 5)
            {
                uint intValue =            Base256[(@string[i + 0] - 32) & 127];
                intValue = intValue * 85 + Base256[(@string[i + 1] - 32) & 127];
                intValue = intValue * 85 + Base256[(@string[i + 2] - 32) & 127];
                intValue = intValue * 85 + Base256[(@string[i + 3] - 32) & 127];
                intValue = intValue * 85 + Base256[(@string[i + 4] - 32) & 127];

                output.Insert(byteIndex++, (byte)(intValue >> 24));
                intValue = intValue << 8 >> 8;
                output.Insert(byteIndex++, (byte)(intValue >> 16));
                intValue = intValue << 16 >> 16;
                output.Insert(byteIndex++, (byte)(intValue >> 8));
                intValue = intValue << 24 >> 24;
                output.Insert(byteIndex++, (byte)intValue);
            }

            // Remove padding bytes
            while (paddingBytes-- > 0)
            {
                output.RemoveAt(output.Count - 1);
            }

            return output.ToArray();
        }

        private static string ToZ85String(byte[] inArray)
        {
            var encodedLength = (inArray.Length / 4) * 5; // 4 bytes = 5 chars

            var sb = new StringBuilder(encodedLength);
            for (var i = 0; i < inArray.Length; i += 4)
            {
                // Add 4 bytes to a binary frame
                var signedInt = inArray[i + 0] << 24 |
                                inArray[i + 1] << 16 |
                                inArray[i + 2] << 8 |
                                inArray[i + 3];

                var binaryFrame = (uint)signedInt;

                var encodedChars = new char[5];

                // Convert into 5 characters, dividing by 85 and taking the remainder
                uint divisor = 52200625; // 85 * 85 * 85 * 85;
                for (var j = 0; j < 5; j++)
                {
                    var divisible = (binaryFrame / divisor) % 85;
                    encodedChars[j] = Chars[divisible];
                    binaryFrame -= divisible * divisor;
                    divisor /= 85;
                }

                sb.Append(encodedChars);
            }

            return sb.ToString();
        }
    }
}
