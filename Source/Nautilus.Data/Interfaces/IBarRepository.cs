//--------------------------------------------------------------------------------------------------
// <copyright file="IBarRepository.cs" company="Nautech Systems Pty Ltd">
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

using Nautilus.DomainModel.Enums;
using Nautilus.DomainModel.Frames;
using Nautilus.DomainModel.ValueObjects;

namespace Nautilus.Data.Interfaces
{
    /// <summary>
    /// Provides a repository for accessing <see cref="Bar"/> data.
    /// </summary>
    public interface IBarRepository : IBarRepositoryReadOnly, IRepository
    {
        /// <summary>
        /// Adds the given bars to the repository.
        /// </summary>
        /// <param name="barData">The market data to add.</param>
        void Add(BarDataFrame barData);

        /// <summary>
        /// Adds the given bar to the repository.
        /// </summary>
        /// <param name="barType">The barType to add.</param>
        /// <param name="bar">The bar to add.</param>
        void Add(BarType barType, Bar bar);

        /// <summary>
        /// Adds the given bars to the repository.
        /// </summary>
        /// <param name="barType">The barType to add.</param>
        /// <param name="bars">The bars to add.</param>
        void Add(BarType barType, Bar[] bars);

        /// <summary>
        /// Trims the bars of the given structure held in the repository to equal the given days.
        /// </summary>
        /// <param name="barStructure">The bar resolution to trim.</param>
        /// <param name="days">The number of days (keys) to trim to.</param>
        void TrimToDays(BarStructure barStructure, int days);
    }
}
