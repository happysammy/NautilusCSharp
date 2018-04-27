// -------------------------------------------------------------------------------------------------
// <copyright file="Indicator.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Indicators.Base
{
    using NautechSystems.CSharp;
    using NautechSystems.CSharp.Validation;
    using NodaTime;

    /// <summary>
    /// The indicator base.
    /// </summary>
    public abstract class Indicator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Indicator"/> class.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        protected Indicator(string name)
        {
            Validate.NotNull(name, nameof(name));

            this.Name = name;
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets or sets a value indicating whether initialized.
        /// </summary>
        public bool Initialized { get; protected set; }

        /// <summary>
        /// Gets or sets the last time.
        /// </summary>
        public Option<ZonedDateTime?> LastTime { get; protected set; }
    }
}