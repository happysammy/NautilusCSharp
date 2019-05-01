//--------------------------------------------------------------------------------------------------
// <copyright file="Precondition.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Core.Correctness
{
    using System;
    using System.Collections.Generic;
    using Nautilus.Core.Annotations;

    /// <summary>
    /// Provides precondition checking methods which are executed in release configuration.
    /// If the check passes then the method does nothing. If the check fails a type of
    /// <see cref="ArgumentException"/> is thrown with a message.
    /// </summary>
    [Immutable]
    public static class Precondition
    {
        /// <summary>
        /// The check passes if the predicate is true.
        /// </summary>
        /// <param name="predicate">The predicate under check.</param>
        /// <param name="paramName">The parameter name.</param>
        /// <exception cref="ArgumentException">If the predicate is false.</exception>
        public static void True(bool predicate, string paramName)
        {
            if (!predicate)
            {
                throw new ArgumentException(FailedMsg.WasFalse(paramName));
            }
        }

        /// <summary>
        /// The check passes if the condition is false, or both the condition and predicate are
        /// true.
        /// </summary>
        /// <param name="condition">The condition under check.</param>
        /// <param name="predicate">The predicate under check.</param>
        /// <param name="paramName">The parameter name.</param>
        /// <exception cref="ArgumentException">If the condition is true and the predicate is false.</exception>
        public static void TrueIf(bool condition, bool predicate, string paramName)
        {
            if (condition && !predicate)
            {
                throw new ArgumentException(FailedMsg.WasFalse(paramName));
            }
        }

        /// <summary>
        /// The check passes if the argument is not null.
        /// </summary>
        /// <param name="argument">The argument under check.</param>
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
        /// The check passes if the argument is not null, empty or whitespace.
        /// </summary>
        /// <param name="argument">The argument under check.</param>
        /// <param name="paramName">The parameter name.</param>
        /// <exception cref="ArgumentException">If the string argument is null, empty or white space.</exception>
        public static void NotEmptyOrWhiteSpace(string argument, string paramName)
        {
            if (string.IsNullOrWhiteSpace(argument))
            {
                throw new ArgumentNullException(paramName, FailedMsg.WasNullEmptyOrWhitespace(paramName));
            }
        }

        /// <summary>
        /// The check passes if the struct argument is not the default value.
        /// </summary>
        /// <typeparam name="T">The argument type.</typeparam>
        /// <param name="argument">The argument under check.</param>
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
        /// The check passes if the argument is not equal to the notToEqual object.
        /// </summary>
        /// <param name="argument">The argument under check.</param>
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
        /// The check passes if the argument is equal to the toEqual object.
        /// </summary>
        /// <param name="argument">The argument under check.</param>
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
        /// The check passes if the <see cref="IReadOnlyCollection{T}"/> is not empty.
        /// </summary>
        /// <typeparam name="T">The collection type.</typeparam>
        /// <param name="collection">The collection under check.</param>
        /// <param name="paramName">The parameter name.</param>
        /// <exception cref="ArgumentException">If the collection is empty.</exception>
        public static void NotEmpty<T>(IReadOnlyCollection<T> collection, string paramName)
        {
            if (collection.Count == 0)
            {
                throw new ArgumentException(FailedMsg.WasEmptyList(paramName));
            }
        }

        /// <summary>
        /// The check passes if the <see cref="IReadOnlyCollection{T}"/> is empty.
        /// </summary>
        /// <typeparam name="T">The collection type.</typeparam>
        /// <param name="collection">The collection under check.</param>
        /// <param name="paramName">The parameter name.</param>
        /// <exception cref="ArgumentException">If the collection is not empty.</exception>
        public static void Empty<T>(IReadOnlyCollection<T> collection, string paramName)
        {
            if (collection.Count != 0)
            {
                throw new ArgumentException(FailedMsg.WasEmptyList(paramName));
            }
        }

        /// <summary>
        /// The check passes if the <see cref="IReadOnlyDictionary{TKey,TValue}"/> is not empty.
        /// </summary>
        /// <typeparam name="TKey">The key type.</typeparam>
        /// <typeparam name="TValue">The value type.</typeparam>
        /// <param name="dictionary">The dictionary under check.</param>
        /// <param name="paramName">The parameter name.</param>
        /// <exception cref="ArgumentException">If the dictionary is empty.</exception>
        public static void NotEmpty<TKey, TValue>(IReadOnlyDictionary<TKey, TValue> dictionary, string paramName)
        {
            if (dictionary.Count == 0)
            {
                throw new ArgumentException(FailedMsg.WasEmptyDictionary(paramName));
            }
        }

        /// <summary>
        /// The check passes if the <see cref="IReadOnlyDictionary{TKey,TValue}"/> is empty.
        /// </summary>
        /// <typeparam name="TKey">The key type.</typeparam>
        /// <typeparam name="TValue">The value type.</typeparam>
        /// <param name="dictionary">The dictionary under check.</param>
        /// <param name="paramName">The parameter name.</param>
        /// <exception cref="ArgumentException">If the dictionary is not empty.</exception>
        public static void Empty<TKey, TValue>(IReadOnlyDictionary<TKey, TValue> dictionary, string paramName)
        {
            if (dictionary.Count != 0)
            {
                throw new ArgumentException(FailedMsg.WasEmptyDictionary(paramName));
            }
        }

        /// <summary>
        /// The check passes if the <see cref="ICollection{T}"/> contains the given element.
        /// </summary>
        /// <typeparam name="T">The collection type.</typeparam>
        /// <param name="element">The element for the collection to contain.</param>
        /// <param name="collection">The collection under check.</param>
        /// <param name="paramName">The element parameter name.</param>
        /// <param name="collectionName">The collection name.</param>
        /// <exception cref="ArgumentException">If the collection does not contain the element.</exception>
        public static void IsIn<T>(T element, ICollection<T> collection, string paramName, string collectionName)
        {
            if (element is null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            if (!collection.Contains(element))
            {
                throw new ArgumentException(FailedMsg.WasNotInCollection(element, paramName, collectionName));
            }
        }

        /// <summary>
        /// The check passes if the <see cref="ICollection{T}"/> contains the given element.
        /// </summary>
        /// <typeparam name="T">The collection type.</typeparam>
        /// <param name="element">The element for the collection not to contain.</param>
        /// <param name="collection">The collection under check.</param>
        /// <param name="paramName">The element parameter name.</param>
        /// <param name="collectionName">The collection name.</param>
        /// <exception cref="ArgumentException">If the collection does not contain the element.</exception>
        public static void NotIn<T>(T element, ICollection<T> collection, string paramName, string collectionName)
        {
            if (element is null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            if (collection.Contains(element))
            {
                throw new ArgumentException(FailedMsg.WasNotInCollection(element, paramName, collectionName));
            }
        }

        /// <summary>
        /// The check passes if the <see cref="IReadOnlyDictionary{TKey,TValue}"/> contains the given key.
        /// </summary>
        /// <typeparam name="TKey">The key type.</typeparam>
        /// <typeparam name="TValue">The value type.</typeparam>
        /// <param name="key">The key for the dictionary to contain.</param>
        /// <param name="dictionary">The dictionary under check.</param>
        /// <param name="paramName">The key parameter name.</param>
        /// <param name="dictName">The dictionary name.</param>
        /// <exception cref="ArgumentException">If the dictionary does not contain the key.</exception>
        public static void KeyIn<TKey, TValue>(TKey key, IReadOnlyDictionary<TKey, TValue> dictionary, string paramName, string dictName)
        {
            if (key is null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (!dictionary.ContainsKey(key))
            {
                throw new ArgumentException(FailedMsg.WasNotInDictionary(key, paramName, dictName));
            }
        }

        /// <summary>
        /// The check passes if the <see cref="IReadOnlyDictionary{TKey,TValue}"/> does not contain the given key.
        /// </summary>
        /// <typeparam name="TKey">The key type.</typeparam>
        /// <typeparam name="TValue">The value type.</typeparam>
        /// <param name="key">The key for the dictionary not to contain.</param>
        /// <param name="dictionary">The dictionary under check.</param>
        /// <param name="paramName">The key parameter name.</param>
        /// <param name="dictName">The dictionary name.</param>
        /// <exception cref="ArgumentException">If the dictionary already contains the key.</exception>
        public static void KeyNotIn<TKey, TValue>(TKey key, IReadOnlyDictionary<TKey, TValue> dictionary, string paramName, string dictName)
        {
            if (key is null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (dictionary.ContainsKey(key))
            {
                throw new ArgumentException(FailedMsg.WasInDictionary(key, paramName, dictName));
            }
        }

        /// <summary>
        /// The check passes if the value is greater than zero.
        /// </summary>
        /// <param name="value">The value under check.</param>
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
        /// The check passes if the value is greater than zero.
        /// </summary>
        /// <param name="value">The value under check.</param>
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
        /// The check passes if the value is greater than zero.
        /// </summary>
        /// <param name="value">The value under check.</param>
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
        /// The check passes if the value is greater than zero.
        /// </summary>
        /// <param name="value">The value under check.</param>
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
        /// The check passes if the value is greater than or equal to zero.
        /// </summary>
        /// <param name="value">The value under check.</param>
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
        /// The check passes if the value is greater than or equal to zero.
        /// </summary>
        /// <param name="value">The value under check.</param>
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
        /// The check passes if the value is greater than or equal to zero.
        /// </summary>
        /// <param name="value">The value under check.</param>
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
        /// The check passes if the value is greater than or equal to zero.
        /// </summary>
        /// <param name="value">The value under check.</param>
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
        /// The check passes if the value is not out of the specified range (inclusive of bounds).
        /// </summary>
        /// <param name="value">The value under check.</param>
        /// <param name="lowerBound">The range lower bound (inclusive).</param>
        /// <param name="upperBound">The range upper bound (inclusive).</param>
        /// <param name="paramName">The parameter name.</param>
        /// <exception cref="ArgumentException">If the value is out of the specified range.</exception>
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
        /// The check passes if the value is not out of the specified range (inclusive of bounds).
        /// </summary>
        /// <param name="value">The value under check.</param>
        /// <param name="lowerBound">The range lower bound (inclusive).</param>
        /// <param name="upperBound">The range upper bound (inclusive).</param>
        /// <param name="paramName">The parameter name.</param>
        /// <exception cref="ArgumentException">If the value is out of the specified range.</exception>
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
        /// The check passes if the value is not out of the specified range (inclusive of bounds).
        /// </summary>
        /// <param name="value">The value under check.</param>
        /// <param name="lowerBound">The range lower bound (inclusive).</param>
        /// <param name="upperBound">The range upper bound (inclusive).</param>
        /// <param name="paramName">The parameter name.</param>
        /// <exception cref="ArgumentException">If the value is out of the specified range.</exception>
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
        /// The check passes if the value is not out of the specified range (inclusive of bounds).
        /// </summary>
        /// <param name="value">The value under check.</param>
        /// <param name="lowerBound">The range lower bound (inclusive).</param>
        /// <param name="upperBound">The range upper bound (inclusive).</param>
        /// <param name="paramName">The parameter name.</param>
        /// <exception cref="ArgumentException">If the value is out of the specified range.</exception>
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
    }
}
