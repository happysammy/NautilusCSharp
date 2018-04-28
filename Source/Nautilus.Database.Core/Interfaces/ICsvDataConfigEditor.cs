//--------------------------------------------------------------
// <copyright file="ICsvDataConfigEditor.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------
using System.Collections.Generic;
using NautechSystems.CSharp.CQS;
using NodaTime;

namespace Nautilus.Database.Core.Interfaces
{
    /// <summary>
    /// Temporary interface to provide a hook into the Dukascopy editor.
    /// </summary>
    public interface ICsvDataConfigEditor
    {
        bool InitialFromDateSpecified { get; }

        CommandResult InitialFromDateConfigCsv(
            IReadOnlyList<string> currencyPairs,
            ZonedDateTime toDateTime);

        CommandResult UpdateConfigCsv(
            IReadOnlyList<string> currencyPairs,
            ZonedDateTime fromDateTime,
            ZonedDateTime toDateTime);
    }
}