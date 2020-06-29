//--------------------------------------------------------------------------------------------------
// <copyright file="Label.cs" company="Nautech Systems Pty Ltd">
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

namespace Nautilus.Core.Types
{
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Correctness;

    /// <summary>
    /// Represents a validated label.
    /// </summary>
    [Immutable]
    public sealed class Label : Identifier<Label>
    {
        private const string None = nameof(None);

        /// <summary>
        /// Initializes a new instance of the <see cref="Label"/> class.
        /// </summary>
        /// <param name="value">The label value.</param>
        public Label(string value = None)
            : base(value)
        {
            Debug.NotEmptyOrWhiteSpace(value, nameof(value));
        }

        /// <summary>
        /// Returns a value indicating whether the label is equal to 'None'.
        /// </summary>
        /// <returns>True if the label value is 'None', else False.</returns>
        public bool IsNone()
        {
            return this.Value == None;
        }

        /// <summary>
        /// Returns a value indicating whether the label is not equal to 'None'.
        /// </summary>
        /// <returns>True if the label value is not 'None', else False.</returns>
        public bool NotNone()
        {
            return this.Value != None;
        }
    }
}
