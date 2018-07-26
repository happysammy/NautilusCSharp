//--------------------------------------------------------------------------------------------------
// <copyright file="CanBeDefaultAttribute.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Core.Annotations
{
    using System;

    /// <summary>
    /// This decorative attribute indicates that the annotated struct parameter could be the default
    /// value (therefore an explicit check for default is not required).
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    public sealed class CanBeDefaultAttribute : Attribute
    {
    }
}
