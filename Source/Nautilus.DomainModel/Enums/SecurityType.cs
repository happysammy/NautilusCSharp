//--------------------------------------------------------------
// <copyright file="SecurityType.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------

namespace Nautilus.DomainModel.Enums
{
    /// <summary>
    /// The security type.
    /// </summary>
    public enum SecurityType
    {
        /// <summary>
        /// Foreign Exchange security type.
        /// </summary>
        Forex = 0,

        /// <summary>
        /// Bond security type.
        /// </summary>
        Bond = 1,

        /// <summary>
        /// Equity security type.
        /// </summary>
        Equity = 2,

        /// <summary>
        /// Futures security type.
        /// </summary>
        Future = 3,

        /// <summary>
        /// Contract For Difference security type.
        /// </summary>
        Cfd = 4,

        /// <summary>
        /// Option security type.
        /// </summary>
        Option = 5
    }
}