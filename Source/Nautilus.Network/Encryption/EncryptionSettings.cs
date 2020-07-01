// -------------------------------------------------------------------------------------------------
// <copyright file="EncryptionSettings.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  https://nautechsystems.io
//
//  Licensed under the GNU Lesser General Public License Version 3.0 (the "License");
//  You may not use this file except in compliance with the License.
//  You may obtain a copy of the License at https://www.gnu.org/licenses/lgpl-3.0.en.html
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
// </copyright>
// -------------------------------------------------------------------------------------------------

using Nautilus.Common.Enums;
using Nautilus.Core.Correctness;

namespace Nautilus.Network.Encryption
{
    /// <summary>
    /// Provides encryption settings.
    /// </summary>
    public sealed class EncryptionSettings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EncryptionSettings"/> class.
        /// </summary>
        /// <param name="algorithm">The specified encryption algorithm.</param>
        /// <param name="publicKey">The public encryption key.</param>
        /// <param name="secretKey">The secret encryption key.</param>
        public EncryptionSettings(
            EncryptionAlgorithm algorithm,
            byte[] publicKey,
            byte[] secretKey)
        {
            Condition.NotEqualTo(algorithm, EncryptionAlgorithm.Undefined, nameof(algorithm));

            this.UseEncryption = algorithm != EncryptionAlgorithm.None;
            this.Algorithm = algorithm;
            this.PublicKey = publicKey;
            this.SecretKey = secretKey;
        }

        /// <summary>
        /// Gets a value indicating whether encryption should be used.
        /// </summary>
        public bool UseEncryption { get; }

        /// <summary>
        /// Gets the configurations encryption algorithm.
        /// </summary>
        public EncryptionAlgorithm Algorithm { get; }

        /// <summary>
        /// Gets the public encryption key.
        /// </summary>
        public byte[] PublicKey { get; }

        /// <summary>
        /// Gets the secret encryption key.
        /// </summary>
        public byte[] SecretKey { get; }

        /// <summary>
        /// Create encryption settings with no encryption specified.
        /// </summary>
        /// <returns>The configuration.</returns>
        public static EncryptionSettings None()
        {
            return new EncryptionSettings(
                EncryptionAlgorithm.None,
                new byte[0],
                new byte[0]);
        }
    }
}
