//--------------------------------------------------------------------------------------------------
// <copyright file="CanBeDefaultAttribute.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Core.Annotations
{
    using System;

    /// <summary>
    /// This decorative attribute indicates that the annotated struct parameter default value is
    /// expected and valid in all cases.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    public sealed class CanBeDefaultAttribute : Attribute
    {
    }
}
