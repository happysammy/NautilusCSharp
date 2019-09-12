// -------------------------------------------------------------------------------------------------
// <copyright file="MsgPackCommandSerializer.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   https://nautechsystems.io
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Serialization
{
    using MsgPack;
    using Nautilus.Common.Interfaces;
    using Nautilus.Core.Correctness;
    using Nautilus.Core.Extensions;
    using Nautilus.Core.Message;
    using Nautilus.DomainModel.Commands;
    using Nautilus.DomainModel.Entities;
    using Nautilus.Serialization.Internal;

    /// <summary>
    /// Provides a <see cref="Command"/> message binary serializer for the MessagePack specification.
    /// </summary>
    public sealed class MsgPackCommandSerializer : IMessageSerializer<Command>
    {
        private readonly IdentifierCache identifierCache;
        private readonly OrderSerializer orderSerializer;

        /// <summary>
        /// Initializes a new instance of the <see cref="MsgPackCommandSerializer"/> class.
        /// </summary>
        public MsgPackCommandSerializer()
        {
            this.identifierCache = new IdentifierCache();
            this.orderSerializer = new OrderSerializer();
        }

        /// <inheritdoc />
        public byte[] Serialize(Command command)
        {
            var package = new MessagePackObjectDictionary
            {
                { nameof(Command.Type), command.Type.Name },
                { nameof(Command.Id), command.Id.ToString() },
                { nameof(Command.Timestamp), command.Timestamp.ToIsoString() },
            };

            switch (command)
            {
                case AccountInquiry cmd:
                    package.Add(nameof(cmd.AccountId), cmd.AccountId.Value);
                    break;
                case SubmitOrder cmd:
                    package.Add(nameof(cmd.TraderId), cmd.TraderId.Value);
                    package.Add(nameof(cmd.StrategyId), cmd.StrategyId.Value);
                    package.Add(nameof(cmd.AccountId), cmd.AccountId.Value);
                    package.Add(nameof(cmd.PositionId), cmd.PositionId.Value);
                    package.Add(nameof(cmd.Order), this.orderSerializer.Serialize(cmd.Order));
                    break;
                case SubmitAtomicOrder cmd:
                    package.Add(nameof(cmd.TraderId), cmd.TraderId.Value);
                    package.Add(nameof(cmd.StrategyId), cmd.StrategyId.Value);
                    package.Add(nameof(cmd.AccountId), cmd.AccountId.Value);
                    package.Add(nameof(cmd.PositionId), cmd.PositionId.Value);
                    package.Add(nameof(cmd.AtomicOrder.Entry), this.orderSerializer.Serialize(cmd.AtomicOrder.Entry));
                    package.Add(nameof(cmd.AtomicOrder.StopLoss), this.orderSerializer.Serialize(cmd.AtomicOrder.StopLoss));
                    package.Add(nameof(cmd.AtomicOrder.TakeProfit), this.orderSerializer.SerializeNullable(cmd.AtomicOrder.TakeProfit));
                    break;
                case ModifyOrder cmd:
                    package.Add(nameof(cmd.TraderId), cmd.TraderId.Value);
                    package.Add(nameof(cmd.AccountId), cmd.AccountId.Value);
                    package.Add(nameof(cmd.OrderId), cmd.OrderId.Value);
                    package.Add(nameof(cmd.ModifiedPrice), cmd.ModifiedPrice.ToString());
                    break;
                case CancelOrder cmd:
                    package.Add(nameof(cmd.TraderId), cmd.TraderId.Value);
                    package.Add(nameof(cmd.AccountId), cmd.AccountId.Value);
                    package.Add(nameof(cmd.OrderId), cmd.OrderId.Value);
                    package.Add(nameof(cmd.CancelReason), cmd.CancelReason);
                    break;
                default:
                    throw ExceptionFactory.InvalidSwitchArgument(command, nameof(command));
            }

            return MsgPackSerializer.Serialize(package);
        }

        /// <inheritdoc />
        public Command Deserialize(byte[] commandBytes)
        {
            var unpacked = MsgPackSerializer.Deserialize<MessagePackObjectDictionary>(commandBytes);

            var command = unpacked[nameof(Command.Type)].AsString();
            var id = ObjectExtractor.Guid(unpacked[nameof(Command.Id)]);
            var timestamp = unpacked[nameof(Command.Timestamp)].AsString().ToZonedDateTimeFromIso();

            switch (command)
            {
                case nameof(AccountInquiry):
                    return new AccountInquiry(
                        this.identifierCache.AccountId(unpacked),
                        id,
                        timestamp);
                case nameof(SubmitOrder):
                    return new SubmitOrder(
                        this.identifierCache.TraderId(unpacked),
                        this.identifierCache.AccountId(unpacked),
                        this.identifierCache.StrategyId(unpacked),
                        ObjectExtractor.PositionId(unpacked),
                        this.orderSerializer.Deserialize(unpacked[nameof(SubmitOrder.Order)].AsBinary()),
                        id,
                        timestamp);
                case nameof(SubmitAtomicOrder):
                    return new SubmitAtomicOrder(
                        this.identifierCache.TraderId(unpacked),
                        this.identifierCache.AccountId(unpacked),
                        this.identifierCache.StrategyId(unpacked),
                        ObjectExtractor.PositionId(unpacked),
                        new AtomicOrder(
                            this.orderSerializer.Deserialize(unpacked[nameof(AtomicOrder.Entry)].AsBinary()),
                            this.orderSerializer.Deserialize(unpacked[nameof(AtomicOrder.StopLoss)].AsBinary()),
                            this.orderSerializer.DeserializeNullable(unpacked[nameof(AtomicOrder.TakeProfit)].AsBinary())),
                        id,
                        timestamp);
                case nameof(ModifyOrder):
                    return new ModifyOrder(
                        this.identifierCache.TraderId(unpacked),
                        this.identifierCache.AccountId(unpacked),
                        ObjectExtractor.OrderId(unpacked),
                        ObjectExtractor.Price(unpacked[nameof(ModifyOrder.ModifiedPrice)].AsString()),
                        id,
                        timestamp);
                case nameof(CancelOrder):
                    return new CancelOrder(
                        this.identifierCache.TraderId(unpacked),
                        this.identifierCache.AccountId(unpacked),
                        ObjectExtractor.OrderId(unpacked),
                        unpacked[nameof(CancelOrder.CancelReason)].AsString(),
                        id,
                        timestamp);
                default:
                    throw ExceptionFactory.InvalidSwitchArgument(command, nameof(command));
            }
        }
    }
}
