//--------------------------------------------------------------------------------------------------
// <copyright file="FxcmTags.cs" company="Nautech Systems Pty Ltd">
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

namespace Nautilus.Fxcm
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Provides custom FXCM FIX message tags.
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Consistency with FIX conventions.")]
    public static class FxcmTags
    {
        /// <summary>
        /// Gets the FXCM SymID tag number (9000).
        /// </summary>
        public static int SymID => 9000;

        /// <summary>
        /// Gets the FXCM SymPrecision tag number (9001).
        /// </summary>
        public static int SymPrecision => 9001;

        /// <summary>
        /// Gets the FXCM SymPointSize tag number (9002).
        /// </summary>
        public static int SymPointSize => 9002;

        /// <summary>
        /// Gets the FXCM SymInterestBuy tag number (9003).
        /// </summary>
        public static int SymInterestBuy => 9003;

        /// <summary>
        /// Gets the FXCMSymInterestBuy tag number (9004).
        /// </summary>
        public static int SymInterestSell => 9004;

        /// <summary>
        /// Gets the FXCM SymSortOrder tag number (9005).
        /// </summary>
        public static int SymSortOrder => 9005;

        /// <summary>
        /// Gets the FXCM StartDate tag number (9012).
        /// </summary>
        public static int StartDate => 9012;

        /// <summary>
        /// Gets the FXCM StartTime tag number (9013).
        /// </summary>
        public static int StartTime => 9013;

        /// <summary>
        /// Gets the FXCM EndDate tag number (9014).
        /// </summary>
        public static int EndDate => 9014;

        /// <summary>
        /// Gets the FXCM EndTime tag number (9015).
        /// </summary>
        public static int EndTime => 9015;

        /// <summary>
        /// Gets the FXCM NoParam tag number (9016).
        /// </summary>
        public static int NoParam => 9016;

        /// <summary>
        /// Gets the FXCM ParamName tag number (9017).
        /// </summary>
        public static int ParamName => 9017;

        /// <summary>
        /// Gets the FXCM ParamValue tag number (9018).
        /// </summary>
        public static int ParamValue => 9018;

        /// <summary>
        /// Gets the FXCM ServerTimeZone tag number (9019).
        /// </summary>
        public static int ServerTimeZone => 9019;

        /// <summary>
        /// Gets the FXCM ContinuousFlag tag number (9020).
        /// </summary>
        public static int ContinuousFlag => 9020;

        /// <summary>
        /// Gets the FXCM RequestRejectReason tag number (9025).
        /// </summary>
        public static int RequestRejectReason => 9025;

        /// <summary>
        /// Gets the FXCM ErrorDetails tag number (9029).
        /// </summary>
        public static int ErrorDetails => 9029;

        /// <summary>
        /// Gets the FXCM ServerTimeZoneName tag number (9030).
        /// </summary>
        public static int ServerTimeZoneName => 9030;

        /// <summary>
        /// Gets the FXCM UsedMarginLiquidation tag number (9038).
        /// </summary>
        public static int UsedMarginLiquidation => 9038;

        /// <summary>
        /// Gets the FXCM PosInterest tag number (9040).
        /// </summary>
        public static int PosInterest => 9040;

        /// <summary>
        /// Gets the FXCM PosID tag number (9041).
        /// </summary>
        public static int PosID => 9041;

        /// <summary>
        /// Gets the FXCM PosOpenTime tag number (9042).
        /// </summary>
        public static int PosOpenTime => 9042;

        /// <summary>
        /// Gets the FXCM CloseSettlePrice tag number (9043).
        /// </summary>
        public static int CloseSettlePrice => 9043;

        /// <summary>
        /// Gets the FXCM CloseTime tag number (9044).
        /// </summary>
        public static int CloseTime => 9044;

        /// <summary>
        /// Gets the FXCM MarginCall tag number (9045).
        /// </summary>
        public static int MarginCall => 9045;

        /// <summary>
        /// Gets the FXCM UsedMarginMaintenance tag number (9046).
        /// </summary>
        public static int UsedMarginMaintenance => 9046;

        /// <summary>
        /// Gets the FXCM CashDaily tag number (9047).
        /// </summary>
        public static int CashDaily => 9047;

        /// <summary>
        /// Gets the FXCM CloseClOrdID tag number (9048).
        /// </summary>
        public static int CloseClOrdID => 9048;

        /// <summary>
        /// Gets the FXCM CloseSecondaryClOrdID tag number (9049).
        /// </summary>
        public static int CloseSecondaryClOrdID => 9049;

        /// <summary>
        /// Gets the FXCM OrdType tag number (9050).
        /// </summary>
        public static int OrdType => 9050;

        /// <summary>
        /// Gets the FXCM OrdStatus tag number (9051).
        /// </summary>
        public static int OrdStatus => 9051;

        /// <summary>
        /// Gets the FXCM ClosePNL tag number (9052).
        /// </summary>
        public static int ClosePNL => 9052;

        /// <summary>
        /// Gets the FXCM PosCommission tag number (9053).
        /// </summary>
        public static int PosCommission => 9053;

        /// <summary>
        /// Gets the FXCM CloseOrderID tag number (9054).
        /// </summary>
        public static int CloseOrderID => 9054;

        /// <summary>
        /// Gets the FXCM MaxNoResults tag number (9060).
        /// </summary>
        public static int MaxNoResults => 9060;

        /// <summary>
        /// Gets the FXCM PegFluctuatePts tag number (9061).
        /// </summary>
        public static int PegFluctuatePts => 9061;

        /// <summary>
        /// Gets the FXCM SubscriptionStatus tag number (9076).
        /// </summary>
        public static int SubscriptionStatus => 9076;

        /// <summary>
        /// Gets the FXCM PosIDRef tag number (9078).
        /// </summary>
        public static int PosIDRef => 9078;

        /// <summary>
        /// Gets the FXCM ContingencyID tag number (9079).
        /// </summary>
        public static int ContingencyID => 9079;

        /// <summary>
        /// Gets the FXCM ProductID tag number (9080).
        /// </summary>
        public static int ProductID => 9080;

        /// <summary>
        /// Gets the FXCM CondDistStop tag number (9090).
        /// </summary>
        public static int CondDistStop => 9090;

        /// <summary>
        /// Gets the FXCM CondDistLimit tag number (9091).
        /// </summary>
        public static int CondDistLimit => 9091;

        /// <summary>
        /// Gets the FXCM CondDistEntryStop tag number (9092).
        /// </summary>
        public static int CondDistEntryStop => 9092;

        /// <summary>
        /// Gets the FXCM CondDistEntryLimit tag number (9093).
        /// </summary>
        public static int CondDistEntryLimit => 9093;

        /// <summary>
        /// Gets the FXCM MaxQuantity tag number (9094).
        /// </summary>
        public static int MaxQuantity => 9094;

        /// <summary>
        /// Gets the FXCM MinQuantity tag number (9095).
        /// </summary>
        public static int MinQuantity => 9095;

        /// <summary>
        /// Gets the FXCM TradingStatus tag number (9096).
        /// </summary>
        public static int TradingStatus => 9096;
    }
}
