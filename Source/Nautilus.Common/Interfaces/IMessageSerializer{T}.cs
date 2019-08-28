//--------------------------------------------------------------------------------------------------
// <copyright file="IMessageSerializer{T}.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Interfaces
{
    using Nautilus.Core.Types;

    /// <summary>
    /// Provides a binary serializer for <see cref="Message"/>s of type T.
    /// </summary>
    /// <typeparam name="T">The <see cref="Message"/> type.</typeparam>
    public interface IMessageSerializer<T> : ISerializer<T>
        where T : Message
    {
    }
}
