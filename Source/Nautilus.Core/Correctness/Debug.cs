//--------------------------------------------------------------------------------------------------
// <copyright file="Debug.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
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
        /// Check the condition predicate is true.
        /// </summary>
        /// <param name="condition">The condition predicate to check.</param>
        /// <param name="description">The description of the condition predicate.</param>
        /// <exception cref="ArgumentException">If the condition is false.</exception>
        [Conditional("DEBUG")]
        public static void True(bool condition, string description)
        {
            Condition.True(condition, description);
        }

        /// <summary>
        /// Check the condition predicate is false.
        /// </summary>
        /// <param name="condition">The condition predicate to check.</param>
        /// <param name="description">The condition description.</param>
        /// <exception cref="ArgumentException">If the condition is true.</exception>
        [Conditional("DEBUG")]
        public static void False(bool condition, string description)
        {
            Condition.False(condition, description);
        }

        /// <summary>
        /// Check the argument is not null.
        /// </summary>
        /// <param name="argument">The argument to check.</param>
        /// <param name="paramName">The parameter name.</param>
        /// <typeparam name="T">The arguments type.</typeparam>
        /// <exception cref="ArgumentNullException">If the argument is null.</exception>
        [Conditional("DEBUG")]
        public static void NotNull<T>(T argument, string paramName)
        {
            Condition.NotNull(argument, paramName);
        }

        /// <summary>
        /// Check the argument is not null, empty or whitespace.
        /// </summary>
        /// <param name="argument">The argument to check.</param>
        /// <param name="paramName">The parameter name.</param>
        /// <exception cref="ArgumentNullException">If the string argument is null, empty or white space.</exception>
        [Conditional("DEBUG")]
        public static void NotEmptyOrWhiteSpace(string argument, string paramName)
        {
            Condition.NotEmptyOrWhiteSpace(argument, paramName);
        }

        /// <summary>
        /// Check the argument is not the default value.
        /// </summary>
        /// <typeparam name="T">The argument type.</typeparam>
        /// <param name="argument">The argument to check.</param>
        /// <param name="paramName">The parameter name.</param>
        /// <exception cref="ArgumentException">If the argument is the default value.</exception>
        [Conditional("DEBUG")]
        public static void NotDefault<T>(T argument, string paramName)
            where T : struct
        {
            Condition.NotDefault(argument, paramName);
        }

        /// <summary>
        /// Check the argument is not equal to the notToEqual object.
        /// </summary>
        /// <param name="argument">The argument to check.</param>
        /// <param name="notToEqual">The object not to be equal to.</param>
        /// <param name="paramName">The parameter name.</param>
        /// <exception cref="ArgumentException">If the argument equals notToEqual.</exception>
        [Conditional("DEBUG")]
        public static void NotEqualTo(object argument, object notToEqual, string paramName)
        {
            Condition.NotEqualTo(argument, notToEqual, paramName);
        }

        /// <summary>
        /// Check the argument is equal to the toEqual object.
        /// </summary>
        /// <param name="argument">The argument to check.</param>
        /// <param name="toEqual">The object to be equal to.</param>
        /// <param name="paramName">The parameter name.</param>
        /// <exception cref="ArgumentException">If the argument does not equal toEqual.</exception>
        [Conditional("DEBUG")]
        public static void EqualTo(object argument, object toEqual, string paramName)
        {
            Condition.EqualTo(argument, toEqual, paramName);
        }

        /// <summary>
        /// Check the collection is not empty.
        /// </summary>
        /// <typeparam name="T">The collection type.</typeparam>
        /// <param name="collection">The collection to check.</param>
        /// <param name="paramName">The parameter name.</param>
        /// <exception cref="ArgumentNullException">If the collection is null.</exception>
        /// <exception cref="ArgumentException">If the collection is empty.</exception>
        [Conditional("DEBUG")]
        public static void NotEmpty<T>(IReadOnlyCollection<T> collection, string paramName)
        {
            Condition.NotEmpty(collection, paramName);
        }

        /// <summary>
        /// Check the collection is empty.
        /// </summary>
        /// <typeparam name="T">The collection type.</typeparam>
        /// <param name="collection">The collection to check.</param>
        /// <param name="paramName">The parameter name.</param>
        /// <exception cref="ArgumentNullException">If the collection is null.</exception>
        /// <exception cref="ArgumentException">If the collection is not empty.</exception>
        [Conditional("DEBUG")]
        public static void Empty<T>(IReadOnlyCollection<T> collection, string paramName)
        {
            Condition.Empty(collection, paramName);
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
        [Conditional("DEBUG")]
        public static void NotEmpty<TKey, TValue>(Dictionary<TKey, TValue> dictionary, string paramName)
            where TKey : class
        {
            Condition.NotEmpty(dictionary, paramName);
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
        [Conditional("DEBUG")]
        public static void Empty<TKey, TValue>(Dictionary<TKey, TValue> dictionary, string paramName)
            where TKey : class
        {
            Condition.Empty(dictionary, paramName);
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
        [Conditional("DEBUG")]
        public static void IsIn<T>(T element, ICollection<T> collection, string paramName, string collectionName)
        {
            Condition.IsIn(element, collection, paramName, collectionName);
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
        [Conditional("DEBUG")]
        public static void NotIn<T>(T element, ICollection<T> collection, string paramName, string collectionName)
        {
            Condition.NotIn(element, collection, paramName, collectionName);
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
        [Conditional("DEBUG")]
        public static void KeyIn<TKey, TValue>(TKey key, Dictionary<TKey, TValue> dictionary, string paramName, string dictName)
            where TKey : class
        {
            Condition.KeyIn(key, dictionary, paramName, dictName);
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
        [Conditional("DEBUG")]
        public static void KeyNotIn<TKey, TValue>(TKey key, Dictionary<TKey, TValue> dictionary, string paramName, string dictName)
            where TKey : class
        {
            Condition.KeyNotIn(key, dictionary, paramName, dictName);
        }

        /// <summary>
        /// Check the value is positive (> 0).
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <param name="paramName">The parameter name.</param>
        /// <exception cref="ArgumentOutOfRangeException">If the value is not positive (> 0).</exception>
        [Conditional("DEBUG")]
        public static void PositiveInt32(int value, string paramName)
        {
            Condition.PositiveInt32(value, paramName);
        }

        /// <summary>
        /// Check the value is positive (> 0).
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <param name="paramName">The parameter name.</param>
        /// <exception cref="ArgumentOutOfRangeException">If the value is not positive (> 0).</exception>
        [Conditional("DEBUG")]
        public static void PositiveInt64(long value, string paramName)
        {
            Condition.PositiveInt64(value, paramName);
        }

        /// <summary>
        /// Check the value is positive (> 0).
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <param name="paramName">The parameter name.</param>
        /// <exception cref="ArgumentOutOfRangeException">If the value is not positive (> 0).</exception>
        [Conditional("DEBUG")]
        public static void PositiveDouble(double value, string paramName)
        {
            Condition.PositiveDouble(value, paramName);
        }

        /// <summary>
        /// Check the value is positive (> 0).
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <param name="paramName">The parameter name.</param>
        /// <exception cref="ArgumentOutOfRangeException">If the value is not positive (> 0).</exception>
        [Conditional("DEBUG")]
        public static void PositiveDecimal(decimal value, string paramName)
        {
            Condition.PositiveDecimal(value, paramName);
        }

        /// <summary>
        /// Check the value is greater than or equal to zero.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <param name="paramName">The parameter name.</param>
        /// <exception cref="ArgumentOutOfRangeException">If the value is negative.</exception>
        [Conditional("DEBUG")]
        public static void NotNegativeInt32(int value, string paramName)
        {
            Condition.NotNegativeInt32(value, paramName);
        }

        /// <summary>
        /// Check the value is greater than or equal to zero.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <param name="paramName">The parameter name.</param>
        /// <exception cref="ArgumentOutOfRangeException">If the value is negative.</exception>
        [Conditional("DEBUG")]
        public static void NotNegativeInt64(long value, string paramName)
        {
            Condition.NotNegativeInt64(value, paramName);
        }

        /// <summary>
        /// Check the value is greater than or equal to zero.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <param name="paramName">The parameter name.</param>
        /// <exception cref="ArgumentOutOfRangeException">If the value is negative.</exception>
        [Conditional("DEBUG")]
        public static void NotNegativeDouble(double value, string paramName)
        {
            Condition.NotNegativeDouble(value, paramName);
        }

        /// <summary>
        /// Check the value is greater than or equal to zero.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <param name="paramName">The parameter name.</param>
        /// <exception cref="ArgumentOutOfRangeException">If the value is negative.</exception>
        [Conditional("DEBUG")]
        public static void NotNegativeDecimal(decimal value, string paramName)
        {
            Condition.NotNegativeDecimal(value, paramName);
        }

        /// <summary>
        /// Check the value is within specified range (inclusive of bounds).
        /// </summary>
        /// <param name="value">The value to check.</param>
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
        /// Check the value is within specified range (inclusive of bounds).
        /// </summary>
        /// <param name="value">The value to check.</param>
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
        /// Check the value is within specified range (inclusive of bounds).
        /// </summary>
        /// <param name="value">The value to check.</param>
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
        /// Check the value is within specified range (inclusive of bounds).
        /// </summary>
        /// <param name="value">The value to check.</param>
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
