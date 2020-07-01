// -------------------------------------------------------------------------------------------------
// <copyright file="EncryptionProvider.cs" company="Nautech Systems Pty Ltd">
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

using NetMQ;

namespace Nautilus.Network.Encryption
{
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
