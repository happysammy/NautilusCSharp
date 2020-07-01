// -------------------------------------------------------------------------------------------------
// <copyright file="WireConfiguration.cs" company="Nautech Systems Pty Ltd">
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
using Nautilus.Network.Encryption;

namespace Nautilus.Network.Configuration
{
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
