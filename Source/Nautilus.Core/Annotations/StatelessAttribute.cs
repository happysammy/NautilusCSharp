//--------------------------------------------------------------------------------------------------
// <copyright file="StatelessAttribute.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Core.Annotations
{
    using System;

    /// <summary>
    /// This decorative attribute indicates that the annotated class should be completely stateless
    /// to fulfill its design specification. The internal state of the object is set at
    /// construction and no subsequent modifications are allowed.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class StatelessAttribute : Attribute
    {
    }
}
