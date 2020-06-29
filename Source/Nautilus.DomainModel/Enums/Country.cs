//--------------------------------------------------------------------------------------------------
// <copyright file="Country.cs" company="Nautech Systems Pty Ltd">
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
    using Nautilus.Core.Annotations;

    /// <summary>
    /// The country an economic news event effects.
    /// </summary>
    public enum Country
    {
        /// <summary>
        /// The enumerator value is undefined (invalid).
        /// </summary>
        [InvalidValue]
        Undefined = 0,

        /// <summary>
        /// The United States country type.
        /// </summary>
        UnitedStates,

        /// <summary>
        /// The United Kingdom country type.
        /// </summary>
        UnitedKingdom,

        /// <summary>
        /// The European Monetary Union country type.
        /// </summary>
        EuropeanMonetaryUnion,

        /// <summary>
        /// The Australia country type.
        /// </summary>
        Australia,

        /// <summary>
        /// The Japan country type.
        /// </summary>
        Japan,

        /// <summary>
        /// The Canada country type.
        /// </summary>
        Canada,

        /// <summary>
        /// The New Zealand country type.
        /// </summary>
        NewZealand,

        /// <summary>
        /// The China country type.
        /// </summary>
        China,

        /// <summary>
        /// The Switzerland country type.
        /// </summary>
        Switzerland,

        /// <summary>
        /// The France country type.
        /// </summary>
        France,

        /// <summary>
        /// The Spain country type.
        /// </summary>
        Spain,

        /// <summary>
        /// The Italy country type.
        /// </summary>
        Italy,

        /// <summary>
        /// The Portugal country type.
        /// </summary>
        Portugal,

        /// <summary>
        /// The Germany country type.
        /// </summary>
        Germany,

        /// <summary>
        /// The Greece country type.
        /// </summary>
        Greece,
    }
}
