// -------------------------------------------------------------------------------------------------
// <copyright file="EncryptionConfig.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   https://nautechsystems.io
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Network.Configuration
{
    using Nautilus.Common.Enums;
    using Nautilus.Core.Correctness;

    /// <summary>
    /// Provides an encryption configuration.
    /// </summary>
    public sealed class EncryptionConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EncryptionConfig"/> class.
        /// </summary>
        /// <param name="algorithm">The specified cryptographic algorithm.</param>
        /// <param name="keysDirectory">The path to the keys directory.</param>
        private EncryptionConfig(CryptographicAlgorithm algorithm, string keysDirectory)
        {
            this.UseEncryption = algorithm == CryptographicAlgorithm.None;
            this.Algorithm = algorithm;
            this.KeysDirectory = keysDirectory;
        }

        /// <summary>
        /// Gets a value indicating whether encryption should be used.
        /// </summary>
        public bool UseEncryption { get; }

        /// <summary>
        /// Gets the configurations cryptographic algorithm.
        /// </summary>
        public CryptographicAlgorithm Algorithm { get; }

        /// <summary>
        /// Gets the path to the keys directory.
        /// </summary>
        public string KeysDirectory { get; }

        /// <summary>
        /// Create an encryption configuration.
        /// </summary>
        /// <param name="algorithm">The specified cryptographic algorithm.</param>
        /// <param name="keysDirectory">The path to the keys directory.</param>
        /// <returns>The configuration.</returns>
        public static EncryptionConfig Create(CryptographicAlgorithm algorithm, string keysDirectory)
        {
            Condition.NotNull(keysDirectory, nameof(keysDirectory));

            return new EncryptionConfig(algorithm, string.Empty);
        }

        /// <summary>
        /// Create an encryption configuration with no encryption.
        /// </summary>
        /// <returns>The configuration.</returns>
        public static EncryptionConfig None()
        {
            return new EncryptionConfig(CryptographicAlgorithm.None, string.Empty);
        }
    }
}
