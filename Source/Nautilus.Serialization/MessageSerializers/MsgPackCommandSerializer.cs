// -------------------------------------------------------------------------------------------------
// <copyright file="MsgPackCommandSerializer.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  https://nautechsystems.io
//
//  Licensed under the GNU Lesser General Public License Version 3.0 (the "License");
//  You may not use this file except in compliance with the License.
//  You may obtain a copy of the License at https://www.gnu.org/licenses/lgpl-3.0.en.html
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Serialization.MessageSerializers
{
    using System.Collections.Generic;
    using MessagePack;
    using Nautilus.Common.Interfaces;
    using Nautilus.Core.Correctness;
    using Nautilus.Core.Message;
    using Nautilus.DomainModel.Commands;
    using Nautilus.DomainModel.Entities;
    using Nautilus.Serialization.MessageSerializers.Internal;

    /// <summary>
    /// Provides a <see cref="Command"/> message binary serializer for the MessagePack specification.
    /// </summary>
    public sealed class MsgPackCommandSerializer : IMessageSerializer<Command>
    {
        private readonly IdentifierCache identifierCache;
        private readonly MsgPackOrderSerializer orderSerializer;

        /// <summary>
        /// Initializes a new instance of the <see cref="MsgPackCommandSerializer"/> class.
        /// </summary>
        public MsgPackCommandSerializer()
        {
            this.identifierCache = new IdentifierCache();
            this.orderSerializer = new MsgPackOrderSerializer();
        }

        /// <inheritdoc />
        public byte[] Serialize(Command command)
        {
            var package = new Dictionary<string, byte[]>
            {
                { nameof(Command.Type), ObjectSerializer.Serialize(command.Type) },
                { nameof(Command.Id), ObjectSerializer.Serialize(command.Id) },
                { nameof(Command.Timestamp), ObjectSerializer.Serialize(command.Timestamp) },
            };

            switch (command)
            {
                case AccountInquiry cmd:
                    package.Add(nameof(cmd.TraderId), ObjectSerializer.Serialize(cmd.TraderId));
                    package.Add(nameof(cmd.AccountId), ObjectSerializer.Serialize(cmd.AccountId));
                    break;
                case SubmitOrder cmd:
                    package.Add(nameof(cmd.TraderId), ObjectSerializer.Serialize(cmd.TraderId));
                    package.Add(nameof(cmd.StrategyId), ObjectSerializer.Serialize(cmd.StrategyId));
                    package.Add(nameof(cmd.AccountId), ObjectSerializer.Serialize(cmd.AccountId));
                    package.Add(nameof(cmd.PositionId), ObjectSerializer.Serialize(cmd.PositionId));
                    package.Add(nameof(cmd.Order), this.orderSerializer.Serialize(cmd.Order));
                    break;
                case SubmitAtomicOrder cmd:
                    package.Add(nameof(cmd.TraderId), ObjectSerializer.Serialize(cmd.TraderId));
                    package.Add(nameof(cmd.StrategyId), ObjectSerializer.Serialize(cmd.StrategyId));
                    package.Add(nameof(cmd.AccountId), ObjectSerializer.Serialize(cmd.AccountId));
                    package.Add(nameof(cmd.PositionId), ObjectSerializer.Serialize(cmd.PositionId));
                    package.Add(nameof(cmd.AtomicOrder.Entry), this.orderSerializer.Serialize(cmd.AtomicOrder.Entry));
                    package.Add(nameof(cmd.AtomicOrder.StopLoss), this.orderSerializer.Serialize(cmd.AtomicOrder.StopLoss));
                    package.Add(nameof(cmd.AtomicOrder.TakeProfit), this.orderSerializer.SerializeNullable(cmd.AtomicOrder.TakeProfit));
                    break;
                case ModifyOrder cmd:
                    package.Add(nameof(cmd.TraderId), ObjectSerializer.Serialize(cmd.TraderId));
                    package.Add(nameof(cmd.AccountId), ObjectSerializer.Serialize(cmd.AccountId));
                    package.Add(nameof(cmd.OrderId), ObjectSerializer.Serialize(cmd.OrderId));
                    package.Add(nameof(cmd.ModifiedQuantity), ObjectSerializer.Serialize(cmd.ModifiedQuantity));
                    package.Add(nameof(cmd.ModifiedPrice), ObjectSerializer.Serialize(cmd.ModifiedPrice));
                    break;
                case CancelOrder cmd:
                    package.Add(nameof(cmd.TraderId), ObjectSerializer.Serialize(cmd.TraderId));
                    package.Add(nameof(cmd.AccountId), ObjectSerializer.Serialize(cmd.AccountId));
                    package.Add(nameof(cmd.OrderId), ObjectSerializer.Serialize(cmd.OrderId));
                    package.Add(nameof(cmd.CancelReason), ObjectSerializer.Serialize(cmd.CancelReason));
                    break;
                default:
                    throw ExceptionFactory.InvalidSwitchArgument(command, nameof(command));
            }

            return MessagePackSerializer.Serialize(package);
        }

        /// <inheritdoc />
        public Command Deserialize(byte[] dataBytes)
        {
            var unpacked = MessagePackSerializer.Deserialize<Dictionary<string, byte[]>>(dataBytes);

            var command = ObjectDeserializer.AsString(unpacked[nameof(Command.Type)]);
            var id = ObjectDeserializer.AsGuid(unpacked[nameof(Command.Id)]);
            var timestamp = ObjectDeserializer.AsZonedDateTime(unpacked[nameof(Command.Timestamp)]);

            switch (command)
            {
                case nameof(AccountInquiry):
                    return new AccountInquiry(
                        this.identifierCache.TraderId(unpacked),
                        this.identifierCache.AccountId(unpacked),
                        id,
                        timestamp);
                case nameof(SubmitOrder):
                    return new SubmitOrder(
                        this.identifierCache.TraderId(unpacked),
                        this.identifierCache.AccountId(unpacked),
                        this.identifierCache.StrategyId(unpacked),
                        ObjectDeserializer.AsPositionId(unpacked),
                        this.orderSerializer.Deserialize(unpacked[nameof(SubmitOrder.Order)]),
                        id,
                        timestamp);
                case nameof(SubmitAtomicOrder):
                    return new SubmitAtomicOrder(
                        this.identifierCache.TraderId(unpacked),
                        this.identifierCache.AccountId(unpacked),
                        this.identifierCache.StrategyId(unpacked),
                        ObjectDeserializer.AsPositionId(unpacked),
                        new AtomicOrder(
                            this.orderSerializer.Deserialize(unpacked[nameof(AtomicOrder.Entry)]),
                            this.orderSerializer.Deserialize(unpacked[nameof(AtomicOrder.StopLoss)]),
                            this.orderSerializer.DeserializeNullable(unpacked[nameof(AtomicOrder.TakeProfit)])),
                        id,
                        timestamp);
                case nameof(ModifyOrder):
                    return new ModifyOrder(
                        this.identifierCache.TraderId(unpacked),
                        this.identifierCache.AccountId(unpacked),
                        ObjectDeserializer.AsOrderId(unpacked),
                        ObjectDeserializer.AsQuantity(unpacked[nameof(ModifyOrder.ModifiedQuantity)]),
                        ObjectDeserializer.AsPrice(unpacked[nameof(ModifyOrder.ModifiedPrice)]),
                        id,
                        timestamp);
                case nameof(CancelOrder):
                    return new CancelOrder(
                        this.identifierCache.TraderId(unpacked),
                        this.identifierCache.AccountId(unpacked),
                        ObjectDeserializer.AsOrderId(unpacked),
                        ObjectDeserializer.AsString(unpacked[nameof(CancelOrder.CancelReason)]),
                        id,
                        timestamp);
                default:
                    throw ExceptionFactory.InvalidSwitchArgument(command, nameof(command));
            }
        }
    }
}
