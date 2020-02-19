// -------------------------------------------------------------------------------------------------
// <copyright file="EncryptionProvider.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   https://nautechsystems.io
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Network.Encryption
{
    using NetMQ;

    /// <summary>
    /// Provides encryption setup for ZeroMQ sockets.
    /// </summary>
    public static class EncryptionProvider
    {
        /// <summary>
        /// Setup Curve encryption.
        /// </summary>
        /// <param name="settings">The encryption settings.</param>
        /// <param name="socket">The socket for the encryption.</param>
        public static void SetupSocket(EncryptionSettings settings, NetMQSocket socket)
        {
            var certificate = new NetMQCertificate(settings.SecretKey, settings.PublicKey);

            socket.Options.CurveServer = true;
            socket.Options.CurveCertificate = certificate;
        }
    }
}
