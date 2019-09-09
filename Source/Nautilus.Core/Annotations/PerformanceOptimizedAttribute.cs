//--------------------------------------------------------------------------------------------------
// <copyright file="PerformanceOptimizedAttribute.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Core.Annotations
{
    using System;

    /// <summary>
    /// This decorative attribute indicates that the annotated method has been performance
    /// optimized, therefore there is a tendency towards low level implementations over
    /// typical OO abstractions as follows;
    /// - primitive types
    /// - concrete types (avoiding interface dispatch overhead)
    /// - arrays
    /// - for loops
    ///
    /// To meet its design specifications of performance as a priority, avoid refactoring of this
    /// class, struct or method towards increasing OO abstractions for code readability and
    /// understanding.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class PerformanceOptimizedAttribute : Attribute
    {
    }
}
