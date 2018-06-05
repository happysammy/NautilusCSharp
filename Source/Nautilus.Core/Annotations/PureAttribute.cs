//--------------------------------------------------------------------------------------------------
// <copyright file="PureAttribute.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the Apache 2.0 license
//  as found in the LICENSE.txt file.
//  https://github.com/nautechsystems/Nautilus.Core
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Core.Annotations
{
    using System;

    /// <summary>
    /// This decorative attribute indicates that the annotated method is pure, and thus has no side
    /// effects.
    /// </summary>
    /// <remarks>
    ///     Properties of a pure function include;
    ///     - Does not produce side effects (including assigning a variable, or throwing an exception).
    ///     - Does not mutate the input arguments in any way.
    ///     - Does not depend on anything external to itself (or reference any global variable).
    ///     - Produces the same output for a given input.
    ///     - Is stateless.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class PureAttribute : Attribute
    {
    }
}
