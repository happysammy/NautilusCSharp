//--------------------------------------------------------------------------------------------------
// <copyright file="PerformanceOptimizedAttribute.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Core.Annotations
{
    using System;

    /// <summary>
    /// This decorative attribute indicates that the annotated class, struct or method has been
    /// performance optimized (therefore there is a tendency towards low level implementations such
    /// as primitive types, arrays and for loops - over typical OO abstractions).
    ///
    /// To meet its design specifications of performance as a priority, refactoring of this method
    /// towards increasing OO abstractions for code readability and understanding should be avoided.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Method)]
    public sealed class PerformanceOptimizedAttribute : Attribute
    {
    }
}
