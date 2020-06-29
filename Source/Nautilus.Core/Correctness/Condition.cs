//--------------------------------------------------------------------------------------------------
// <copyright file="Condition.cs" company="Nautech Systems Pty Ltd">
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
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Provides checking of function or method conditions which are executed in debug and release
    /// configurations. A condition is a predicate which must be true just prior to the execution of
    /// some section of code - for correct behaviour as per the design specification.
    /// If a check fails an <see cref="Exception"/> is thrown with a descriptive message.
    /// </summary>
    public static class Condition
    {
        /// <summary>
        /// Check the condition predicate is true.
        /// </summary>
        /// <param name="condition">The condition predicate to check.</param>
        /// <param name="description">The description of the condition predicate.</param>
        /// <exception cref="ArgumentException">If the condition is false.</exception>
        public static void True(bool condition, string description)
        {
            if (!condition)
            {
                throw new ArgumentException(FailedMsg.WasFalse(description));
            }
        }

        /// <summary>
        /// Check the condition predicate is false.
        /// </summary>
        /// <param name="condition">The condition predicate to check.</param>
        /// <param name="description">The condition description.</param>
        /// <exception cref="ArgumentException">If the condition is true.</exception>
        public static void False(bool condition, string description)
        {
            if (condition)
            {
                throw new ArgumentException(FailedMsg.WasTrue(description));
            }
        }

        /// <summary>
        /// Check the argument is not null.
        /// </summary>
        /// <param name="argument">The argument to check.</param>
        /// <param name="paramName">The parameter name.</param>
        /// <typeparam name="T">The arguments type.</typeparam>
        /// <exception cref="ArgumentNullException">If the argument is null.</exception>
        public static void NotNull<T>(T argument, string paramName)
        {
            if (argument == null)
            {
                throw new ArgumentNullException(paramName, FailedMsg.WasNull(paramName));
            }
        }

        /// <summary>
        /// Check the argument is not a null, empty or all whitespace string.
        /// </summary>
        /// <param name="argument">The argument to check.</param>
        /// <param name="paramName">The parameter name.</param>
        /// <exception cref="ArgumentNullException">If the string argument is null, empty or white space.</exception>
        public static void NotEmptyOrWhiteSpace(string argument, string paramName)
        {
            if (string.IsNullOrWhiteSpace(argument))
            {
                throw new ArgumentNullException(paramName, FailedMsg.WasNullEmptyOrWhitespace(argument, paramName));
            }
        }

        /// <summary>
        /// Check the argument is not the default value.
        /// </summary>
        /// <typeparam name="T">The argument type.</typeparam>
        /// <param name="argument">The argument to check.</param>
        /// <param name="paramName">The parameter name.</param>
        /// <exception cref="ArgumentException">If the argument is the default value.</exception>
        public static void NotDefault<T>(T argument, string paramName)
            where T : struct
        {
            if (argument.Equals(default(T)))
            {
                throw new ArgumentException(FailedMsg.WasDefault(argument, paramName));
            }
        }

        /// <summary>
        /// Check the argument is not equal to the notToEqual object.
        /// </summary>
        /// <param name="argument">The argument to check.</param>
        /// <param name="notToEqual">The object not to be equal to.</param>
        /// <param name="paramName">The parameter name.</param>
        /// <exception cref="ArgumentException">If the argument equals notToEqual.</exception>
        public static void NotEqualTo(object argument, object notToEqual, string paramName)
        {
            if (argument.Equals(notToEqual))
            {
                throw new ArgumentException(FailedMsg.WasEqualTo(argument, notToEqual, paramName));
            }
        }

        /// <summary>
        /// Check the argument is equal to the toEqual object.
        /// </summary>
        /// <param name="argument">The argument to check.</param>
        /// <param name="toEqual">The object to be equal to.</param>
        /// <param name="paramName">The parameter name.</param>
        /// <exception cref="ArgumentException">If the argument does not equal toEqual.</exception>
        public static void EqualTo(object argument, object toEqual, string paramName)
        {
            if (!argument.Equals(toEqual))
            {
                throw new ArgumentException(FailedMsg.WasNotEqualTo(argument, toEqual, paramName));
            }
        }

        /// <summary>
        /// Check the collection is not empty.
        /// </summary>
        /// <typeparam name="T">The collection type.</typeparam>
        /// <param name="collection">The collection to check.</param>
        /// <param name="paramName">The parameter name.</param>
        /// <exception cref="ArgumentNullException">If the collection is null.</exception>
        /// <exception cref="ArgumentException">If the collection is empty.</exception>
        public static void NotEmpty<T>(IReadOnlyCollection<T> collection, string paramName)
        {
            if (collection is null)
            {
                throw new ArgumentNullException(paramName);
            }

            if (collection.Count == 0)
            {
                throw new ArgumentException(FailedMsg.WasEmptyList(paramName));
            }
        }

        /// <summary>
        /// Check the collection is empty.
        /// </summary>
        /// <typeparam name="T">The collection type.</typeparam>
        /// <param name="collection">The collection to check.</param>
        /// <param name="paramName">The parameter name.</param>
        /// <exception cref="ArgumentNullException">If the collection is null.</exception>
        /// <exception cref="ArgumentException">If the collection is not empty.</exception>
        public static void Empty<T>(IReadOnlyCollection<T> collection, string paramName)
        {
            if (collection is null)
            {
                throw new ArgumentNullException(paramName);
            }

            if (collection.Count != 0)
            {
                throw new ArgumentException(FailedMsg.WasEmptyList(paramName));
            }
        }

        /// <summary>
        /// Check the dictionary is not empty.
        /// </summary>
        /// <typeparam name="TKey">The key type.</typeparam>
        /// <typeparam name="TValue">The value type.</typeparam>
        /// <param name="dictionary">The dictionary to check.</param>
        /// <param name="paramName">The parameter name.</param>
        /// <exception cref="ArgumentNullException">If the dictionary is null.</exception>
        /// <exception cref="ArgumentException">If the dictionary is empty.</exception>
        public static void NotEmpty<TKey, TValue>(Dictionary<TKey, TValue>? dictionary, string paramName)
            where TKey : class
        {
            if (dictionary is null)
            {
                throw new ArgumentNullException(paramName);
            }

            if (dictionary.Count == 0)
            {
                throw new ArgumentException(FailedMsg.WasEmptyDictionary(paramName));
            }
        }

        /// <summary>
        /// Check the dictionary is empty.
        /// </summary>
        /// <typeparam name="TKey">The key type.</typeparam>
        /// <typeparam name="TValue">The value type.</typeparam>
        /// <param name="dictionary">The dictionary to check.</param>
        /// <param name="paramName">The parameter name.</param>
        /// <exception cref="ArgumentNullException">If the dictionary is null.</exception>
        /// <exception cref="ArgumentException">If the dictionary is not empty.</exception>
        public static void Empty<TKey, TValue>(Dictionary<TKey, TValue>? dictionary, string paramName)
            where TKey : class
        {
            if (dictionary is null)
            {
                throw new ArgumentNullException(paramName);
            }

            if (dictionary.Count != 0)
            {
                throw new ArgumentException(FailedMsg.WasEmptyDictionary(paramName));
            }
        }

        /// <summary>
        /// Check the collection contains the given element.
        /// </summary>
        /// <typeparam name="T">The collection type.</typeparam>
        /// <param name="element">The element for the collection to contain.</param>
        /// <param name="collection">The collection to check.</param>
        /// <param name="paramName">The element parameter name.</param>
        /// <param name="collectionName">The collection name.</param>
        /// <exception cref="ArgumentNullException">If the element is null.</exception>
        /// <exception cref="ArgumentNullException">If the collection is null.</exception>
        /// <exception cref="ArgumentException">If the collection does not contain the element.</exception>
        public static void IsIn<T>(T element, ICollection<T> collection, string paramName, string collectionName)
        {
            if (element is null)
            {
                throw new ArgumentNullException(paramName);
            }

            if (collection is null)
            {
                throw new ArgumentNullException(collectionName);
            }

            if (!collection.Contains(element))
            {
                throw new ArgumentException(FailedMsg.WasNotInCollection(element, paramName, collectionName));
            }
        }

        /// <summary>
        /// Check the collection does not contain the given element.
        /// </summary>
        /// <typeparam name="T">The collection type.</typeparam>
        /// <param name="element">The element for the collection not to contain.</param>
        /// <param name="collection">The collection to check.</param>
        /// <param name="paramName">The element parameter name.</param>
        /// <param name="collectionName">The collection name.</param>
        /// <exception cref="ArgumentNullException">If the element is null.</exception>
        /// <exception cref="ArgumentNullException">If the collection is null.</exception>
        /// <exception cref="ArgumentException">If the collection does not contain the element.</exception>
        public static void NotIn<T>(T element, ICollection<T> collection, string paramName, string collectionName)
        {
            if (element is null)
            {
                throw new ArgumentNullException(paramName);
            }

            if (collection is null)
            {
                throw new ArgumentNullException(collectionName);
            }

            if (collection.Contains(element))
            {
                throw new ArgumentException(FailedMsg.WasInCollection(element, paramName, collectionName));
            }
        }

        /// <summary>
        /// Check the dictionary contains the given key.
        /// </summary>
        /// <typeparam name="TKey">The key type.</typeparam>
        /// <typeparam name="TValue">The value type.</typeparam>
        /// <param name="key">The key for the dictionary to contain.</param>
        /// <param name="dictionary">The dictionary to check.</param>
        /// <param name="paramName">The key parameter name.</param>
        /// <param name="dictName">The dictionary name.</param>
        /// <exception cref="ArgumentNullException">If the key is null.</exception>
        /// <exception cref="ArgumentNullException">If the dictionary is null.</exception>
        /// <exception cref="ArgumentException">If the dictionary does not contain the key.</exception>
        public static void KeyIn<TKey, TValue>(TKey key, Dictionary<TKey, TValue>? dictionary, string paramName, string dictName)
            where TKey : class
        {
            if (key is null)
            {
                throw new ArgumentNullException(paramName);
            }

            if (dictionary is null)
            {
                throw new ArgumentNullException(dictName);
            }

            if (!dictionary.ContainsKey(key))
            {
                throw new ArgumentException(FailedMsg.WasNotInDictionary(key, paramName, dictName));
            }
        }

        /// <summary>
        /// Check the dictionary does not contain the given key.
        /// </summary>
        /// <typeparam name="TKey">The key type.</typeparam>
        /// <typeparam name="TValue">The value type.</typeparam>
        /// <param name="key">The key for the dictionary not to contain.</param>
        /// <param name="dictionary">The dictionary to check.</param>
        /// <param name="paramName">The key parameter name.</param>
        /// <param name="dictName">The dictionary name.</param>
        /// <exception cref="ArgumentNullException">If the key is null.</exception>
        /// <exception cref="ArgumentNullException">If the dictionary is null.</exception>
        /// <exception cref="ArgumentException">If the dictionary already contains the key.</exception>
        public static void KeyNotIn<TKey, TValue>(TKey key, Dictionary<TKey, TValue> dictionary, string paramName, string dictName)
            where TKey : class
        {
            if (key is null)
            {
                throw new ArgumentNullException(paramName);
            }

            if (dictionary is null)
            {
                throw new ArgumentNullException(paramName);
            }

            if (dictionary.ContainsKey(key))
            {
                throw new ArgumentException(FailedMsg.WasInDictionary(key, paramName, dictName));
            }
        }

        /// <summary>
        /// Check the value is positive (> 0).
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <param name="paramName">The parameter name.</param>
        /// <exception cref="ArgumentOutOfRangeException">If the value is not positive (> 0).</exception>
        public static void PositiveInt32(int value, string paramName)
        {
            if (value <= 0)
            {
                throw new ArgumentOutOfRangeException(paramName, FailedMsg.WasNotPositive(value, paramName));
            }
        }

        /// <summary>
        /// Check the value is positive (> 0).
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <param name="paramName">The parameter name.</param>
        /// <exception cref="ArgumentOutOfRangeException">If the value is not positive (> 0).</exception>
        public static void PositiveInt64(long value, string paramName)
        {
            if (value <= 0)
            {
                throw new ArgumentOutOfRangeException(paramName, FailedMsg.WasNotPositive(value, paramName));
            }
        }

        /// <summary>
        /// Check the value is positive (> 0).
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <param name="paramName">The parameter name.</param>
        /// <exception cref="ArgumentOutOfRangeException">If the value is not positive (> 0).</exception>
        public static void PositiveDouble(double value, string paramName)
        {
            if (value <= 0)
            {
                throw new ArgumentOutOfRangeException(paramName, FailedMsg.WasNotPositive(value, paramName));
            }
        }

        /// <summary>
        /// Check the value is positive (> 0).
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <param name="paramName">The parameter name.</param>
        /// <exception cref="ArgumentOutOfRangeException">If the value is not positive (> 0).</exception>
        public static void PositiveDecimal(decimal value, string paramName)
        {
            if (value <= decimal.Zero)
            {
                throw new ArgumentOutOfRangeException(paramName, FailedMsg.WasNotPositive(value, paramName));
            }
        }

        /// <summary>
        /// Check the value is greater than or equal to zero.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <param name="paramName">The parameter name.</param>
        /// <exception cref="ArgumentOutOfRangeException">If the value less than zero.</exception>
        public static void NotNegativeInt32(int value, string paramName)
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(paramName, FailedMsg.WasNegative(value, paramName));
            }
        }

        /// <summary>
        /// Check the value is greater than or equal to zero.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <param name="paramName">The parameter name.</param>
        /// <exception cref="ArgumentOutOfRangeException">If the value is less than zero.</exception>
        public static void NotNegativeInt64(long value, string paramName)
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(paramName, FailedMsg.WasNegative(value, paramName));
            }
        }

        /// <summary>
        /// Check the value is greater than or equal to zero.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <param name="paramName">The parameter name.</param>
        /// <exception cref="ArgumentOutOfRangeException">If the value is less than zero.</exception>
        public static void NotNegativeDouble(double value, string paramName)
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(paramName, FailedMsg.WasNegative(value, paramName));
            }
        }

        /// <summary>
        /// Check the value is greater than or equal to zero.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <param name="paramName">The parameter name.</param>
        /// <exception cref="ArgumentOutOfRangeException">If the value is less than zero.</exception>
        public static void NotNegativeDecimal(decimal value, string paramName)
        {
            if (value < decimal.Zero)
            {
                throw new ArgumentOutOfRangeException(paramName, FailedMsg.WasNegative(value, paramName));
            }
        }

        /// <summary>
        /// Check the value is within specified range (inclusive of bounds).
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <param name="lowerBound">The range lower bound (inclusive).</param>
        /// <param name="upperBound">The range upper bound (inclusive).</param>
        /// <param name="paramName">The parameter name.</param>
        /// <exception cref="ArgumentOutOfRangeException">If the value is out of the specified range.</exception>
        public static void NotOutOfRangeInt32(
            int value,
            int lowerBound,
            int upperBound,
            string paramName)
        {
            if (value < lowerBound || value > upperBound)
            {
                throw new ArgumentOutOfRangeException(paramName, FailedMsg.WasOutOfRange(value, lowerBound, upperBound, paramName));
            }
        }

        /// <summary>
        /// Check the value is within specified range (inclusive of bounds).
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <param name="lowerBound">The range lower bound (inclusive).</param>
        /// <param name="upperBound">The range upper bound (inclusive).</param>
        /// <param name="paramName">The parameter name.</param>
        /// <exception cref="ArgumentOutOfRangeException">If the value is out of the specified range.</exception>
        public static void NotOutOfRangeInt64(
            long value,
            long lowerBound,
            long upperBound,
            string paramName)
        {
            if (value < lowerBound || value > upperBound)
            {
                throw new ArgumentOutOfRangeException(paramName, FailedMsg.WasOutOfRange(value, lowerBound, upperBound, paramName));
            }
        }

        /// <summary>
        /// Check the value is within specified range (inclusive of bounds).
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <param name="lowerBound">The range lower bound (inclusive).</param>
        /// <param name="upperBound">The range upper bound (inclusive).</param>
        /// <param name="paramName">The parameter name.</param>
        /// <exception cref="ArgumentOutOfRangeException">If the value is out of the specified range.</exception>
        public static void NotOutOfRangeDouble(
            double value,
            double lowerBound,
            double upperBound,
            string paramName)
        {
            if (value < lowerBound || value > upperBound)
            {
                throw new ArgumentOutOfRangeException(paramName, FailedMsg.WasOutOfRange(value, lowerBound, upperBound, paramName));
            }
        }

        /// <summary>
        /// Check the value is within specified range (inclusive of bounds).
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <param name="lowerBound">The range lower bound (inclusive).</param>
        /// <param name="upperBound">The range upper bound (inclusive).</param>
        /// <param name="paramName">The parameter name.</param>
        /// <exception cref="ArgumentOutOfRangeException">If the value is out of the specified range.</exception>
        public static void NotOutOfRangeDecimal(
            decimal value,
            decimal lowerBound,
            decimal upperBound,
            string paramName)
        {
            if (value < lowerBound || value > upperBound)
            {
                throw new ArgumentOutOfRangeException(paramName, FailedMsg.WasOutOfRange(value, lowerBound, upperBound, paramName));
            }
        }

        /// <summary>
        /// Check the object has the specified attribute. Exists as constraints do not allow checking of attributes.
        /// </summary>
        /// <typeparam name="TAttribute">The attribute to match.</typeparam>
        /// <param name="argument">The argument to check.</param>
        /// <param name="paramName">The parameter name.</param>
        /// <exception cref="ArgumentException">If the argument does not have the attribute.</exception>
        public static void HasAttribute<TAttribute>(Type argument, string paramName)
            where TAttribute : Attribute
        {
            var attributes = argument.GetCustomAttributes(typeof(TAttribute), true);
            if (attributes.Length == 0)
            {
                throw new ArgumentException(FailedMsg.DidNotHaveAttribute(typeof(TAttribute).Name, paramName));
            }
        }
    }
}
