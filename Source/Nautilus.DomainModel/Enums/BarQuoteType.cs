//--------------------------------------------------------------
// <copyright file="BarQuoteType.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------

namespace Nautilus.DomainModel.Enums
{
    /// <summary>
    /// The quote type used to construct a trade bar.
    /// </summary>
    public enum BarQuoteType
    {
        /// <summary>
        /// The bid quote type.
        /// </summary>
        Bid = 0,

        /// <summary>
        /// The ask quote type.
        /// </summary>
        Ask = 1,

        /// <summary>
        /// The mid quote type.
        /// </summary>
        Mid = 2
    }
}
