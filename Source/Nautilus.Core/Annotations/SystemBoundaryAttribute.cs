//--------------------------------------------------------------------------------------------------
// <copyright file="SystemBoundaryAttribute.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Core.Annotations
{
    using System;

    /// <summary>
    /// This decorative attribute indicates that the annotated method is at the service boundary,
    /// all input argument data passed into the method should be fully validated to preserve the
    /// systems design specification.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class SystemBoundaryAttribute : Attribute
    {
    }
}
