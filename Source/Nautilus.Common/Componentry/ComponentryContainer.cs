//--------------------------------------------------------------------------------------------------
// <copyright file="ComponentryContainer.cs" company="Nautech Systems Pty Ltd">
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

namespace Nautilus.Common.Componentry
{
    using Microsoft.Extensions.Logging;
    using Nautilus.Common.Interfaces;
    using Nautilus.Core.Annotations;

    /// <summary>
    /// The setup componentry container for <see cref="Nautilus"/> systems.
    /// </summary>
    [Immutable]
    public sealed class ComponentryContainer : IComponentryContainer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ComponentryContainer"/> class.
        /// </summary>
        /// <param name="clock">The container clock.</param>
        /// <param name="guidFactory">The container GUID factory.</param>
        /// <param name="loggerFactory">The container logger factory.</param>
        public ComponentryContainer(
            IZonedClock clock,
            IGuidFactory guidFactory,
            ILoggerFactory loggerFactory)
        {
            this.Clock = clock;
            this.GuidFactory = guidFactory;
            this.LoggerFactory = loggerFactory;
        }

        /// <inheritdoc />
        public IZonedClock Clock { get; }

        /// <inheritdoc />
        public IGuidFactory GuidFactory { get; }

        /// <inheritdoc />
        public ILoggerFactory LoggerFactory { get; }
    }
}
