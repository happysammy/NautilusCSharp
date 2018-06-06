//--------------------------------------------------------------------------------------------------
// <copyright file="Debug.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Core.Validation
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Nautilus.Core.Annotations;

    /// <summary>
    /// Provides validation methods which are executed in debug configuration only.
    /// If validation passes the method does nothing. If the validation fails a
    /// <see cref="ValidationException"/> is throw which will contain the inner
    /// <see cref="ArgumentException"/> with details including a message and parameter name.
    /// </summary>
    [Immutable]
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
        /// <exception cref="ValidationException">Throws if the condition does not pass the predicate.</exception>
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
        public static void CollectionNotNullOrEmpty<T>(ICollection<T> collection, string paramName)
        {
            Validate.CollectionNotNullOrEmpty(collection, paramName);
        }

        /// <summary>
        /// The validation passes if the <see cref="IReadOnlyCollection{T}"/> is not null, or empty.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <param name="paramName">The parameter name.</param>
        /// <typeparam name="T">The type of collection.</typeparam>
        /// <exception cref="ValidationException">Throws if the collection is null or empty.</exception>
        [Conditional("DEBUG")]
        [DebuggerStepThrough]
        public static void ReadOnlyCollectionNotNullOrEmpty<T>(IReadOnlyCollection<T> collection, string paramName)
        {
            Validate.ReadOnlyCollectionNotNullOrEmpty(collection, paramName);
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
        public static void CollectionEmpty<T>(ICollection<T> collection, string paramName)
        {
            Validate.CollectionEmpty(collection, paramName);
        }

        /// <summary>
        /// The validation passes if the <see cref="IReadOnlyCollection{T}"/> is not null, and is
        /// empty (count zero).
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <param name="paramName">The parameter name.</param>
        /// <typeparam name="T">The type of collection.</typeparam>
        /// <exception cref="ValidationException">Throws if the collection is not empty.</exception>
        [Conditional("DEBUG")]
        [DebuggerStepThrough]
        public static void ReadOnlyCollectionEmpty<T>(IReadOnlyCollection<T> collection, string paramName)
        {
            Validate.ReadOnlyCollectionEmpty(collection, paramName);
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
        public static void CollectionContains<T>(T element, string paramName, ICollection<T> collection)
        {
            Validate.CollectionContains(element, paramName, collection);
        }

        /// <summary>
        /// The validation passes if the <see cref="IReadOnlyCollection{T}"/> contains the given
        /// element.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="paramName">The parameter name.</param>
        /// <param name="collection">The collection.</param>
        /// <typeparam name="T">The type of collection.</typeparam>
        /// <exception cref="ValidationException">Throws if collection does not contain the element.</exception>
        [Conditional("DEBUG")]
        [DebuggerStepThrough]
        public static void ReadOnlyCollectionContains<T>(T element, string paramName, IReadOnlyCollection<T> collection)
        {
            Validate.ReadOnlyCollectionContains(element, paramName, collection);
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
        public static void CollectionDoesNotContain<T>(T element, string paramName, ICollection<T> collection)
        {
            Validate.CollectionDoesNotContain(element, paramName, collection);
        }

        /// <summary>
        /// The validation passes if the <see cref="IReadOnlyCollection{T}"/> does not contain the
        /// given element.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="paramName">The parameter name.</param>
        /// <param name="collection">The collection.</param>
        /// <typeparam name="T">The type of collection.</typeparam>
        /// <exception cref="ValidationException">Throws if the collection contains the element.</exception>
        [Conditional("DEBUG")]
        [DebuggerStepThrough]
        public static void ReadOnlyCollectionDoesNotContain<T>(T element, string paramName, IReadOnlyCollection<T> collection)
        {
            Validate.ReadOnlyCollectionDoesNotContain(element, paramName, collection);
        }

        /// <summary>
        /// The validation passes if the <see cref="IDictionary{TKey,TValue}"/> contains the given
        /// key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="paramName">The parameter name.</param>
        /// <param name="dictionary">The dictionary.</param>
        /// <typeparam name="T1">The type of the keys.</typeparam>
        /// <typeparam name="T2">The type of the values</typeparam>
        /// <exception cref="ValidationException">Throws if the dictionary does not contain the key.</exception>
        [Conditional("DEBUG")]
        [DebuggerStepThrough]
        public static void DictionaryContainsKey<T1, T2>(T1 key, string paramName, IDictionary<T1, T2> dictionary)
        {
            Validate.DictionaryContainsKey(key, paramName, dictionary);
        }

        /// <summary>
        /// The validation passes if the <see cref="IReadOnlyDictionary{TKey,TValue}"/> contains the
        /// given key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="paramName">The parameter name.</param>
        /// <param name="dictionary">The dictionary.</param>
        /// <typeparam name="T1">The type of the keys.</typeparam>
        /// <typeparam name="T2">The type of the values</typeparam>
        /// <exception cref="ValidationException">Throws if the dictionary does not contain the key.</exception>
        [Conditional("DEBUG")]
        [DebuggerStepThrough]
        public static void ReadOnlyDictionaryContainsKey<T1, T2>(T1 key, string paramName, IReadOnlyDictionary<T1, T2> dictionary)
        {
            Validate.ReadOnlyDictionaryContainsKey(key, paramName, dictionary);
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
        public static void DictionaryDoesNotContainKey<T1, T2>(T1 key, string paramName, IDictionary<T1, T2> dictionary)
        {
            Validate.DictionaryDoesNotContainKey(key, paramName, dictionary);
        }

        /// <summary>
        /// The validation passes if the <see cref="IReadOnlyDictionary{TKey,TValue}"/> does not
        /// contain the given key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="paramName">The parameter name.</param>
        /// <param name="dictionary">The dictionary.</param>
        /// <typeparam name="T1">The type of the keys.</typeparam>
        /// <typeparam name="T2">The type of the values.</typeparam>
        /// <exception cref="ValidationException">Throws if the dictionary contains the key.</exception>
        [Conditional("DEBUG")]
        [DebuggerStepThrough]
        public static void ReadOnlyDictionaryDoesNotContainKey<T1, T2>(T1 key, string paramName, IReadOnlyDictionary<T1, T2> dictionary)
        {
            Validate.ReadOnlyDictionaryDoesNotContainKey(key, paramName, dictionary);
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
        public static void Int32NotOutOfRange(
            int value,
            string paramName,
            int lowerBound,
            int upperBound,
            RangeEndPoints endPoints = RangeEndPoints.Inclusive)
        {
            Validate.Int32NotOutOfRange(value, paramName, lowerBound, upperBound, endPoints);
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
        public static void LongNotOutOfRange(
            long value,
            string paramName,
            long lowerBound,
            long upperBound,
            RangeEndPoints endPoints = RangeEndPoints.Inclusive)
        {
            Validate.LongNotOutOfRange(value, paramName, lowerBound, upperBound, endPoints);
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
        public static void DoubleNotOutOfRange(
            double value,
            string paramName,
            double lowerBound,
            double upperBound,
            RangeEndPoints endPoints = RangeEndPoints.Inclusive)
        {
            Validate.DoubleNotOutOfRange(value, paramName, lowerBound, upperBound, endPoints);
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
        public static void DecimalNotOutOfRange(
            decimal value,
            string paramName,
            decimal lowerBound,
            decimal upperBound,
            RangeEndPoints endPoints = RangeEndPoints.Inclusive)
        {
            Validate.DecimalNotOutOfRange(value, paramName, lowerBound, upperBound, endPoints);
        }

        /// <summary>
        /// The validation passes if the value is not an invalid number [or throws].
        /// </summary>
        /// <param name="value">The value to be checked.</param>
        /// <param name="paramName">The parameter name.</param>
        /// <exception cref="ValidationException">Throws if the value is not a valid number.</exception>
        [Conditional("DEBUG")]
        [DebuggerStepThrough]
        public static void DoubleNotInvalidNumber(double value, string paramName)
        {
            Validate.DoubleNotInvalidNumber(value, paramName);
        }
    }
}
