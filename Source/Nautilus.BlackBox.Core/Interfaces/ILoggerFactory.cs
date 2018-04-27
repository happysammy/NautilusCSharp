// -------------------------------------------------------------------------------------------------
// <copyright file="ILoggerFactory.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.BlackBox.Core.Interfaces
{
    using Nautilus.BlackBox.Core.Enums;
    using Nautilus.DomainModel.ValueObjects;

    /// <summary>
    /// The <see cref="ILoggerFactory"/> interface.
    /// </summary>
    public interface ILoggerFactory
    {
        /// <summary>
        /// Creates and returns a new <see cref="ILogger"/> from the given inputs.
        /// </summary>
        /// <param name="service">The black box service context.</param>
        /// <param name="component">The component label.</param>
        /// <returns>A <see cref="ILogger"/>.</returns>
        ILogger Create(BlackBoxService service, Label component);
    }
}