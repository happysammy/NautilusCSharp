//--------------------------------------------------------------------------------------------------
// <copyright file="Debug.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Core.Validation
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    /// <summary>
    /// Provides validation methods which are executed in debug configuration only.
    /// If validation passes the method does nothing. If the validation fails a
    /// <see cref="ValidationException"/> is throw which will contain the inner
    /// <see cref="ArgumentException"/> with details including a message and parameter name.
    /// </summary>
    public static class Debug
    {
        /// <summary>
        /// The validation passes if the predicate is true.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <param name="paramName">The parameter name.</param>
        /// <exception cref="ValidationException">Throws if the predicate is false.</exception>
        [Conditional("DEBUG")]
        [DebuggerStepThrough]
        public static void True(bool predicate, string paramName)
        {
            Validate.True(predicate, paramName);
        }

        /// <summary>
        /// The validation passes if the condition is false, or both the condition and predicate are
        /// true.
        /// </summary>
        /// <param name="condition">The condition.</param>
        /// <param name="predicate">The predicate.</param>
        /// <param name="paramName">The parameter name.</param>
        /// <exception cref="ValidationException">Throws if the condition is true and the predicate is false.</exception>
        [Conditional("DEBUG")]
        [DebuggerStepThrough]
        public static void TrueIf(bool condition, bool predicate, string paramName)
        {
            Validate.TrueIf(condition, predicate, paramName);
        }

        /// <summary>
        /// The validation passes if the argument is not null.
        /// </summary>
        /// <param name="argument">The argument.</param>
        /// <param name="paramName">The parameter name.</param>
        /// <typeparam name="T">The arguments type.</typeparam>
        /// <exception cref="ValidationException">Throws if the argument is null.</exception>
        [Conditional("DEBUG")]
        [DebuggerStepThrough]
        public static void NotNull<T>(T argument, string paramName)
        {
            Validate.NotNull(argument, paramName);
        }

        /// <summary>
        /// The validation passes if the <see cref="string"/> argument is not null.
        /// </summary>
        /// <param name="argument">The string argument.</param>
        /// <param name="paramName">The parameter name.</param>
        /// <exception cref="ValidationException">Throws if the argument is null.</exception>
        [Conditional("DEBUG")]
        [DebuggerStepThrough]
        public static void NotNull(string argument, string paramName)
        {
            Validate.NotNull(argument, paramName);
        }

        /// <summary>
        /// The validation passes if the struct argument is not the default value.
        /// </summary>
        /// <typeparam name="T">The argument type.</typeparam>
        /// <param name="argument">The string argument.</param>
        /// <param name="paramName">The parameter name.</param>
        /// <exception cref="ValidationException">Throws if the argument is the default value.</exception>
        [Conditional("DEBUG")]
        [DebuggerStepThrough]
        public static void NotDefault<T>(T argument, string paramName)
            where T : struct
        {
            Validate.NotDefault(argument, paramName);
        }

        /// <summary>
        /// The validation passes if the <see cref="ICollection{T}"/> is not null, or empty.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <param name="paramName">The parameter name.</param>
        /// <typeparam name="T">The type of collection.</typeparam>
        /// <exception cref="ValidationException">Throws if the collection is null or empty.</exception>
        [Conditional("DEBUG")]
        [DebuggerStepThrough]
        public static void NotNullOrEmpty<T>(IReadOnlyCollection<T> collection, string paramName)
        {
            Validate.NotNullOrEmpty(collection, paramName);
        }

        /// <summary>
        /// The validation passes if the <see cref="ICollection{T}"/> is not null, and is empty
        /// (count zero).
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <param name="paramName">The parameter name.</param>
        /// <typeparam name="T">The type of collection.</typeparam>
        /// <exception cref="ValidationException">Throws if the collection is not empty.</exception>
        [Conditional("DEBUG")]
        [DebuggerStepThrough]
        public static void Empty<T>(IReadOnlyCollection<T> collection, string paramName)
        {
            Validate.Empty(collection, paramName);
        }

        /// <summary>
        /// The validation passes if the <see cref="ICollection{T}"/> contains the given element.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="paramName">The parameter name.</param>
        /// <param name="collection">The collection.</param>
        /// <typeparam name="T">The type of collection.</typeparam>
        /// <exception cref="ValidationException">Throws if the collection does not contain the element.</exception>
        [Conditional("DEBUG")]
        [DebuggerStepThrough]
        public static void Contains<T>(
            T element,
            string paramName,
            IReadOnlyCollection<T> collection)
        {
            Validate.Contains(element, paramName, collection);
        }

        /// <summary>
        /// The validation passes if the <see cref="ICollection{T}"/> does not contain the given
        /// element.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="paramName">The parameter name.</param>
        /// <param name="collection">The collection.</param>
        /// <typeparam name="T">The type of collection.</typeparam>
        /// <exception cref="ValidationException">Throws if the collection contains the element.</exception>
        [Conditional("DEBUG")]
        [DebuggerStepThrough]
        public static void DoesNotContain<T>(
            T element,
            string paramName,
            IReadOnlyCollection<T> collection)
        {
            Validate.DoesNotContain(element, paramName, collection);
        }

        /// <summary>
        /// The validation passes if the <see cref="IDictionary{TKey,TValue}"/> contains the given
        /// key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="paramName">The parameter name.</param>
        /// <param name="dictionary">The dictionary.</param>
        /// <typeparam name="T1">The type of the keys.</typeparam>
        /// <typeparam name="T2">The type of the values.</typeparam>
        /// <exception cref="ValidationException">Throws if the dictionary does not contain the key.</exception>
        [Conditional("DEBUG")]
        [DebuggerStepThrough]
        public static void ContainsKey<T1, T2>(
            T1 key,
            string paramName,
            IReadOnlyDictionary<T1, T2> dictionary)
        {
            Validate.ContainsKey(key, paramName, dictionary);
        }

        /// <summary>
        /// The validation passes if the <see cref="IDictionary{TKey,TValue}"/> does not contain the
        /// given key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="paramName">The parameter name.</param>
        /// <param name="dictionary">The dictionary.</param>
        /// <typeparam name="T1">The type of the keys.</typeparam>
        /// <typeparam name="T2">The type of the values.</typeparam>
        /// <exception cref="ValidationException">Throws if the dictionary contains the key.</exception>
        [Conditional("DEBUG")]
        [DebuggerStepThrough]
        public static void DoesNotContainKey<T1, T2>(
            T1 key,
            string paramName,
            IReadOnlyDictionary<T1, T2> dictionary)
        {
            Validate.DoesNotContainKey(key, paramName, dictionary);
        }

        /// <summary>
        /// The validation passes if the object is not equal to the other object.
        /// </summary>
        /// <param name="obj">The input object.</param>
        /// <param name="paramName">The parameter name.</param>
        /// <param name="objNotToEqual">The other object not to equal.</param>
        /// <exception cref="ValidationException">Throws if the objects are equal.</exception>
        [Conditional("DEBUG")]
        [DebuggerStepThrough]
        public static void NotEqualTo(object obj, string paramName, object objNotToEqual)
        {
            Validate.NotEqualTo(obj, paramName, objNotToEqual);
        }

        /// <summary>
        /// The condition passes if the object is equal to the other object.
        /// </summary>
        /// <param name="obj">The input object.</param>
        /// <param name="paramName">The parameter name.</param>
        /// <param name="objToEqual">The other object to be equal to.</param>
        /// <exception cref="ValidationException">Throws if the objects are not equal.</exception>
        [Conditional("DEBUG")]
        [DebuggerStepThrough]
        public static void EqualTo(object obj, string paramName, object objToEqual)
        {
            Validate.EqualTo(obj, paramName, objToEqual);
        }

        /// <summary>
        /// The validation passes if the value is greater than zero.
        /// </summary>
        /// <param name="value">The value to be checked.</param>
        /// <param name="paramName">The parameter name.</param>
        /// <exception cref="ValidationException">Throws if the argument is less than or equal to zero.</exception>
        [Conditional("DEBUG")]
        [DebuggerStepThrough]
        public static void PositiveInt32(
            int value,
            string paramName)
        {
            Validate.PositiveInt32(value, paramName);
        }

        /// <summary>
        /// The validation passes if the value is greater than or equal to zero.
        /// </summary>
        /// <param name="value">The value to be checked.</param>
        /// <param name="paramName">The parameter name.</param>
        /// <exception cref="ValidationException">Throws if the argument is less than zero.</exception>
        [Conditional("DEBUG")]
        [DebuggerStepThrough]
        public static void NotNegativeInt32(
            int value,
            string paramName)
        {
            Validate.NotNegativeInt32(value, paramName);
        }

        /// <summary>
        /// The validation passes if the value is greater than zero.
        /// </summary>
        /// <param name="value">The value to be checked.</param>
        /// <param name="paramName">The parameter name.</param>
        /// <exception cref="ValidationException">Throws if the argument is less than or equal to zero.</exception>
        [Conditional("DEBUG")]
        [DebuggerStepThrough]
        public static void PositiveInt64(
            long value,
            string paramName)
        {
            Validate.PositiveInt64(value, paramName);
        }

        /// <summary>
        /// The validation passes if the value is greater than or equal to zero.
        /// </summary>
        /// <param name="value">The value to be checked.</param>
        /// <param name="paramName">The parameter name.</param>
        /// <exception cref="ValidationException">Throws if the argument is less than zero.</exception>
        [Conditional("DEBUG")]
        [DebuggerStepThrough]
        public static void NotNegativeInt64(
            long value,
            string paramName)
        {
            Validate.NotNegativeInt64(value, paramName);
        }

        /// <summary>
        /// The validation passes if the value is greater than zero.
        /// </summary>
        /// <param name="value">The value to be checked.</param>
        /// <param name="paramName">The parameter name.</param>
        /// <exception cref="ValidationException">Throws if the argument is less than or equal to zero.</exception>
        [Conditional("DEBUG")]
        [DebuggerStepThrough]
        public static void PositiveDouble(
            double value,
            string paramName)
        {
            Validate.PositiveDouble(value, paramName);
        }

        /// <summary>
        /// The validation passes if the value is greater than or equal to zero.
        /// </summary>
        /// <param name="value">The value to be checked.</param>
        /// <param name="paramName">The parameter name.</param>
        /// <exception cref="ValidationException">Throws if the argument is less than zero.</exception>
        [Conditional("DEBUG")]
        [DebuggerStepThrough]
        public static void NotNegativeDouble(
            double value,
            string paramName)
        {
            Validate.NotNegativeDouble(value, paramName);
        }

        /// <summary>
        /// The validation passes if the value is greater than zero.
        /// </summary>
        /// <param name="value">The value to be checked.</param>
        /// <param name="paramName">The parameter name.</param>
        /// <exception cref="ValidationException">Throws if the argument is less than or equal to zero.</exception>
        [Conditional("DEBUG")]
        [DebuggerStepThrough]
        public static void PositiveDecimal(
            decimal value,
            string paramName)
        {
            Validate.PositiveDecimal(value, paramName);
        }

        /// <summary>
        /// The validation passes if the value is greater than or equal to zero.
        /// </summary>
        /// <param name="value">The value to be checked.</param>
        /// <param name="paramName">The parameter name.</param>
        /// <exception cref="ValidationException">Throws if the argument is less than zero.</exception>
        [Conditional("DEBUG")]
        [DebuggerStepThrough]
        public static void NotNegativeDecimal(
            decimal value,
            string paramName)
        {
            Validate.NotNegativeDecimal(value, paramName);
        }

        /// <summary>
        /// The validation passes if the value is not out of the specified range.
        /// </summary>
        /// <param name="value">The value to be checked.</param>
        /// <param name="paramName">The parameter name.</param>
        /// <param name="lowerBound">The range lower bound.</param>
        /// <param name="upperBound">The range upper bound.</param>
        /// <param name="endPoints">The range end points literal.</param>
        /// <exception cref="ValidationException">Throws if the value is out of the specified range.</exception>
        [Conditional("DEBUG")]
        [DebuggerStepThrough]
        public static void NotOutOfRangeInt32(
            int value,
            string paramName,
            int lowerBound,
            int upperBound,
            RangeEndPoints endPoints = RangeEndPoints.Inclusive)
        {
            Validate.NotOutOfRangeInt32(value, paramName, lowerBound, upperBound, endPoints);
        }

        /// <summary>
        /// The validation passes if the value is not out of the specified range.
        /// </summary>
        /// <param name="value">The value to be checked.</param>
        /// <param name="paramName">The parameter name.</param>
        /// <param name="lowerBound">The range lower bound.</param>
        /// <param name="upperBound">The range upper bound.</param>
        /// <param name="endPoints">The range end points literal.</param>
        /// <exception cref="ValidationException">Throws if the value is out of the specified range.</exception>
        [Conditional("DEBUG")]
        [DebuggerStepThrough]
        public static void NotOutOfRangeInt64(
            long value,
            string paramName,
            long lowerBound,
            long upperBound,
            RangeEndPoints endPoints = RangeEndPoints.Inclusive)
        {
            Validate.NotOutOfRangeInt64(value, paramName, lowerBound, upperBound, endPoints);
        }

        /// <summary>
        /// The validation passes if the value is not out of the specified range.
        /// </summary>
        /// <param name="value">The value to be checked.</param>
        /// <param name="paramName">The parameter name.</param>
        /// <param name="lowerBound">The range lower bound.</param>
        /// <param name="upperBound">The range upper bound.</param>
        /// <param name="endPoints">The range end points literal.</param>
        /// <exception cref="ValidationException">Throws if the value is out of the specified range.</exception>
        [Conditional("DEBUG")]
        [DebuggerStepThrough]
        public static void NotOutOfRangeDouble(
            double value,
            string paramName,
            double lowerBound,
            double upperBound,
            RangeEndPoints endPoints = RangeEndPoints.Inclusive)
        {
            Validate.NotOutOfRangeDouble(value, paramName, lowerBound, upperBound, endPoints);
        }

        /// <summary>
        /// The validation passes if the value is not out of the specified range.
        /// </summary>
        /// <param name="value">The value to be checked.</param>
        /// <param name="paramName">The parameter name.</param>
        /// <param name="lowerBound">The range lower bound.</param>
        /// <param name="upperBound">The range upper bound.</param>
        /// <param name="endPoints">The range end points literal.</param>
        /// <exception cref="ValidationException">Throws if the value is out of the specified range.</exception>
        [Conditional("DEBUG")]
        [DebuggerStepThrough]
        public static void NotOutOfRangeDecimal(
            decimal value,
            string paramName,
            decimal lowerBound,
            decimal upperBound,
            RangeEndPoints endPoints = RangeEndPoints.Inclusive)
        {
            Validate.NotOutOfRangeDecimal(value, paramName, lowerBound, upperBound, endPoints);
        }
    }
}
