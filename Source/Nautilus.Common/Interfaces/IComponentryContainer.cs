//--------------------------------------------------------------------------------------------------
// <copyright file="IComponentryContainer.cs" company="Nautech Systems Pty Ltd">
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

using Microsoft.Extensions.Logging;

namespace Nautilus.Common.Interfaces
{
    /// <summary>
    /// The componentry container for constructing service components.
    /// </summary>
    public interface IComponentryContainer
    {
        /// <summary>
        /// Gets the containers clock.
        /// </summary>
        IZonedClock Clock { get; }

        /// <summary>
        /// Gets the containers GUID factory.
        /// </summary>
        IGuidFactory GuidFactory { get; }

        /// <summary>
        /// Gets the containers logger.
        /// </summary>
        ILoggerFactory LoggerFactory { get; }
    }
}
