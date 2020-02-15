//--------------------------------------------------------------------------------------------------
// <copyright file="ArrayCreator{T}.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Core
{
    using System.Collections.Generic;

    /// <summary>
    /// Provides an efficient way of converting between types.
    /// </summary>
    /// <typeparam name="T">The array type.</typeparam>
    public static class ArrayCreator<T>
    {
        /// <summary>
        /// Convert the given list to an array of the same type.
        /// </summary>
        /// <param name="list">The list to convert.</param>
        /// <returns>The converted array.</returns>
        public static T[] ToArray(List<T> list)
        {
            var array = new T[list.Count];
            for (var i = 0; i < list.Count; i++)
            {
                array[i] = list[i];
            }

            return array;
        }
    }
}
