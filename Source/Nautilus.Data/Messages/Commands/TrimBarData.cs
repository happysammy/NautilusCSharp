//--------------------------------------------------------------------------------------------------
// <copyright file="TrimBarData.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Data.Messages.Commands
{
    using System;
    using Nautilus.Core;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Collections;
    using Nautilus.Core.Validation;
    using Nautilus.DomainModel.Enums;
    using NodaTime;

    /// <summary>
    /// The command to trim the bar data keys held in the database to be equal to the size of the
    /// given rolling window.
    /// </summary>
    [Immutable]
    public class TrimBarData : Command
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TrimBarData"/> class.
        /// </summary>
        /// <param name="resolutions">The bar data resolutions to trim.</param>
        /// <param name="rollingWindow">The bar data rolling window size to trim to.</param>
        /// <param name="id">The message identifier.</param>
        /// <param name="timestamp">The message timestamp.</param>
        public TrimBarData(
            ReadOnlyList<Resolution> resolutions,
            int rollingWindow,
            Guid id,
            ZonedDateTime timestamp)
            : base(id, timestamp)
        {
            Debug.NotDefault(id, nameof(id));
            Debug.NotDefault(timestamp, nameof(timestamp));
            Debug.PositiveInt32(rollingWindow, nameof(rollingWindow));

            this.Resolutions = resolutions;
            this.RollingWindowSize = rollingWindow;
        }

        /// <summary>
        /// Gets the resolutions for the bar data trimming operation.
        /// </summary>
        public ReadOnlyList<Resolution> Resolutions { get; }

        /// <summary>
        /// Gets the rolling window size for the bar data trimming operation.
        /// </summary>
        public int RollingWindowSize { get; }
    }
}
