// -------------------------------------------------------------------------------------------------
// <copyright file="EncryptionSettings.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   https://nautechsystems.io
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Network.Encryption
{
    using Nautilus.Common.Enums;
    using Nautilus.Core.Correctness;

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
        /// Gets the configurations cryptographic algorithm.
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
        /// Create an encryption configuration with no encryption.
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
