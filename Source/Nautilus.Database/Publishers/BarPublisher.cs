//--------------------------------------------------------------------------------------------------
// <copyright file="BarPublisher.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Database.Publishers
{
    using Nautilus.Database.Interfaces;
    using Nautilus.DomainModel.Events;
    using Nautilus.DomainModel.ValueObjects;

    /// <summary>
    /// Providers a protobuf implementation for the <see cref="Bar"/> publisher.
    /// </summary>
    public class BarPublisher : IBarPublisher
    {
        public void Publish(BarDataEvent barEvent)
        {
            throw new System.NotImplementedException();
        }
    }
}
