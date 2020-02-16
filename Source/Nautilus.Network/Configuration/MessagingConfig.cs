// -------------------------------------------------------------------------------------------------
// <copyright file="MessagingConfig.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   https://nautechsystems.io
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Network.Configuration
{
    using Nautilus.Common.Enums;

    /// <summary>
    /// Represents a messaging protocol configuration.
    /// </summary>
    public sealed class MessagingConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessagingConfig"/> class.
        /// </summary>
        /// <param name="apiVersion">The messaging API version.</param>
        /// <param name="compression">The messaging compression codec.</param>
        /// <param name="encryption">The messaging encryption cryptographic algorithm.</param>
        /// <param name="keysDirectory">The messaging encryption cryptographic keys directory.</param>
        public MessagingConfig(
            string apiVersion,
            CompressionCodec compression,
            CryptographicAlgorithm encryption,
            string keysDirectory)
        {
            this.Version = apiVersion;
            this.CompressionCodec = compression;
            this.EncryptionConfig = EncryptionConfig.Create(encryption, keysDirectory);
        }

        /// <summary>
        /// Gets the messaging API version.
        /// </summary>
        public string Version { get; }

        /// <summary>
        /// Gets the messaging compression codec.
        /// </summary>
        public CompressionCodec CompressionCodec { get; }

        /// <summary>
        /// Gets the messaging encryption configuration.
        /// </summary>
        public EncryptionConfig EncryptionConfig { get; }

        /// <summary>
        /// Return a default development environment messaging configuration with no compression or encryption.
        /// </summary>
        /// <returns>The messaging configuration.</returns>
        public static MessagingConfig Development()
        {
            return new MessagingConfig(
                "1.0",
                CompressionCodec.None,
                CryptographicAlgorithm.None,
                string.Empty);
        }
    }
}
