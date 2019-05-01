//--------------------------------------------------------------------------------------------------
// <copyright file="AssertMsg.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Core
{
    /// <summary>
    /// Provides common debug assertion messages.
    /// </summary>
    public static class AssertMsg
    {
        /// <summary>
        /// Return the debug assert message for the given argument.
        /// </summary>
        /// <param name="paramName">The parameter being asserted.</param>
        /// <returns>A string.</returns>
        public static string IsNull(string paramName)
        {
            return $"the {paramName} argument cannot be null.";
        }

        /// <summary>
        /// Return the debug assert message for the given argument.
        /// </summary>
        /// <param name="paramName">The parameter being asserted.</param>
        /// <returns>A string.</returns>
        public static string IsNullOrWhitespace(string paramName)
        {
            return $"the {paramName} argument cannot be null, empty or whitespace.";
        }

        /// <summary>
        /// Return the debug assert message for the given argument.
        /// </summary>
        /// <param name="paramName">The parameter being asserted.</param>
        /// <returns>A string.</returns>
        public static string IsDefault(string paramName)
        {
            return $"the {paramName} argument cannot be the default value.";
        }
    }
}
