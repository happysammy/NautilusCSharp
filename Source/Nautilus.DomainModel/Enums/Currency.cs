//--------------------------------------------------------------------------------------------------
// <copyright file="Currency.cs" company="Nautech Systems Pty Ltd">
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
    /// Represents an ISO 4217 currency code.
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Correct names")]
    public enum Currency
    {
        /// <summary>
        /// The enumerator value is undefined (invalid).
        /// </summary>
        [InvalidValue]
        Undefined = 0,

        /// <summary>
        /// Australian dollar.
        /// </summary>
        AUD = 036,

        /// <summary>
        /// Canadian dollar.
        /// </summary>
        CAD = 124,

        /// <summary>
        /// Swiss franc.
        /// </summary>
        CHF = 756,

        /// <summary>
        /// Chinese yuan.
        /// </summary>
        CNY = 156,

        /// <summary>
        /// Chinese yuan (off-shore market abbreviation).
        /// </summary>
        CNH = 999,

        /// <summary>
        /// Czech koruna.
        /// </summary>
        CZK = 203,

        /// <summary>
        /// Euro.
        /// </summary>
        EUR = 978,

        /// <summary>
        /// Pound sterling
        /// </summary>
        GBP = 826,

        /// <summary>
        /// Hong Kong dollar.
        /// </summary>
        HKD = 344,

        /// <summary>
        /// Japanese yen.
        /// </summary>
        JPY = 392,

        /// <summary>
        /// The mxn.
        /// </summary>
        MXN = 484,

        /// <summary>
        /// Norwegian krone.
        /// </summary>
        NOK = 578,

        /// <summary>
        /// New Zealand dollar.
        /// </summary>
        NZD = 554,

        /// <summary>
        /// Swedish krona/kronor.
        /// </summary>
        SEK = 752,

        /// <summary>
        /// Turkish lira.
        /// </summary>
        TRY = 949,

        /// <summary>
        /// Singapore dollar.
        /// </summary>
        SGD = 702,

        /// <summary>
        /// United States dollar.
        /// </summary>
        USD = 840,

        /// <summary>
        /// Silver (one troy ounce).
        /// </summary>
        XAG = 961,

        /// <summary>
        /// Platinum (one troy ounce).
        /// </summary>
        XPT = 962,

        /// <summary>
        /// Gold (one troy ounce).
        /// </summary>
        XAU = 959,

        /// <summary>
        /// South African rand.
        /// </summary>
        ZAR = 710,
    }
}
