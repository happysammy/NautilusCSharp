// -------------------------------------------------------------------------------------------------
// <copyright file="EncryptionProvider.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   https://nautechsystems.io
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Network
{
    using System.IO;
    using System.Text;
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
        public static void Setup(EncryptionConfig config, NetMQSocket socket)
        {
            // Load keys
            var publicKey = Encoding.UTF8.GetBytes(File.ReadAllText(config.KeysDirectory));
            var secretKey = Encoding.UTF8.GetBytes(File.ReadAllText(config.KeysDirectory));
            var certificate = new NetMQCertificate(publicKey, secretKey);

            socket.Options.CurveServer = true;
            socket.Options.CurveCertificate = certificate;
        }
    }
}
