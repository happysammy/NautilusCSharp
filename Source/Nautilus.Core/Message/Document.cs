//--------------------------------------------------------------------------------------------------
// <copyright file="Document.cs" company="Nautech Systems Pty Ltd">
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
using Nautilus.Core.Annotations;
using Nautilus.Core.Correctness;
using Nautilus.Core.Enums;
using NodaTime;

namespace Nautilus.Core.Message
{
    /// <summary>
    /// The base class for all <see cref="Document"/> messages.
    /// </summary>
    [Immutable]
    public abstract class Document : Types.Message
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Document"/> class.
        /// </summary>
        /// <param name="type">The document type.</param>
        /// <param name="id">The document identifier.</param>
        /// <param name="timestamp">The document timestamp.</param>
        protected Document(
            Type type,
            Guid id,
            ZonedDateTime timestamp)
            : base(
                MessageType.Document,
                type,
                id,
                timestamp)
        {
            Debug.NotDefault(id, nameof(id));
            Debug.NotDefault(timestamp, nameof(timestamp));
        }
    }
}
