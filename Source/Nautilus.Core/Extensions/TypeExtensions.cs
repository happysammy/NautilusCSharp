//--------------------------------------------------------------------------------------------------
// <copyright file="TypeExtensions.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  https://nautechsystems.io
//
//  Licensed under the GNU Lesser General Public License Version 3.0 (the "License");
//  You may not use this file except in compliance with the License.
//  You may obtain a copy of the License at https://www.gnu.org/licenses/lgpl-3.0.en.html
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
// </copyright>
//--------------------------------------------------------------------------------------------------

using System;
using System.Text;

namespace Nautilus.Core.Extensions
{
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
                ? GetGenericName(type)
                : type.Name;
        }

        private static string GetGenericName(Type type)
        {
            var builder1 = new StringBuilder(type.Name.Split('`')[0]);
            var genericArgs = type.GenericTypeArguments;
            var builder2 = new StringBuilder(genericArgs[0].Name);

            if (genericArgs.Length > 1)
            {
                for (var i = 1; i < genericArgs.Length; i++)
                {
                    builder2.Append($",{genericArgs[i].Name}");
                }
            }

            return builder1.Append($"<{builder2}>").ToString();
        }
    }
}
