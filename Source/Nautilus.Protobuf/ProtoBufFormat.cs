// -------------------------------------------------------------------------------------------------
// <copyright file="ProtoBufExtensions.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.ProtoBuf
{
    using System;
    using System.IO;
    using ServiceStack.Web;
    using ServiceStack;
    using global::ProtoBuf.Meta;

    public class ProtoBufFormat : IPlugin, IProtoBufPlugin
    {
        public void Register(IAppHost appHost)
        {
            appHost.ContentTypes.Register(MimeTypes.ProtoBuf, Serialize, Deserialize);
        }

        private static RuntimeTypeModel model;
        public static RuntimeTypeModel Model => model ?? (model = TypeModel.Create());

        public static void Serialize(IRequest requestContext, object dto, Stream outputStream)
        {
            Serialize(dto, outputStream);
        }

        public static void Serialize(object dto, Stream outputStream)
        {
            Model.Serialize(outputStream, dto);
        }

        public static object Deserialize(Type type, Stream fromStream)
        {
            var obj = Model.Deserialize(fromStream, null, type);
            return obj;
        }

        public string GetProto(Type type)
        {
            return Model.GetSchema(type);
        }
    }
}
