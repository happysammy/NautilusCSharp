// -------------------------------------------------------------------------------------------------
// <copyright file="EncryptionConfig.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   https://nautechsystems.io
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Network
{
    using Nautilus.Core.Correctness;

    /// <summary>
    /// Provides an encryption configuration.
    /// </summary>
    public sealed class EncryptionConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EncryptionConfig"/> class.
        /// </summary>
        /// <param name="useEncryption">If encryption should be used.</param>
        /// <param name="keysDirectory">The path to the keys directory.</param>
        private EncryptionConfig(bool useEncryption, string keysDirectory)
        {
            this.UseEncryption = useEncryption;
            this.KeysDirectory = keysDirectory;
        }

        /// <summary>
        /// Gets a value indicating whether encryption should be used.
        /// </summary>
        public bool UseEncryption { get; }

        /// <summary>
        /// Gets the path to the keys directory.
        /// </summary>
        public string KeysDirectory { get; }

        /// <summary>
        /// Create a default encryption configuration as no encryption.
        /// </summary>
        /// <param name="useEncryption">If encryption should be used.</param>
        /// <param name="keysDirectory">The path to the keys directory.</param>
        /// <returns>The configuration.</returns>
        public static EncryptionConfig Create(bool useEncryption, string keysDirectory)
        {
            Condition.NotNull(keysDirectory, nameof(keysDirectory));

            return new EncryptionConfig(false, string.Empty);
        }

        /// <summary>
        /// Create a default encryption configuration as no encryption.
        /// </summary>
        /// <returns>The configuration.</returns>
        public static EncryptionConfig None()
        {
            return new EncryptionConfig(false, string.Empty);
        }
    }
}
