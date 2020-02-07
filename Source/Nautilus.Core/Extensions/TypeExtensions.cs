//--------------------------------------------------------------------------------------------------
// <copyright file="TypeExtensions.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Core.Extensions
{
    using System;

    /// <summary>
    /// Provides useful generic <see cref="Type"/> extension methods.
    /// </summary>
    public static class TypeExtensions
    {
        /// <summary>
        /// Returns the formatted name string of the given type.
        /// </summary>
        /// <param name="type">The type to extract the name from.</param>
        /// <returns>The name <see cref="string"/>.</returns>
        public static string NameFormatted(this Type type)
        {
            return type.IsGenericType
                ? ParseGenericName(type)
                : type.Name;
        }

        private static string ParseGenericName(Type type)
        {
            var typeName = type.Name.Split('`')[0];
            var genericArgs = type.GenericTypeArguments;
            var genericName = genericArgs[0].Name;

            if (genericArgs.Length > 1)
            {
                for (var i = 1; i < genericArgs.Length; i++)
                {
                    genericName += $",{genericArgs[i].Name}";
                }
            }

            return typeName + $"<{genericName}>";
        }
    }
}
