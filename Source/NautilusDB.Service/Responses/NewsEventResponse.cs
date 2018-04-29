//--------------------------------------------------------------------------------------------------
// <copyright file="NewsEventResponse.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace NautilusDB.Service.Responses
{
    using Nautilus.Database.Core.Types;

    public class NewsEventResponse
    {
        public NewsEventResponse(
            bool isSuccess,
            string message,
            EconomicNewsEventFrame economicNewsEvents)
        {
            this.IsSuccess = isSuccess;
            this.Message = message;
            this.EconomicNewsEvents = economicNewsEvents;
        }

        public bool IsSuccess { get; private set; }

        public string Message { get; private set; }

        public EconomicNewsEventFrame EconomicNewsEvents { get; private set; }
    }
}
