//--------------------------------------------------------------------------------------------------
// <copyright file="ConfigReader.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.BlackBox.Core.Setup
{
    using System;
    using System.Collections.Specialized;
    using NautechSystems.CSharp.Extensions;
    using NautechSystems.CSharp.Validation;

    /// <summary>
    /// The static <see cref="ConfigReader"/> class. Returns the configuration value of a given
    /// <see cref="NameValueCollection"/>.
    /// </summary>
    public static class ConfigReader
    {
        /// <summary>
        /// Returns the string value of the given argument name from the given
        /// <see cref="NameValueCollection"/>.
        /// </summary>
        /// <param name="arguments">The arguments.</param>
        /// <param name="argumentName">The argument name.</param>
        /// <returns>A <see cref="string"/>.</returns>
        public static string GetArgumentValue(NameValueCollection arguments, string argumentName)
        {
            Validate.NotNull(arguments, nameof(arguments));
            Validate.NotNull(argumentName, nameof(argumentName));

            var argument = arguments[argumentName];

            if (string.IsNullOrEmpty(argument))
            {
                throw new ArgumentException($"Cannot find {argumentName} in {arguments}");
            }

            if (!string.IsNullOrEmpty(argument))
            {
                argument = argument.RemoveAllWhitespace();
            }

            Debug.NotNull(argument, nameof(argument));

            return argument;
        }
    }
}