//--------------------------------------------------------------------------------------------------
// <copyright file="State.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Enums
{
    using Nautilus.Core.Annotations;

    /// <summary>
    /// Represents the status of a service component.
    /// </summary>
    public enum State
    {
        /// <summary>
        /// The component state is unknown (this is an invalid value).
        /// </summary>
        [InvalidValue]
        Unknown = 0,

        /// <summary>
        /// The component is initialized.
        /// </summary>
        Initialized = 1,

        /// <summary>
        /// The component is running normally.
        /// </summary>
        Running = 2,

        /// <summary>
        /// The component has gracefully stopped.
        /// </summary>
        Stopped = 1,

        /// <summary>
        /// The component has failed and will not process further work.
        /// </summary>
        Failed = 3,
    }
}
