// -------------------------------------------------------------------------------------------------
// <copyright file="MessagingConfiguration.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   https://nautechsystems.io
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Network.Configuration
{
    using System.Collections.Immutable;
    using Nautilus.Common.Enums;
    using Nautilus.Core.Correctness;
    using Nautilus.Core.Extensions;

    /// <summary>
    /// Represents a messaging protocol configuration.
    /// </summary>
    public sealed class MessagingConfiguration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessagingConfiguration"/> class.
        /// </summary>
        /// <param name="apiVersion">The messaging API version.</param>
        /// <param name="compression">The messaging compression codec.</param>
        /// <param name="encryptionConfig">The messaging encryption cryptographic algorithm.</param>
        private MessagingConfiguration(
            string apiVersion,
            CompressionCodec compression,
            EncryptionConfiguration encryptionConfig)
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
        public EncryptionConfiguration EncryptionConfig { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessagingConfiguration"/> class.
        /// </summary>
        /// <param name="configuration">The messaging configuration.</param>
        /// <returns>The configuration.</returns>
        public static MessagingConfiguration Create(ImmutableDictionary<string, string> configuration)
        {
            var encryptionConfig = EncryptionConfiguration.Create(
                configuration["Messaging:Encryption"].ToEnum<CryptographicAlgorithm>(),
                configuration["Messaging:KeysPath"]);

            return new MessagingConfiguration(
                configuration["Messaging:Version"],
                configuration["Messaging:Compression"].ToEnum<CompressionCodec>(),
                encryptionConfig);
        }

        /// <summary>
        /// Return a default development environment messaging configuration with no compression or encryption.
        /// </summary>
        /// <returns>The messaging configuration.</returns>
        public static MessagingConfiguration Development()
        {
            return new MessagingConfiguration(
                "1.0",
                CompressionCodec.None,
                EncryptionConfiguration.None());
        }
    }
}
