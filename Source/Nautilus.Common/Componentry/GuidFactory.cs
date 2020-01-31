﻿//--------------------------------------------------------------------------------------------------
// <copyright file="GuidFactory.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Componentry
{
    using System;
    using Nautilus.Common.Interfaces;

    /// <summary>
    /// Provides <see cref="Guid"/>(s) for the service.
    /// </summary>
    public sealed class GuidFactory : IGuidFactory
    {
        /// <inheritdoc />
        public Guid Generate()
        {
            return Guid.NewGuid();
        }
    }
}
