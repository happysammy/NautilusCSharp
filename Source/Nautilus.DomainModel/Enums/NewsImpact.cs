// -------------------------------------------------------------------------------------------------
// <copyright file="NewsImpact.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.Enums
{
    /// <summary>
    /// The <see cref="NewsImpact"/> enumeration. Represents the relative market impact of known
    /// economic news events.
    /// </summary>
    public enum NewsImpact
    {
        /// <summary>
        /// No news impact.
        /// </summary>
        None = 0,

        /// <summary>
        /// Low news impact
        /// </summary>
        Low = 1,

        /// <summary>
        /// Medium news impact.
        /// </summary>
        Medium = 2,

        /// <summary>
        /// High news impact.
        /// </summary>
        High = 3
    }
}