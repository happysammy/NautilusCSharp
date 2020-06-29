//--------------------------------------------------------------------------------------------------
// <copyright file="ITickRepository.cs" company="Nautech Systems Pty Ltd">
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

namespace Nautilus.Data.Interfaces
{
    using System.Collections.Generic;
    using Nautilus.DomainModel.ValueObjects;

    /// <summary>
    /// Provides a repository for accessing <see cref="Tick"/> data.
    /// </summary>
    public interface ITickRepository : ITickRepositoryReadOnly, IRepository
    {
        /// <summary>
        /// Add the given tick to the repository.
        /// </summary>
        /// <param name="tick">The tick to add.</param>
        void Add(Tick tick);

        /// <summary>
        /// Add the given ticks to the repository.
        /// </summary>
        /// <param name="ticks">The ticks to add.</param>
        void Add(List<Tick> ticks);

        /// <summary>
        /// Trims each symbols tick data in the repository to equal the given days.
        /// </summary>
        /// <param name="trimToDays">The number of days (keys) to trim to.</param>
        void TrimToDays(int trimToDays);
    }
}
