//--------------------------------------------------------------------------------------------------
// <copyright file="NewsEventResponse.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace NautilusDB.Service.Responses
{
    using Nautilus.Data.Types;

    public class EconomicEventResponse
    {
        public EconomicEventResponse(
            bool isSuccess,
            string message,
            EconomicEventFrame economicEvents)
        {
            this.IsSuccess = isSuccess;
            this.Message = message;
            this.EconomicEvents = economicEvents;
        }

        public bool IsSuccess { get; private set; }

        public string Message { get; private set; }

        public EconomicEventFrame EconomicEvents { get; private set; }
    }
}
