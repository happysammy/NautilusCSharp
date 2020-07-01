//--------------------------------------------------------------------------------------------------
// <copyright file="BarBuilder.cs" company="Nautech Systems Pty Ltd">
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

using Nautilus.Core.Correctness;
using Nautilus.DomainModel.ValueObjects;
using NodaTime;

namespace Nautilus.Data.Aggregation
{
    /// <summary>
    /// Provides a builder for creating <see cref="Bar"/> objects.
    /// </summary>
    public sealed class BarBuilder
    {
        private readonly Price open;

        private Price high;
        private Price low;
        private Price close;
        private decimal volume;

        /// <summary>
        /// Initializes a new instance of the <see cref="BarBuilder"/> class.
        /// </summary>
        /// <param name="open">The open price for the bar.</param>
        public BarBuilder(Price open)
        {
            this.open = open;
            this.high = open;
            this.low = open;
            this.close = open;
            this.volume += 1;
        }

        /// <summary>
        /// Updates the bar builder with the given quote price.
        /// </summary>
        /// <param name="quote">The quote price.</param>
        public void Update(Price quote)
        {
            if (quote > this.high)
            {
                this.high = quote;
            }

            if (quote < this.low)
            {
                this.low = quote;
            }

            this.close = quote;

            this.volume += 1;
        }

        /// <summary>
        /// Creates and returns a new <see cref="Bar"/> based on the values held by the builder.
        /// </summary>
        /// <param name="closeTime">The close time of the bar.</param>
        /// <returns>A <see cref="Bar"/>.</returns>
        public Bar Build(ZonedDateTime closeTime)
        {
            Debug.NotDefault(closeTime, nameof(closeTime));

            return new Bar(
                this.open,
                this.high,
                this.low,
                this.close,
                Volume.Create(this.volume),
                closeTime);
        }
    }
}
