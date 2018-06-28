// -------------------------------------------------------------------------------------------------
// <copyright file="RedisServiceStack.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Redis
{
    using Nautilus.Core.Extensions;
    using NodaTime;
    using ServiceStack.Text;

    /// <summary>
    /// Provides configuration options for Service Stack with Redis.
    /// </summary>
    public static class RedisServiceStack
    {
        /// <summary>
        /// Configures Service Stack to work with Redis.
        /// </summary>
        public static void ConfigureServiceStack()
        {
            JsConfig<ZonedDateTime>.RawSerializeFn = ZonedDateTimeExtensions.ToIsoString;
            JsConfig<ZonedDateTime>.RawDeserializeFn = ZonedDateTimeExtensions.ToZonedDateTimeFromIso;
        }
    }
}
