//--------------------------------------------------------------------------------------------------
// <copyright file="InvalidValueAttribute.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Core.Annotations
{
    using System;

    /// <summary>
    /// This decorative attribute indicates that the annotated field value is not valid or expected.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class InvalidValueAttribute : Attribute
    {
    }
}
