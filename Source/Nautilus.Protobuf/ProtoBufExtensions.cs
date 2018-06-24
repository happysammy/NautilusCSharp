// -------------------------------------------------------------------------------------------------
// <copyright file="ProtoBufExtensions.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.ProtoBuf
{
    using ServiceStack.Text;

    public static class ProtoBufExtensions
    {
        public static byte[] ToProtoBuf<T>(this T obj)
        {
            using (var ms = MemoryStreamFactory.GetStream())
            {
                ProtoBufFormat.Serialize(obj, ms);
                var bytes = ms.ToArray();
                return bytes;
            }
        }

        public static T FromProtoBuf<T>(this byte[] bytes)
        {
            using (var ms = MemoryStreamFactory.GetStream(bytes))
            {
                var obj = (T)ProtoBufFormat.Deserialize(typeof(T), ms);
                return obj;
            }
        }
    }
}
