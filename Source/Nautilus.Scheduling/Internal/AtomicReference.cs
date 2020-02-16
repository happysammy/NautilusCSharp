// -------------------------------------------------------------------------------------------------
// <copyright file="AtomicReference.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   https://nautechsystems.io
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Scheduling.Internal
{
    using System.Threading;

    /// <summary>
    /// Implementation of the java.concurrent.util AtomicReference type.
    ///
    /// Uses <see cref="Volatile"/> internally to enforce ordering of writes
    /// without any explicit locking. .NET's strong memory on write guarantees might already enforce
    /// this ordering, but the addition of the Volatile guarantees it.
    /// </summary>
    /// <typeparam name="T">The type.</typeparam>
    internal class AtomicReference<T>
        where T : class
    {
        private T? atomicValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="AtomicReference{T}"/> class.
        /// </summary>
        internal AtomicReference()
        {
            this.atomicValue = null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AtomicReference{T}"/> class.
        /// </summary>
        /// <param name="originalValue">The original value.</param>
        private AtomicReference(T originalValue)
        {
            this.atomicValue = originalValue;
        }

        /// <summary>
        /// Gets the current value of this <see cref="AtomicReference{T}"/>.
        /// </summary>
        internal T? Value => this.atomicValue ?? Volatile.Read(ref this.atomicValue);

        /// <summary>
        /// Performs an implicit conversion from <see cref="AtomicReference{T}"/> to <typeparamref name="T"/>.
        /// </summary>
        /// <param name="atomicReference">The reference to convert.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator T?(AtomicReference<T> atomicReference) => atomicReference?.Value;

        /// <summary>
        /// Performs an implicit conversion from <typeparamref name="T"/> to <see cref="AtomicReference{T}"/>.
        /// </summary>
        /// <param name="value">The reference to convert.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator AtomicReference<T>(T value) => new AtomicReference<T>(value);

        /// <summary>
        /// If <see cref="Value"/> equals <paramref name="expected"/>, then set the Value to
        /// <paramref name="newValue"/>.
        /// </summary>
        /// <param name="expected">The expected value.</param>
        /// <param name="newValue">The new value.</param>
        /// <returns><c>true</c> if <paramref name="newValue"/> was set.</returns>
        public bool CompareAndSet(T? expected, T newValue)
        {
            var previous = Interlocked.CompareExchange(ref this.atomicValue, newValue, expected);
            return ReferenceEquals(previous, expected);
        }

        /// <summary>
        /// Atomically sets the <see cref="Value"/> to <paramref name="newValue"/> and returns the old <see cref="Value"/>.
        /// </summary>
        /// <param name="newValue">The new value.</param>
        /// <returns>The old value.</returns>
        public T? GetAndSet(T newValue) => Interlocked.Exchange(ref this.atomicValue, newValue);
    }
}
