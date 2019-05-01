//--------------------------------------------------------------------------------------------------
// <copyright file="Debug.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Core.Correctness
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    /// <summary>
    /// Provides various assertion checking methods which are executed in debug configuration.
    /// If the check passes then the method does nothing. If the check fails an AssertionFailed is
    /// raised with a message.
    /// </summary>
    public static class Debug
    {
        /// <summary>
        /// The check passes if the predicate is true.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <param name="description">The predicate description.</param>
        /// <exception cref="ArgumentException">If the predicate is false.</exception>
        [Conditional("DEBUG")]
        public static void True(bool predicate, string description)
        {
            Precondition.True(predicate, description);
        }

        /// <summary>
        /// The check passes if the condition is false, or both the condition and predicate are
        /// true.
        /// </summary>
        /// <param name="condition">The condition.</param>
        /// <param name="predicate">The predicate.</param>
        /// <param name="paramName">The parameter name.</param>
        /// <exception cref="ArgumentException">If the condition is true and the predicate is false.</exception>
        [Conditional("DEBUG")]
        public static void TrueIf(bool condition, bool predicate, string paramName)
        {
            Precondition.TrueIf(condition, predicate, paramName);
        }

        /// <summary>
        /// The check passes if the argument is not null.
        /// </summary>
        /// <param name="argument">The argument.</param>
        /// <param name="paramName">The parameter name.</param>
        /// <typeparam name="T">The arguments type.</typeparam>
        /// <exception cref="ArgumentNullException">If the argument is null.</exception>
        [Conditional("DEBUG")]
        public static void NotNull<T>(T argument, string paramName)
        {
            Precondition.NotNull(argument, paramName);
        }

        /// <summary>
        /// The check passes if the <see cref="string"/> argument is not null.
        /// </summary>
        /// <param name="argument">The string argument.</param>
        /// <param name="paramName">The parameter name.</param>
        /// <exception cref="ArgumentException">If the string argument is null, empty or white space.</exception>
        [Conditional("DEBUG")]
        public static void NotEmptyOrWhiteSpace(string argument, string paramName)
        {
            Precondition.NotEmptyOrWhiteSpace(argument, paramName);
        }

        /// <summary>
        /// The check passes if the struct argument is not the default value.
        /// </summary>
        /// <typeparam name="T">The argument type.</typeparam>
        /// <param name="argument">The argument.</param>
        /// <param name="paramName">The parameter name.</param>
        /// <exception cref="ArgumentException">If the argument is the default value.</exception>
        [Conditional("DEBUG")]
        public static void NotDefault<T>(T argument, string paramName)
            where T : struct
        {
            Precondition.NotDefault(argument, paramName);
        }

        /// <summary>
        /// The check passes if the <see cref="ICollection{T}"/> is not null, or empty.
        /// </summary>
        /// <param name="collection">The collection being checked.</param>
        /// <param name="paramName">The parameter name.</param>
        /// <typeparam name="T">The type of collection.</typeparam>
        /// <exception cref="ArgumentException">If the collection is empty.</exception>
        [Conditional("DEBUG")]
        public static void NotEmpty<T>(ICollection<T> collection, string paramName)
        {
            Precondition.NotEmpty(collection, paramName);
        }

        /// <summary>
        /// The check passes if the <see cref="ICollection{T}"/> is not null, or empty.
        /// </summary>
        /// <param name="dictionary">The list being checked.</param>
        /// <param name="paramName">The parameter name.</param>
        /// <typeparam name="TK">The type of key.</typeparam>
        /// <typeparam name="TV">The type of value.</typeparam>
        /// <exception cref="ArgumentException">If the dictionary is empty.</exception>
        [Conditional("DEBUG")]
        public static void NotEmpty<TK, TV>(Dictionary<TK, TV> dictionary, string paramName)
        {
            Precondition.NotEmpty(dictionary, paramName);
        }

        /// <summary>
        /// The check passes if the <see cref="ICollection{T}"/> contains the given element.
        /// </summary>
        /// <typeparam name="T">The type of collection.</typeparam>
        /// <param name="element">The element to contain.</param>
        /// <param name="collection">The collection being checked.</param>
        /// <param name="paramName">The parameter name.</param>
        /// <param name="collectionName">The collection name.</param>
        /// <exception cref="ArgumentException">If the collection does not contain the element.</exception>
        [Conditional("DEBUG")]
        public static void IsIn<T>(T element, ICollection<T> collection, string paramName, string collectionName)
        {
            Precondition.IsIn(element, collection, paramName, collectionName);
        }

        /// <summary>
        /// The check passes if the <see cref="ICollection{T}"/> contains the given element.
        /// </summary>
        /// <typeparam name="T">The type of collection.</typeparam>
        /// <param name="element">The element to contain.</param>
        /// <param name="collection">The collection being checked.</param>
        /// <param name="paramName">The parameter name.</param>
        /// <param name="collectionName">The collection name.</param>
        /// <exception cref="ArgumentException">If the collection does not contain the element.</exception>
        [Conditional("DEBUG")]
        public static void IsNotIn<T>(T element, ICollection<T> collection, string paramName, string collectionName)
        {
            Precondition.IsNotIn(element, collection, paramName, collectionName);
        }

        /// <summary>
        /// The check passes if the <see cref="IDictionary{TKey,TValue}"/> contains the given
        /// key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="paramName">The parameter name.</param>
        /// <param name="dictName">The dictionary name.</param>
        /// <typeparam name="T1">The type of the keys.</typeparam>
        /// <typeparam name="T2">The type of the values.</typeparam>
        /// <exception cref="ArgumentException">If the dictionary does not contain the key.</exception>
        [Conditional("DEBUG")]
        public static void KeyIn<T1, T2>(T1 key, Dictionary<T1, T2> dictionary, string paramName, string dictName)
        {
            Precondition.KeyIn(key, dictionary, paramName, dictName);
        }

        /// <summary>
        /// The check passes if the <see cref="IDictionary{TKey,TValue}"/> contains the given
        /// key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="paramName">The parameter name.</param>
        /// <param name="dictName">The dictionary name.</param>
        /// <typeparam name="T1">The type of the keys.</typeparam>
        /// <typeparam name="T2">The type of the values.</typeparam>
        //// <exception cref="ArgumentException">If the dictionary does not contain the key.</exception>
        [Conditional("DEBUG")]
        public static void KeyNotIn<T1, T2>(T1 key, Dictionary<T1, T2> dictionary, string paramName, string dictName)
        {
            Precondition.KeyNotIn(key, dictionary, paramName, dictName);
        }

        /// <summary>
        /// The check passes if the object is not equal to the other object.
        /// </summary>
        /// <param name="obj">The input object.</param>
        /// <param name="objNotToEqual">The other object not to equal.</param>
        /// <param name="paramName">The parameter name.</param>
        /// <exception cref="ArgumentException">If the objects are equal.</exception>
        [Conditional("DEBUG")]
        public static void NotEqualTo(object obj, object objNotToEqual, string paramName)
        {
            Precondition.NotEqualTo(obj, objNotToEqual, paramName);
        }

        /// <summary>
        /// The check passes if the object is equal to the other object.
        /// </summary>
        /// <param name="obj">The input object.</param>
        /// <param name="objToEqual">The other object to be equal to.</param>
        /// <param name="paramName">The parameter name.</param>
        /// <exception cref="ArgumentException">If the objects are not equal.</exception>
        [Conditional("DEBUG")]
        public static void EqualTo(object obj, object objToEqual, string paramName)
        {
            Precondition.NotEqualTo(obj, objToEqual, paramName);
        }

        /// <summary>
        /// The check passes if the value is greater than zero.
        /// </summary>
        /// <param name="value">The value to be checked.</param>
        /// <param name="paramName">The parameter name.</param>
        /// <exception cref="ArgumentOutOfRangeException">If the value is not positive (> 0).</exception>
        [Conditional("DEBUG")]
        public static void PositiveInt32(int value, string paramName)
        {
            Precondition.PositiveInt32(value, paramName);
        }

        /// <summary>
        /// The check passes if the value is greater than zero.
        /// </summary>
        /// <param name="value">The value to be checked.</param>
        /// <param name="paramName">The parameter name.</param>
        /// <exception cref="ArgumentOutOfRangeException">If the value is not positive (> 0).</exception>
        [Conditional("DEBUG")]
        public static void PositiveInt64(long value, string paramName)
        {
            Precondition.PositiveInt64(value, paramName);
        }

        /// <summary>
        /// The check passes if the value is greater than zero.
        /// </summary>
        /// <param name="value">The value to be checked.</param>
        /// <param name="paramName">The parameter name.</param>
        /// <exception cref="ArgumentOutOfRangeException">If the value is not positive (> 0).</exception>
        [Conditional("DEBUG")]
        public static void PositiveDouble(double value, string paramName)
        {
            Precondition.PositiveDouble(value, paramName);
        }

        /// <summary>
        /// The check passes if the value is greater than zero.
        /// </summary>
        /// <param name="value">The value to be checked.</param>
        /// <param name="paramName">The parameter name.</param>
        /// <exception cref="ArgumentOutOfRangeException">If the value is not positive (> 0).</exception>
        [Conditional("DEBUG")]
        public static void PositiveDecimal(decimal value, string paramName)
        {
            Precondition.PositiveDecimal(value, paramName);
        }

        /// <summary>
        /// The check passes if the value is greater than or equal to zero.
        /// </summary>
        /// <param name="value">The value to be checked.</param>
        /// <param name="paramName">The parameter name.</param>
        /// <exception cref="ArgumentOutOfRangeException">If the value less than zero.</exception>
        [Conditional("DEBUG")]
        public static void NotNegativeInt32(int value, string paramName)
        {
            Precondition.NotNegativeInt32(value, paramName);
        }

        /// <summary>
        /// The check passes if the value is greater than or equal to zero.
        /// </summary>
        /// <param name="value">The value to be checked.</param>
        /// <param name="paramName">The parameter name.</param>
        /// <exception cref="ArgumentOutOfRangeException">If the value less than zero.</exception>
        [Conditional("DEBUG")]
        public static void NotNegativeInt64(long value, string paramName)
        {
            Precondition.NotNegativeInt64(value, paramName);
        }

        /// <summary>
        /// The check passes if the value is greater than or equal to zero.
        /// </summary>
        /// <param name="value">The value to be checked.</param>
        /// <param name="paramName">The parameter name.</param>
        /// <exception cref="ArgumentOutOfRangeException">If the value less than zero.</exception>
        [Conditional("DEBUG")]
        public static void NotNegativeDouble(double value, string paramName)
        {
            Precondition.NotNegativeDouble(value, paramName);
        }

        /// <summary>
        /// The check passes if the value is greater than or equal to zero.
        /// </summary>
        /// <param name="value">The value to be checked.</param>
        /// <param name="paramName">The parameter name.</param>
        /// <exception cref="ArgumentOutOfRangeException">If the value less than zero.</exception>
        [Conditional("DEBUG")]
        public static void NotNegativeDecimal(decimal value, string paramName)
        {
            Precondition.NotNegativeDecimal(value, paramName);
        }

        /// <summary>
        /// The check passes if the value is not out of the specified range inclusive.
        /// </summary>
        /// <param name="value">The value to be checked.</param>
        /// <param name="lowerBound">The range lower bound.</param>
        /// <param name="upperBound">The range upper bound.</param>
        /// <param name="paramName">The parameter name.</param>
        /// <exception cref="ArgumentException">If the value is out of the specified range inclusive.</exception>
        [Conditional("DEBUG")]
        public static void NotOutOfRangeInt32(
            int value,
            int lowerBound,
            int upperBound,
            string paramName)
        {
            Precondition.NotOutOfRangeInt32(value, lowerBound, upperBound, paramName);
        }

        /// <summary>
        /// The check passes if the value is not out of the specified range inclusive.
        /// </summary>
        /// <param name="value">The value to be checked.</param>
        /// <param name="lowerBound">The range lower bound.</param>
        /// <param name="upperBound">The range upper bound.</param>
        /// <param name="paramName">The parameter name.</param>
        /// <exception cref="ArgumentException">If the value is out of the specified range inclusive.</exception>
        [Conditional("DEBUG")]
        public static void NotOutOfRangeInt64(
            long value,
            long lowerBound,
            long upperBound,
            string paramName)
        {
            Precondition.NotOutOfRangeInt64(value, lowerBound, upperBound, paramName);
        }

        /// <summary>
        /// The check passes if the value is not out of the specified range inclusive.
        /// </summary>
        /// <param name="value">The value to be checked.</param>
        /// <param name="lowerBound">The range lower bound.</param>
        /// <param name="upperBound">The range upper bound.</param>
        /// <param name="paramName">The parameter name.</param>
        /// <exception cref="ArgumentException">If the value is out of the specified range inclusive.</exception>
        [Conditional("DEBUG")]
        public static void NotOutOfRangeDouble(
            double value,
            double lowerBound,
            double upperBound,
            string paramName)
        {
            Precondition.NotOutOfRangeDouble(value, lowerBound, upperBound, paramName);
        }

        /// <summary>
        /// The check passes if the value is not out of the specified range inclusive.
        /// </summary>
        /// <param name="value">The value to be checked.</param>
        /// <param name="lowerBound">The range lower bound.</param>
        /// <param name="upperBound">The range upper bound.</param>
        /// <param name="paramName">The parameter name.</param>
        /// <exception cref="ArgumentException">If the value is out of the specified range inclusive.</exception>
        [Conditional("DEBUG")]
        public static void NotOutOfRangeDecimal(
            decimal value,
            decimal lowerBound,
            decimal upperBound,
            string paramName)
        {
            Precondition.NotOutOfRangeDecimal(value, lowerBound, upperBound, paramName);
        }
    }
}
