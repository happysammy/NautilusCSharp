// -------------------------------------------------------------------------------------------------
// <copyright file="EncryptionConfiguration.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   https://nautechsystems.io
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Network.Configuration
{
    using System;
    using Nautilus.Common.Enums;
    using Nautilus.Core.Correctness;

    /// <summary>
    /// Provides an encryption configuration.
    /// </summary>
    public sealed class EncryptionConfiguration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EncryptionConfiguration"/> class.
        /// </summary>
        /// <param name="algorithm">The specified cryptographic algorithm.</param>
        /// <param name="keysPath">The path to the keys directory.</param>
        private EncryptionConfiguration(CryptographicAlgorithm algorithm, string keysPath)
        {
            this.UseEncryption = algorithm != CryptographicAlgorithm.None;
            this.Algorithm = algorithm;
            this.KeysPath = keysPath;
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
        public string KeysPath { get; }

        /// <summary>
        /// Create an encryption configuration.
        /// </summary>
        /// <param name="algorithm">The specified cryptographic algorithm.</param>
        /// <param name="keysDirectory">The path to the keys directory.</param>
        /// <returns>The configuration.</returns>
        /// <exception cref="ArgumentException">If the keys directory is an empty or whitespace string.</exception>
        public static EncryptionConfiguration Create(CryptographicAlgorithm algorithm, string keysDirectory)
        {
            Condition.NotEmptyOrWhiteSpace(keysDirectory, nameof(keysDirectory));

            return new EncryptionConfiguration(algorithm, keysDirectory);
        }

        /// <summary>
        /// Create an encryption configuration with no encryption.
        /// </summary>
        /// <returns>The configuration.</returns>
        public static EncryptionConfiguration None()
        {
            return new EncryptionConfiguration(CryptographicAlgorithm.None, string.Empty);
        }
    }
}
