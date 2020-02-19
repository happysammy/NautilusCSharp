// -------------------------------------------------------------------------------------------------
// <copyright file="WireConfiguration.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   https://nautechsystems.io
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Network.Configuration
{
    using Nautilus.Common.Enums;
    using Nautilus.Core.Correctness;
    using Nautilus.Network.Encryption;

    /// <summary>
    /// Represents a messaging protocol configuration.
    /// </summary>
    public sealed class WireConfiguration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WireConfiguration"/> class.
        /// </summary>
        /// <param name="apiVersion">The wire messaging API version.</param>
        /// <param name="compression">The wire messaging compression codec.</param>
        /// <param name="encryptionConfig">The wire messaging cryptographic algorithm.</param>
        public WireConfiguration(
            string apiVersion,
            CompressionCodec compression,
            EncryptionSettings encryptionConfig)
        {
            Condition.NotEmptyOrWhiteSpace(apiVersion, nameof(apiVersion));
            Condition.NotEqualTo(compression, CompressionCodec.Undefined, nameof(compression));

            this.Version = apiVersion;
            this.CompressionCodec = compression;
            this.EncryptionConfig = encryptionConfig;
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
        public EncryptionSettings EncryptionConfig { get; }

        /// <summary>
        /// Return a default development environment messaging configuration with no compression or encryption.
        /// </summary>
        /// <returns>The messaging configuration.</returns>
        public static WireConfiguration Development()
        {
            return new WireConfiguration(
                "1.0",
                CompressionCodec.None,
                EncryptionSettings.None());
        }
    }
}
