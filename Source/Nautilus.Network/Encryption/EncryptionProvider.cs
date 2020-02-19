// -------------------------------------------------------------------------------------------------
// <copyright file="EncryptionProvider.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   https://nautechsystems.io
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Network.Encryption
{
    using System.IO;
    using Nautilus.Network.Configuration;
    using NetMQ;

    /// <summary>
    /// Provides encryption setup for ZeroMQ sockets.
    /// </summary>
    public static class EncryptionProvider
    {
        /// <summary>
        /// Setup Curve encryption.
        /// </summary>
        /// <param name="config">The configuration for the encryption.</param>
        /// <param name="socket">The socket for the encryption.</param>
        public static void SetupSocket(EncryptionConfiguration config, NetMQSocket socket)
        {
            // Load keys
            var publicKeyPath = Path.Combine(config.KeysPath, "server.key");
            var secretKeyPath = Path.Combine(config.KeysPath, "server.key_secret");

            var publicKeyFileSplit = File.ReadAllText(publicKeyPath).Split("public-key = ");
            var secretKeyFileSplit = File.ReadAllText(secretKeyPath).Split("secret-key = ");
            var publicKey = publicKeyFileSplit[1].TrimStart('"').TrimEnd().TrimEnd('"');
            var secretKey = secretKeyFileSplit[1].TrimStart('"').TrimEnd().TrimEnd('"');

            var publicKeyEncoded = Z85Encoder.FromZ85String(publicKey);
            var secretKeyEncoded = Z85Encoder.FromZ85String(secretKey);

            var certificate = new NetMQCertificate(secretKeyEncoded, publicKeyEncoded);

            socket.Options.CurveServer = true;
            socket.Options.CurveCertificate = certificate;
        }
    }
}
