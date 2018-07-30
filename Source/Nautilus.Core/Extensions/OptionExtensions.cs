//--------------------------------------------------------------------------------------------------
// <copyright file="OptionExtensions.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Core.Extensions
{
    using System;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.CQS;
    using Nautilus.Core.Validation;

    /// <summary>
    /// Provides useful <see cref="Option{T}"/> extension methods
    /// </summary>
    [Immutable]
    public static class OptionExtensions
    {
        /// <summary>
        /// Converts an <see cref="Option{T}"/> to a <see cref="QueryResult{T}"/>.
        /// </summary>
        /// <param name="option">The option (cannot be null).</param>
        /// <param name="errorMessage">The error message (cannot be null).</param>
        /// <typeparam name="T">The type.</typeparam>
        /// <returns>The queries result.</returns>
        public static QueryResult<T> ToResult<T>(this Option<T> option, string errorMessage)
        {
            Debug.NotNull(option, nameof(option));
            Debug.NotNull(errorMessage, nameof(errorMessage));

            return option.HasNoValue
                ? QueryResult<T>.Fail(errorMessage)
                : QueryResult<T>.Ok(option.Value);
        }

        /// <summary>
        /// Unwraps the <see cref="Option{T}"/> returning the given default value if there is no
        /// value.
        /// </summary>
        /// <param name="option">The option (cannot be null).</param>
        /// <param name="defaultValue">The default Value.</param>
        /// <typeparam name="T">The type.</typeparam>
        /// <returns>A type.</returns>
        public static T Unwrap<T>(this Option<T> option, [CanBeNull] T defaultValue = default(T))
        {
            Debug.NotNull(option, nameof(option));

            return option.Unwrap(x => x, defaultValue);
        }

        /// <summary>
        /// Unwraps the <see cref="Option{T}"/> returning the given default value if there is no
        /// value.
        /// </summary>
        /// <param name="option">The option (cannot be null).</param>
        /// <param name="selector">The selector (cannot be null).</param>
        /// <param name="defaultValue">The default value.</param>
        /// <typeparam name="T">The type.</typeparam>
        /// <typeparam name="TK">The key type.</typeparam>
        /// <returns>The unwrapped type.</returns>
        public static TK Unwrap<T, TK>(
            this Option<T> option,
            Func<T, TK> selector,
            [CanBeNull] TK defaultValue = default)
        {
            Debug.NotNull(option, nameof(option));
            Debug.NotNull(selector, nameof(selector));

            return option.HasValue
                ? selector(option.Value)
                : defaultValue;
        }

        /// <summary>
        /// Where the <see cref="Option{T}"/> has no value then returns the default value for the
        /// type. Otherwise evaluates the given predicate and returns the <see cref="Option{T}"/>
        /// value or the default value for the type).
        /// </summary>
        /// <param name="option">The option (cannot be null).</param>
        /// <param name="predicate">The predicate (cannot be null).</param>
        /// <typeparam name="T">The type.</typeparam>
        /// <returns>The option.</returns>
        public static Option<T> Where<T>(this Option<T> option, Func<T, bool> predicate)
        {
            Debug.NotNull(option, nameof(option));
            Debug.NotNull(predicate, nameof(predicate));

            if (option.HasNoValue)
            {
                return default(T);
            }

            return predicate(option.Value)
                ? option
                : default(T);
        }

        /// <summary>
        /// Selects the default value of the selector type if the <see cref="Option{K}"/> has no
        /// value, otherwise returns an <see cref="Option{K}"/>.
        /// </summary>
        /// <param name="option">The option (cannot be null).</param>
        /// <param name="selector">The selector (cannot be null).</param>
        /// <typeparam name="T">The type.</typeparam>
        /// <typeparam name="TK">The key type.</typeparam>
        /// <returns>The option.</returns>
        public static Option<TK> Select<T, TK>(this Option<T> option, Func<T, TK> selector)
        {
            Debug.NotNull(option, nameof(option));
            Debug.NotNull(selector, nameof(selector));

            return option.HasNoValue
                ? default(TK)
                : selector(option.Value);
        }

        /// <summary>
        /// Selects the default value of the selector type if the <see cref="Option{K}"/> has no
        /// value, otherwise returns an <see cref="Option{K}"/>.
        /// </summary>
        /// <param name="option">The option (cannot be null).</param>
        /// <param name="selector">The selector (cannot be null).</param>
        /// <typeparam name="T">The type.</typeparam>
        /// <typeparam name="TK">The option type.</typeparam>
        /// <returns>The option.</returns>
        public static Option<TK> Select<T, TK>(this Option<T> option, Func<T, Option<TK>> selector)
        {
            Debug.NotNull(option, nameof(option));
            Debug.NotNull(selector, nameof(selector));

            return option.HasNoValue
                ? default(TK)
                : selector(option.Value);
        }

        /// <summary>
        /// Executes the given <see cref="Action{T}"/> where the <see cref="Option{T}"/> has a value.
        /// </summary>
        /// <param name="option">The option (cannot be null).</param>
        /// <param name="action">The action (cannot be null).</param>
        /// <typeparam name="T">The type.</typeparam>
        public static void Execute<T>(this Option<T> option, Action<T> action)
        {
            Debug.NotNull(option, nameof(option));
            Debug.NotNull(action, nameof(action));

            if (option.HasNoValue)
            {
                return;
            }

            action(option.Value);
        }
    }
}
