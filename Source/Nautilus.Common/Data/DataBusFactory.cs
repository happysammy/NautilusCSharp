// -------------------------------------------------------------------------------------------------
// <copyright file="DataBusFactory.cs" company="Nautech Systems Pty Ltd">
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

using Nautilus.Common.Interfaces;
using Nautilus.DomainModel.Entities;
using Nautilus.DomainModel.ValueObjects;

namespace Nautilus.Common.Data
{
    /// <summary>
    /// Provides a factory to create the systems data bus.
    /// </summary>
    public static class DataBusFactory
    {
        /// <summary>
        /// Creates and returns a new message bus adapter.
        /// </summary>
        /// <param name="container">The componentry container.</param>
        /// <returns>The messaging adapter.</returns>
        public static DataBusAdapter Create(IComponentryContainer container)
        {
            return new DataBusAdapter(
                new DataBus<Tick>(container),
                new DataBus<BarData>(container),
                new DataBus<Instrument>(container));
        }
    }
}
