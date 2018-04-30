// -------------------------------------------------------------------------------------------------
// <copyright file="RavenDbAdapter.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.RavenDB
{
    using System.Linq;
    using Nautilus.BlackBox.Core.Interfaces;
    using Raven.Client.Documents;
    using Raven.Client.Documents.Session;

    /// <summary>
    /// The <see cref="RavenDbAdapter"/> class.
    /// </summary>
    public class RavenDbAdapter : IDatabaseAdapter
    {
        private readonly IDocumentStore documentStore;
        private IDocumentSession documentSession;

        /// <summary>
        /// Initializes a new instance of the <see cref="RavenDbAdapter"/> class.
        /// </summary>
        /// <param name="storeName">
        /// The store Name.
        /// </param>
        public RavenDbAdapter(string storeName)
        {
//            this.documentStore = new DocumentStore()
//                                     {
//                                         Url = "http://localhost:8080",
//                                         DefaultDatabase = storeName,
//                                     }.Initialize();
        }

        /// <summary>
        /// The open connection.
        /// </summary>
        public void OpenConnection()
        {
            this.documentSession = this.documentStore.OpenSession();
        }

        /// <summary>
        /// The store.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        public void Store(object entity)
        {
            this.documentSession.Store(entity);
        }

        /// <summary>
        /// The query.
        /// </summary>
        /// <typeparam name="T">
        /// The type.
        /// </typeparam>
        /// <returns>
        /// The <see cref="IQueryable"/>.
        /// </returns>
        public IQueryable<T> Query<T>()
        {
            return this.documentSession.Query<T>();
        }

        /// <summary>
        /// The delete.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        public void Delete(object entity)
        {
            this.documentSession.Delete(entity);
        }

        /// <summary>
        /// The save changes.
        /// </summary>
        public void SaveChanges()
        {
            this.documentSession.SaveChanges();
        }

        /// <summary>
        /// The dispose.
        /// </summary>
        public void Dispose()
        {
            this.documentSession.Dispose();
            this.documentStore.Dispose();
        }
    }
}
