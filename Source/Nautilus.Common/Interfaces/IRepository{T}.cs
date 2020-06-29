//--------------------------------------------------------------------------------------------------
// <copyright file="IRepository{T}.cs" company="Nautech Systems Pty Ltd">
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

namespace Nautilus.Common.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using Nautilus.Core.CQS;

    /// <summary>
    /// Provides a generic repository of type T.
    /// </summary>
    /// <typeparam name="T">The repository type T.</typeparam>
    public interface IRepository<T>
    {
        /// <summary>
        /// Adds the given entity to the repository.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        /// <returns>The result of the operation.</returns>
        CommandResult Add(T entity);

        /// <summary>
        /// Deletes the given entity from the repository.
        /// </summary>
        /// <param name="entity">The entity to delete.</param>
        /// <returns>The result of the operation.</returns>
        CommandResult Delete(T entity);

        /// <summary>
        /// Updates the given entity within the repository.
        /// </summary>
        /// <param name="entity">The entity to update.</param>
        /// <returns>The result of the operation.</returns>
        CommandResult Update(T entity);

        /// <summary>
        /// Returns the entity which satisfies the given predicate.
        /// </summary>
        /// <param name="predicate">The query expression.</param>
        /// <returns>The result of the query.</returns>
        QueryResult<T> GetSingle(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Returns the collection of entities which satisfy the given predicate.
        /// </summary>
        /// <param name="predicate">The query expression.</param>
        /// <returns>The result of the query.</returns>
        QueryResult<IReadOnlyCollection<T>> GetAll(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Returns all entities from the repository of type T.
        /// </summary>
        /// <returns>The result of the query.</returns>
        QueryResult<IReadOnlyCollection<T>> GetAll();

        /// <summary>
        /// Returns the count of entities within the repository which satisfy the given predicate.
        /// </summary>
        /// <param name="predicate">The query expression.</param>
        /// <returns>The count <see cref="int"/>.</returns>
        int Count(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Returns the total count of entities held in the repository.
        /// </summary>
        /// <returns>The count <see cref="int"/>.</returns>
        int Count();
    }
}
