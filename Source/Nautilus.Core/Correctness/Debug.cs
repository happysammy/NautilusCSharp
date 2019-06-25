//--------------------------------------------------------------------------------------------------
// <copyright file="Debug.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Core.Correctness
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    /// <summary>
    /// Provides condition checking methods which are executed in debug configuration.
    /// If the check passes then the method does nothing. If the check fails a type of
    /// <see cref="ArgumentException"/> is thrown with a message.
    /// </summary>
    public static class Debug
    {
        /// <summary>
        /// The condition check passes if the predicate is true.
        /// </summary>
        /// <param name="predicate">The predicate under check.</param>
        /// <param name="description">The predicate description.</param>
        /// <exception cref="ArgumentException">If the predicate is false.</exception>
        [Conditional("DEBUG")]
        public static void True(bool predicate, string description)
        {
            Condition.True(predicate, description);
        }

        /// <summary>
        /// The condition check passes if the argument is not null.
        /// </summary>
        /// <param name="argument">The argument under check.</param>
        /// <param name="paramName">The parameter name.</param>
        /// <typeparam name="T">The arguments type.</typeparam>
        /// <exception cref="ArgumentNullException">If the argument is null.</exception>
        [Conditional("DEBUG")]
        public static void NotNull<T>(T argument, string paramName)
        {
            Condition.NotNull(argument, paramName);
        }

        /// <summary>
        /// The condition check passes if the argument is not null, empty or whitespace.
        /// </summary>
        /// <param name="argument">The argument under check.</param>
        /// <param name="paramName">The parameter name.</param>
        /// <exception cref="ArgumentException">If the string argument is null, empty or white space.</exception>
        [Conditional("DEBUG")]
        public static void NotEmptyOrWhiteSpace(string argument, string paramName)
        {
            Condition.NotEmptyOrWhiteSpace(argument, paramName);
        }

        /// <summary>
        /// The condition check passes if the struct argument is not the default value.
        /// </summary>
        /// <typeparam name="T">The argument type.</typeparam>
        /// <param name="argument">The argument under check.</param>
        /// <param name="paramName">The parameter name.</param>
        /// <exception cref="ArgumentException">If the argument is the default value.</exception>
        [Conditional("DEBUG")]
        public static void NotDefault<T>(T argument, string paramName)
            where T : struct
        {
            Condition.NotDefault(argument, paramName);
        }

        /// <summary>
        /// The condition check passes if the argument is not equal to the notToEqual object.
        /// </summary>
        /// <param name="argument">The argument under check.</param>
        /// <param name="notToEqual">The object not to be equal to.</param>
        /// <param name="paramName">The parameter name.</param>
        /// <exception cref="ArgumentException">If the argument equals notToEqual.</exception>
        [Conditional("DEBUG")]
        public static void NotEqualTo(object argument, object notToEqual, string paramName)
        {
            Condition.NotEqualTo(argument, notToEqual, paramName);
        }

        /// <summary>
        /// The condition check passes if the argument is equal to the toEqual object.
        /// </summary>
        /// <param name="argument">The argument under check.</param>
        /// <param name="toEqual">The object to be equal to.</param>
        /// <param name="paramName">The parameter name.</param>
        /// <exception cref="ArgumentException">If the argument does not equal toEqual.</exception>
        [Conditional("DEBUG")]
        public static void EqualTo(object argument, object toEqual, string paramName)
        {
            Condition.EqualTo(argument, toEqual, paramName);
        }

        /// <summary>
        /// The condition check passes if the <see cref="ICollection{T}"/> is not empty.
        /// </summary>
        /// <typeparam name="T">The collection type.</typeparam>
        /// <param name="collection">The collection under check.</param>
        /// <param name="paramName">The parameter name.</param>
        /// <exception cref="ArgumentNullException">If the collection is null.</exception>
        /// <exception cref="ArgumentException">If the collection is empty.</exception>
        [Conditional("DEBUG")]
        public static void NotEmpty<T>(IReadOnlyCollection<T> collection, string paramName)
        {
            Condition.NotEmpty(collection, paramName);
        }

        /// <summary>
        /// The condition check passes if the <see cref="ICollection{T}"/> is empty.
        /// </summary>
        /// <typeparam name="T">The collection type.</typeparam>
        /// <param name="collection">The collection under check.</param>
        /// <param name="paramName">The parameter name.</param>
        /// <exception cref="ArgumentNullException">If the collection is null.</exception>
        /// <exception cref="ArgumentException">If the collection is not empty.</exception>
        [Conditional("DEBUG")]
        public static void Empty<T>(IReadOnlyCollection<T> collection, string paramName)
        {
            Condition.Empty(collection, paramName);
        }

        /// <summary>
        /// The condition check passes if the <see cref="Dictionary{TKey,TValue}"/> is not empty.
        /// </summary>
        /// <typeparam name="TKey">The key type.</typeparam>
        /// <typeparam name="TValue">The value type.</typeparam>
        /// <param name="dictionary">The dictionary under check.</param>
        /// <param name="paramName">The parameter name.</param>
        /// <exception cref="ArgumentNullException">If the dictionary is null.</exception>
        /// <exception cref="ArgumentException">If the dictionary is empty.</exception>
        [Conditional("DEBUG")]
        public static void NotEmpty<TKey, TValue>(IReadOnlyDictionary<TKey, TValue> dictionary, string paramName)
        {
            Condition.NotEmpty(dictionary, paramName);
        }

        /// <summary>
        /// The condition check passes if the <see cref="Dictionary{TKey,TValue}"/> is empty.
        /// </summary>
        /// <typeparam name="TKey">The key type.</typeparam>
        /// <typeparam name="TValue">The value type.</typeparam>
        /// <param name="dictionary">The dictionary under check.</param>
        /// <param name="paramName">The parameter name.</param>
        /// <exception cref="ArgumentNullException">If the dictionary is null.</exception>
        /// <exception cref="ArgumentException">If the dictionary is not empty.</exception>
        [Conditional("DEBUG")]
        public static void Empty<TKey, TValue>(IReadOnlyDictionary<TKey, TValue> dictionary, string paramName)
        {
            Condition.Empty(dictionary, paramName);
        }

        /// <summary>
        /// The condition check passes if the <see cref="ICollection{T}"/> contains the given element.
        /// </summary>
        /// <typeparam name="T">The collection type.</typeparam>
        /// <param name="element">The element for the collection to contain.</param>
        /// <param name="collection">The collection under check.</param>
        /// <param name="paramName">The element parameter name.</param>
        /// <param name="collectionName">The collection name.</param>
        /// <exception cref="ArgumentNullException">If the element is null.</exception>
        /// <exception cref="ArgumentNullException">If the collection is null.</exception>
        /// <exception cref="ArgumentException">If the collection does not contain the element.</exception>
        [Conditional("DEBUG")]
        public static void IsIn<T>(T element, ICollection<T> collection, string paramName, string collectionName)
        {
            Condition.IsIn(element, collection, paramName, collectionName);
        }

        /// <summary>
        /// The condition check passes if the <see cref="ICollection{T}"/> contains the given element.
        /// </summary>
        /// <typeparam name="T">The collection type.</typeparam>
        /// <param name="element">The element for the collection not to contain.</param>
        /// <param name="collection">The collection under check.</param>
        /// <param name="paramName">The element parameter name.</param>
        /// <param name="collectionName">The collection name.</param>
        /// <exception cref="ArgumentNullException">If the element is null.</exception>
        /// <exception cref="ArgumentNullException">If the collection is null.</exception>
        /// <exception cref="ArgumentException">If the collection does not contain the element.</exception>
        [Conditional("DEBUG")]
        public static void NotIn<T>(T element, ICollection<T> collection, string paramName, string collectionName)
        {
            Condition.NotIn(element, collection, paramName, collectionName);
        }

        /// <summary>
        /// The condition check passes if the <see cref="IReadOnlyDictionary{TKey,TValue}"/> contains the given key.
        /// </summary>
        /// <typeparam name="TKey">The key type.</typeparam>
        /// <typeparam name="TValue">The value type.</typeparam>
        /// <param name="key">The key for the dictionary to contain.</param>
        /// <param name="dictionary">The dictionary under check.</param>
        /// <param name="paramName">The key parameter name.</param>
        /// <param name="dictName">The dictionary name.</param>
        /// <exception cref="ArgumentNullException">If the key is null.</exception>
        /// <exception cref="ArgumentNullException">If the dictionary is null.</exception>
        /// <exception cref="ArgumentException">If the dictionary does not contain the key.</exception>
        [Conditional("DEBUG")]
        public static void KeyIn<TKey, TValue>(TKey key, IReadOnlyDictionary<TKey, TValue> dictionary, string paramName, string dictName)
        {
            Condition.KeyIn(key, dictionary, paramName, dictName);
        }

        /// <summary>
        /// The condition check passes if the <see cref="IReadOnlyDictionary{TKey,TValue}"/> does not contain the given key.
        /// </summary>
        /// <typeparam name="TKey">The key type.</typeparam>
        /// <typeparam name="TValue">The value type.</typeparam>
        /// <param name="key">The key for the dictionary not to contain.</param>
        /// <param name="dictionary">The dictionary under check.</param>
        /// <param name="paramName">The key parameter name.</param>
        /// <param name="dictName">The dictionary name.</param>
        /// <exception cref="ArgumentNullException">If the key is null.</exception>
        /// <exception cref="ArgumentNullException">If the dictionary is null.</exception>
        /// <exception cref="ArgumentException">If the dictionary already contains the key.</exception>
        [Conditional("DEBUG")]
        public static void KeyNotIn<TKey, TValue>(TKey key, IReadOnlyDictionary<TKey, TValue> dictionary, string paramName, string dictName)
        {
            Condition.KeyNotIn(key, dictionary, paramName, dictName);
        }

        /// <summary>
        /// The condition check passes if the value is greater than zero.
        /// </summary>
        /// <param name="value">The value under check.</param>
        /// <param name="paramName">The parameter name.</param>
        /// <exception cref="ArgumentOutOfRangeException">If the value is not positive (> 0).</exception>
        [Conditional("DEBUG")]
        public static void PositiveInt32(int value, string paramName)
        {
            Condition.PositiveInt32(value, paramName);
        }

        /// <summary>
        /// The condition check passes if the value is greater than zero.
        /// </summary>
        /// <param name="value">The value under check.</param>
        /// <param name="paramName">The parameter name.</param>
        /// <exception cref="ArgumentOutOfRangeException">If the value is not positive (> 0).</exception>
        [Conditional("DEBUG")]
        public static void PositiveInt64(long value, string paramName)
        {
            Condition.PositiveInt64(value, paramName);
        }

        /// <summary>
        /// The condition check passes if the value is greater than zero.
        /// </summary>
        /// <param name="value">The value under check.</param>
        /// <param name="paramName">The parameter name.</param>
        /// <exception cref="ArgumentOutOfRangeException">If the value is not positive (> 0).</exception>
        [Conditional("DEBUG")]
        public static void PositiveDouble(double value, string paramName)
        {
            Condition.PositiveDouble(value, paramName);
        }

        /// <summary>
        /// The condition check passes if the value is greater than zero.
        /// </summary>
        /// <param name="value">The value under check.</param>
        /// <param name="paramName">The parameter name.</param>
        /// <exception cref="ArgumentOutOfRangeException">If the value is not positive (> 0).</exception>
        [Conditional("DEBUG")]
        public static void PositiveDecimal(decimal value, string paramName)
        {
            Condition.PositiveDecimal(value, paramName);
        }

        /// <summary>
        /// The condition check passes if the value is greater than or equal to zero.
        /// </summary>
        /// <param name="value">The value under check.</param>
        /// <param name="paramName">The parameter name.</param>
        /// <exception cref="ArgumentOutOfRangeException">If the value less than zero.</exception>
        [Conditional("DEBUG")]
        public static void NotNegativeInt32(int value, string paramName)
        {
            Condition.NotNegativeInt32(value, paramName);
        }

        /// <summary>
        /// The condition check passes if the value is greater than or equal to zero.
        /// </summary>
        /// <param name="value">The value under check.</param>
        /// <param name="paramName">The parameter name.</param>
        /// <exception cref="ArgumentOutOfRangeException">If the value less than zero.</exception>
        [Conditional("DEBUG")]
        public static void NotNegativeInt64(long value, string paramName)
        {
            Condition.NotNegativeInt64(value, paramName);
        }

        /// <summary>
        /// The condition check passes if the value is greater than or equal to zero.
        /// </summary>
        /// <param name="value">The value under check.</param>
        /// <param name="paramName">The parameter name.</param>
        /// <exception cref="ArgumentOutOfRangeException">If the value less than zero.</exception>
        [Conditional("DEBUG")]
        public static void NotNegativeDouble(double value, string paramName)
        {
            Condition.NotNegativeDouble(value, paramName);
        }

        /// <summary>
        /// The condition check passes if the value is greater than or equal to zero.
        /// </summary>
        /// <param name="value">The value under check.</param>
        /// <param name="paramName">The parameter name.</param>
        /// <exception cref="ArgumentOutOfRangeException">If the value less than zero.</exception>
        [Conditional("DEBUG")]
        public static void NotNegativeDecimal(decimal value, string paramName)
        {
            Condition.NotNegativeDecimal(value, paramName);
        }

        /// <summary>
        /// The condition check passes if the value is not out of the specified range (inclusive of bounds).
        /// </summary>
        /// <param name="value">The value under check.</param>
        /// <param name="lowerBound">The range lower bound (inclusive).</param>
        /// <param name="upperBound">The range upper bound (inclusive).</param>
        /// <param name="paramName">The parameter name.</param>
        /// <exception cref="ArgumentOutOfRangeException">If the value is out of the specified range.</exception>
        [Conditional("DEBUG")]
        public static void NotOutOfRangeInt32(
            int value,
            int lowerBound,
            int upperBound,
            string paramName)
        {
            Condition.NotOutOfRangeInt32(value, lowerBound, upperBound, paramName);
        }

        /// <summary>
        /// The condition check passes if the value is not out of the specified range (inclusive of bounds).
        /// </summary>
        /// <param name="value">The value under check.</param>
        /// <param name="lowerBound">The range lower bound (inclusive).</param>
        /// <param name="upperBound">The range upper bound (inclusive).</param>
        /// <param name="paramName">The parameter name.</param>
        /// <exception cref="ArgumentOutOfRangeException">If the value is out of the specified range.</exception>
        [Conditional("DEBUG")]
        public static void NotOutOfRangeInt64(
            long value,
            long lowerBound,
            long upperBound,
            string paramName)
        {
            Condition.NotOutOfRangeInt64(value, lowerBound, upperBound, paramName);
        }

        /// <summary>
        /// The condition check passes if the value is not out of the specified range (inclusive of bounds).
        /// </summary>
        /// <param name="value">The value under check.</param>
        /// <param name="lowerBound">The range lower bound (inclusive).</param>
        /// <param name="upperBound">The range upper bound (inclusive).</param>
        /// <param name="paramName">The parameter name.</param>
        /// <exception cref="ArgumentOutOfRangeException">If the value is out of the specified range.</exception>
        [Conditional("DEBUG")]
        public static void NotOutOfRangeDouble(
            double value,
            double lowerBound,
            double upperBound,
            string paramName)
        {
            Condition.NotOutOfRangeDouble(value, lowerBound, upperBound, paramName);
        }

        /// <summary>
        /// The condition check passes if the value is not out of the specified range (inclusive of bounds).
        /// </summary>
        /// <param name="value">The value under check.</param>
        /// <param name="lowerBound">The range lower bound (inclusive).</param>
        /// <param name="upperBound">The range upper bound (inclusive).</param>
        /// <param name="paramName">The parameter name.</param>
        /// <exception cref="ArgumentOutOfRangeException">If the value is out of the specified range.</exception>
        [Conditional("DEBUG")]
        public static void NotOutOfRangeDecimal(
            decimal value,
            decimal lowerBound,
            decimal upperBound,
            string paramName)
        {
            Condition.NotOutOfRangeDecimal(value, lowerBound, upperBound, paramName);
        }
    }
}
