//--------------------------------------------------------------------------------------------------
// <copyright file="EnvelopeFactory.cs" company="Nautech Systems Pty Ltd">
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
//--------------------------------------------------------------------------------------------------

using System;
using Nautilus.Core.Types;
using Nautilus.Messaging.Interfaces;
using NodaTime;

namespace Nautilus.Messaging
{
    /// <summary>
    /// Provides a factory for creating generic envelopes of type T.
    /// </summary>
    public static class EnvelopeFactory
    {
        private static readonly Type GenericEnvelope = typeof(Envelope<>);

        /// <summary>
        /// Returns a new <see cref="Envelope{T}"/> based on the given <see cref="Message"/>.
        /// </summary>
        /// <param name="message">The message to wrap.</param>
        /// <param name="receiver">The envelope receiver.</param>
        /// <param name="sender">The envelope sender.</param>
        /// <param name="timestamp">The envelope creation timestamp.</param>
        /// <returns>The created envelope.</returns>
        public static IEnvelope Create(
            Message message,
            Address? receiver,
            Address? sender,
            ZonedDateTime timestamp)
        {
            var typedEnvelope = GenericEnvelope.MakeGenericType(message.Type);
            return (IEnvelope)Activator.CreateInstance(typedEnvelope, message, receiver, sender, timestamp)!;
        }
    }
}
