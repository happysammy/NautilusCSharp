//--------------------------------------------------------------------------------------------------
// <copyright file="SecurityType.cs" company="Nautech Systems Pty Ltd">
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

namespace Nautilus.DomainModel.Enums
{
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.Core.Annotations;

    /// <summary>
    /// The type of security.
    /// </summary>.
    public enum SecurityType
    {
        /// <summary>
        /// The enumerator value is undefined (invalid).
        /// </summary>
        [InvalidValue]
        Undefined = 0,

        /// <summary>
        /// The foreign exchange security type.
        /// </summary>
        Forex = 1,

        /// <summary>
        /// The bond security type.
        /// </summary>
        Bond = 2,

        /// <summary>
        /// The equity security type.
        /// </summary>
        Equity = 3,

        /// <summary>
        /// The futures security type.
        /// </summary>
        Futures = 4,

        /// <summary>
        /// The contract for difference security type.
        /// </summary>
        [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Correct name")]
        CFD = 5,

        /// <summary>
        /// The option security type.
        /// </summary>
        Option = 6,

        /// <summary>
        /// The crypto security type.
        /// </summary>
        Crypto = 7,
    }
}
