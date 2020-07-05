//--------------------------------------------------------------------------------------------------
// <copyright file="FailedMsg.cs" company="Nautech Systems Pty Ltd">
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
        /// <param name="description">The condition predicate description.</param>
        /// <returns>The exception string.</returns>
        public static string WasFalse(string description)
        {
            return $"The condition predicate {description} was false.";
        }

        /// <summary>
        /// Return the check failed message.
        /// </summary>
        /// <param name="description">The condition predicate description.</param>
        /// <returns>The exception string.</returns>
        public static string WasTrue(string description)
        {
            return $"The condition predicate {description} was true.";
        }

        /// <summary>
        /// Return the check failed message.
        /// </summary>
        /// <param name="paramName">The parameter being checked.</param>
        /// <returns>The exception string.</returns>
        public static string WasNull(string paramName)
        {
            return $"The {paramName} was null.";
        }

        /// <summary>
        /// Return the check failed message.
        /// </summary>
        /// <param name="value">The argument value.</param>
        /// <param name="paramName">The parameter name being checked.</param>
        /// <returns>The exception string.</returns>
        public static string WasNullEmptyOrWhitespace(string value, string paramName)
        {
            var valueString = value != null ? $"'{value}'" : "null";
            return $"The {paramName} was null, empty or whitespace, was {valueString}.";
        }

        /// <summary>
        /// Return the check failed message.
        /// </summary>
        /// <param name="value">The argument value being checked.</param>
        /// <param name="paramName">The parameter name being checked.</param>
        /// <returns>The exception string.</returns>
        public static string WasDefault(object value, string paramName)
        {
            return $"The {paramName} was default, was {value}.";
        }

        /// <summary>
        /// Return the check failed message.
        /// </summary>
        /// <param name="value">The argument value being checked.</param>
        /// <param name="toEqual">The value the argument should not have equaled.</param>
        /// <param name="paramName">The parameter name being checked.</param>
        /// <returns>The exception string.</returns>
        public static string WasEqualTo(object value, object toEqual, string paramName)
        {
            return $"The {paramName} was equal to {toEqual}, was {value}.";
        }

        /// <summary>
        /// Return the check failed message.
        /// </summary>
        /// <param name="value">The argument value being checked.</param>
        /// <param name="toEqual">The value the argument should have equaled.</param>
        /// <param name="paramName">The parameter name being checked.</param>
        /// <returns>The exception string.</returns>
        public static string WasNotEqualTo(object value, object toEqual, string paramName)
        {
            return $"The {paramName} was not equal to {toEqual}, was {value}.";
        }

        /// <summary>
        /// Return the check failed message.
        /// </summary>
        /// <param name="paramName">The parameter name being checked.</param>
        /// <returns>The exception string.</returns>
        public static string WasEmptyList(string paramName)
        {
            return $"The {paramName} list was empty.";
        }

        /// <summary>
        /// Return the check failed message.
        /// </summary>
        /// <param name="paramName">The parameter name being checked.</param>
        /// <returns>The exception string.</returns>
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
        /// <returns>The exception string.</returns>
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
        /// <returns>The exception string.</returns>
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
        /// <returns>The exception string.</returns>
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
        /// <returns>The exception string.</returns>
        public static string WasInDictionary(object key, string paramName, string dictName)
        {
            return $"The {key} {paramName} key was already in the {dictName} dictionary.";
        }

        /// <summary>
        /// Return the check failed message.
        /// </summary>
        /// <param name="value">The argument value being checked.</param>
        /// <param name="paramName">The parameter name being checked.</param>
        /// <returns>The exception string.</returns>
        public static string WasNotPositive(object value, string paramName)
        {
            return $"The {paramName} was not positive (> 0), was {value}.";
        }

        /// <summary>
        /// Return the check failed message.
        /// </summary>
        /// <param name="value">The argument value being checked.</param>
        /// <param name="paramName">The parameter name being checked.</param>
        /// <returns>The exception string.</returns>
        public static string WasNegative(object value, string paramName)
        {
            return $"The {paramName} was not greater than or equal to zero (>= 0), was {value}.";
        }

        /// <summary>
        /// Return the check failed message.
        /// </summary>
        /// <param name="value">The argument value being checked.</param>
        /// <param name="lowerBound">The lower bound of the range.</param>
        /// <param name="upperBound">The upper bound of the range.</param>
        /// <param name="paramName">The parameter name being checked.</param>
        /// <returns>The exception string.</returns>
        public static string WasOutOfRange(
            object value,
            object lowerBound,
            object upperBound,
            string paramName)
        {
            return $"The {paramName} was out of range [{lowerBound}, {upperBound}], was {value}.";
        }

        /// <summary>
        /// Return the check failed message.
        /// </summary>
        /// <param name="attributeName">The missing attribute.</param>
        /// <param name="paramName">The parameter name being checked.</param>
        /// <returns>The exception string.</returns>
        public static string DidNotHaveAttribute(string attributeName, string paramName)
        {
            return $"The {paramName} did not have the {attributeName} attribute.";
        }
    }
}
