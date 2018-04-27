// -------------------------------------------------------------------------------------------------
// <copyright file="IDatabaseAdapter.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.BlackBox.Core.Interfaces
{
    using System.Linq;

    /// <summary>
    /// The <see cref="IDatabaseAdapter"/> interface.
    /// </summary>
    public interface IDatabaseAdapter
    {
        /// <summary>
        /// Opens the connection to the database.
        /// </summary>
        void OpenConnection();

        /// <summary>
        /// Sends a request for the given query to the database.
        /// </summary>
        /// <typeparam name="T">The query type.</typeparam>
        /// <returns>A <see cref="IQueryable"/>.</returns>
        IQueryable<T> Query<T>();

        /// <summary>
        /// Deletes the given entity from the database.
        /// </summary>
        /// <param name="entity">The entity.</param>
        void Delete(object entity);

        /// <summary>
        /// Stores the given entity in the database.
        /// </summary>
        /// <param name="entity">The entity.</param>
        void Store(object entity);

        /// <summary>
        /// Saves any changes to the database.
        /// </summary>
        void SaveChanges();

        /// <summary>
        /// Disposes the database memory allocation.
        /// </summary>
        void Dispose();
    }
}