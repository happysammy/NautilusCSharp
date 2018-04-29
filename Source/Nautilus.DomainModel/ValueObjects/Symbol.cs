//---------------------------------------------------------------------------------------------------------------------
// <copyright file="Symbol.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
//---------------------------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.ValueObjects
{
    using NautechSystems.CSharp.Annotations;
    using NautechSystems.CSharp.Validation;
    using Nautilus.DomainModel.Enums;

    /// <summary>
    /// Represents a financial market instruments symbol.
    /// </summary>
    [Immutable]
    public sealed class Symbol : ValidString
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Symbol"/> class.
        /// </summary>
        /// <param name="code">The symbols code.</param>
        /// <param name="exchange">The symbols exchange.</param>
        /// <exception cref="ValidationException">Throws if the code is null.</exception>
        public Symbol(string code, Exchange exchange) : base($"{code}.{exchange}")
        {
            Validate.NotNull(code, nameof(code));

            this.Code = code;
            this.Exchange = exchange;
        }

        /// <summary>
        /// Gets the symbols code.
        /// </summary>
        public string Code { get; }

        /// <summary>
        /// Gets the symbols exchange.
        /// </summary>
        public Exchange Exchange { get; }
    }
}
