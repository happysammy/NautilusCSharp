// -------------------------------------------------------------------------------------------------
// <copyright file="MsgPackCommandSerializer.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.MsgPack
{
    using Nautilus.Common.Interfaces;
    using Nautilus.Core;

    public class MsgPackCommandSerializer : ICommandSerializer
    {
        public byte[] Serialize(Command command)
        {
            throw new System.NotImplementedException();
        }

        public Command Deserialize(byte[] commandBytes)
        {
            throw new System.NotImplementedException();
        }
    }
}
