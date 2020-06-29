//--------------------------------------------------------------------------------------------------
// <copyright file="Response.cs" company="Nautech Systems Pty Ltd">
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

namespace Nautilus.Core.Message
{
    using System;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Correctness;
    using Nautilus.Core.Enums;
    using Nautilus.Core.Types;
    using NodaTime;

    /// <summary>
    /// The base class for all <see cref="Response"/> messages.
    /// </summary>
    [Immutable]
    public abstract class Response : Message
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Response"/> class.
        /// </summary>
        /// <param name="type">The response type.</param>
        /// <param name="correlationId">The response correlation identifier.</param>
        /// <param name="id">The response identifier.</param>
        /// <param name="timestamp">The response timestamp.</param>
        protected Response(
            Type type,
            Guid correlationId,
            Guid id,
            ZonedDateTime timestamp)
            : base(
                MessageType.Response,
                type,
                id,
                timestamp)
        {
            Debug.NotDefault(id, nameof(id));
            Debug.NotDefault(timestamp, nameof(timestamp));

            this.CorrelationId = correlationId;
        }

        /// <summary>
        /// Gets the responses correlation identifier.
        /// </summary>
        public Guid CorrelationId { get; }

        /// <summary>
        /// Returns the string representation of this object.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public override string ToString() => $"{this.Type.Name}(CorrelationId={this.CorrelationId})";
    }
}
