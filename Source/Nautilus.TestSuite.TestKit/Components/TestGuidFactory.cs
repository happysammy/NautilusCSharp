//--------------------------------------------------------------------------------------------------
// <copyright file="TestGuidFactory.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.TestKit.Components
{
    using System;
    using Nautilus.Common.Interfaces;

    /// <inheritdoc/>
    public sealed class TestGuidFactory : IGuidFactory
    {
        private readonly Guid guid = Guid.Parse("3532d5de-f67f-4a8d-9c42-e1002fa6733b");

        /// <inheritdoc/>
        public Guid Generate()
        {
            return this.guid;
        }
    }
}
