//--------------------------------------------------------------------------------------------------
// <copyright file="FailedMsg.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Core.Correctness
{
    /// <summary>
    /// Provides common debug assertion messages.
    /// </summary>
    public static class FailedMsg
    {
        /// <summary>
        /// Return the check failed message.
        /// </summary>
        /// <param name="paramName">The parameter being checked.</param>
        /// <returns>The string.</returns>
        public static string WasNull(string paramName)
        {
            return $"The {paramName} was null.";
        }

        /// <summary>
        /// Return the check failed message.
        /// </summary>
        /// <param name="paramName">The parameter name being checked.</param>
        /// <returns>The string.</returns>
        public static string WasNullEmptyOrWhitespace(string paramName)
        {
            return $"The {paramName} was null, empty or whitespace.";
        }

        /// <summary>
        /// Return the check failed message.
        /// </summary>
        /// <param name="value">The argument value being checked.</param>
        /// <param name="paramName">The parameter name being checked.</param>
        /// <returns>The string.</returns>
        public static string WasDefault(object value, string paramName)
        {
            return $"The {paramName} was default, value = {value}.";
        }

        /// <summary>
        /// Return the check failed message.
        /// </summary>
        /// <param name="value">The argument value being checked.</param>
        /// <param name="toEqual">The value the argument should not have equaled.</param>
        /// <param name="paramName">The parameter name being checked.</param>
        /// <returns>The string.</returns>
        public static string WasEqualTo(object value, object toEqual, string paramName)
        {
            return $"The {paramName} was equal to {toEqual}.";
        }

        /// <summary>
        /// Return the check failed message.
        /// </summary>
        /// <param name="value">The argument value being checked.</param>
        /// <param name="toEqual">The value the argument should have equaled.</param>
        /// <param name="paramName">The parameter name being checked.</param>
        /// <returns>The string.</returns>
        public static string WasNotEqualTo(object value, object toEqual, string paramName)
        {
            return $"The {paramName} was not equal to {toEqual}, value = {value}.";
        }

        /// <summary>
        /// Return the check failed message.
        /// </summary>
        /// <param name="paramName">The parameter name being checked.</param>
        /// <returns>The string.</returns>
        public static string WasEmptyList(string paramName)
        {
            return $"The {paramName} list was empty.";
        }

        /// <summary>
        /// Return the check failed message.
        /// </summary>
        /// <param name="paramName">The parameter name being checked.</param>
        /// <returns>The string.</returns>
        public static string WasEmptyDictionary(string paramName)
        {
            return $"The {paramName} dictionary was empty.";
        }

        /// <summary>
        /// Return the check failed message.
        /// </summary>
        /// <param name="element">The element being searched for.</param>
        /// <param name="paramName">The parameter name being checked.</param>
        /// <param name="collectionName">The collection being checked.</param>
        /// <returns>The string.</returns>
        public static string WasNotInCollection(object element, string paramName, string collectionName)
        {
            return $"The {element} {paramName} was not found in the {collectionName} collection.";
        }

        /// <summary>
        /// Return the check failed message.
        /// </summary>
        /// <param name="element">The element being searched for.</param>
        /// <param name="paramName">The parameter name being checked.</param>
        /// <param name="collectionName">The collection being checked.</param>
        /// <returns>The string.</returns>
        public static string WasInCollection(object element, string paramName, string collectionName)
        {
            return $"The {element} {paramName} was already in the {collectionName} collection.";
        }

        /// <summary>
        /// Return the check failed message.
        /// </summary>
        /// <param name="key">The key being searched for.</param>
        /// <param name="paramName">The parameter name being checked.</param>
        /// <param name="dictName">The dictionary being checked.</param>
        /// <returns>The string.</returns>
        public static string WasNotInDictionary(object key, string paramName, string dictName)
        {
            return $"The {key} {paramName} key was not found in the {dictName} dictionary.";
        }

        /// <summary>
        /// Return the check failed message.
        /// </summary>
        /// <param name="key">The key being searched for.</param>
        /// <param name="paramName">The parameter name being checked.</param>
        /// <param name="dictName">The dictionary being checked.</param>
        /// <returns>The string.</returns>
        public static string WasInDictionary(object key, string paramName, string dictName)
        {
            return $"The {key} {paramName} key was already in the {dictName} dictionary.";
        }

        /// <summary>
        /// Return the check failed message.
        /// </summary>
        /// <param name="value">The argument value being checked.</param>
        /// <param name="paramName">The parameter name being checked.</param>
        /// <returns>The string.</returns>
        public static string WasNotPositive(object value, string paramName)
        {
            return $"The {paramName} was not positive (> 0), value = {value}.";
        }

        /// <summary>
        /// Return the check failed message.
        /// </summary>
        /// <param name="value">The argument value being checked.</param>
        /// <param name="paramName">The parameter name being checked.</param>
        /// <returns>The string.</returns>
        public static string WasNegative(object value, string paramName)
        {
            return $"The {paramName} was negative, value = {value}.";
        }

        /// <summary>
        /// Return the check failed message.
        /// </summary>
        /// <param name="value">The argument value being checked.</param>
        /// <param name="lowerBound">The lower bound of the range.</param>
        /// <param name="upperBound">The upper bound of the range.</param>
        /// <param name="paramName">The parameter name being checked.</param>
        /// <returns>The string.</returns>
        public static string WasOutOfRange(
            object value,
            object lowerBound,
            object upperBound,
            string paramName)
        {
            return $"The {paramName} was out of range [{lowerBound}, {upperBound}], value = {value}.";
        }
    }
}
