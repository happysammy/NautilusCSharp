// -------------------------------------------------------------------------------------------------
// <copyright file="IExecutionDatabase.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   https://nautechsystems.io
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Execution.Interfaces
{
    using Nautilus.Core.CQS;
    using Nautilus.DomainModel.Aggregates;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.Identifiers;

    /// <summary>
    /// Provides an execution database for persisting execution related data.
    /// </summary>
    public interface IExecutionDatabase : IExecutionDatabaseRead, IExecutionDatabaseWrite
    {
    }
}
