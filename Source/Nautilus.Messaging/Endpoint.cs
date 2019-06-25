// -------------------------------------------------------------------------------------------------
// <copyright file="Endpoint.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.io
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Messaging
{
    using System;
    using System.Reflection.Emit;
    using Nautilus.Core;
    using Nautilus.Core.Annotations;
    using Nautilus.Messaging.Interfaces;

    /// <inheritdoc />
    [Immutable]
    public sealed class Endpoint : IEndpoint
    {
        private readonly Func<object, bool> target;

        /// <summary>
        /// Initializes a new instance of the <see cref="Endpoint"/> class.
        /// </summary>
        /// <param name="target">The target delegate for the end point.</param>
        public Endpoint(Func<object, bool> target)
        {
            this.target = target;
        }

        /// <summary>
        /// Returns a value indicating whether the <see cref="Label"/>s are equal.
        /// </summary>
        /// <param name="left">The left object.</param>
        /// <param name="right">The right object.</param>
        /// <returns>A <see cref="bool"/>.</returns>
        public static bool operator ==(Endpoint left, Endpoint right)
        {
            if (left is null && right is null)
            {
                return true;
            }

            if (left is null || right is null)
            {
                return false;
            }

            return left.Equals(right);
        }

        /// <summary>
        /// Returns a value indicating whether the <see cref="Endpoint"/>s are not equal.
        /// </summary>
        /// <param name="left">The left object.</param>
        /// <param name="right">The right object.</param>
        /// <returns>A <see cref="bool"/>.</returns>
        public static bool operator !=(Endpoint left,  Endpoint right) => !(left == right);

        /// <inheritdoc />
        public void Send(object message)
        {
            this.target.Invoke(message);
        }

        /// <summary>
        /// Returns a value indicating whether this <see cref="Endpoint"/> is equal
        /// to the given <see cref="object"/>.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns>A <see cref="bool"/>.</returns>
        public override bool Equals(object other) => other is Endpoint endpoint && this.Equals(endpoint);

        /// <summary>
        /// Returns a value indicating whether this <see cref="Endpoint"/> is equal
        /// to the given <see cref="Endpoint"/>.
        /// </summary>
        /// <param name="other">The other object.</param>
        /// <returns>A <see cref="bool"/>.</returns>
        public bool Equals(Endpoint other)
        {
            return this.target == other.target;
        }

        /// <summary>
        /// Returns the hash code of the <see cref="Endpoint"/>.
        /// </summary>
        /// <returns>An <see cref="int"/>.</returns>
        public override int GetHashCode()
        {
            return Hash.GetCode(this.target);
        }
    }
}
