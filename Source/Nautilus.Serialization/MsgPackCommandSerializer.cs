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
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Interfaces;
    using Nautilus.Core.Correctness;
    using Nautilus.Core.Extensions;
    using Nautilus.Core.Message;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.Execution.Messages.Commands;
    using Nautilus.Serialization.Internal;

    /// <summary>
    /// Provides a <see cref="Command"/> message binary serializer for the MessagePack specification.
    /// </summary>
    public sealed class MsgPackCommandSerializer : IMessageSerializer<Command>
    {
        private readonly ObjectCache<string, TraderId> cachedTraderIds;
        private readonly ObjectCache<string, AccountId> cachedAccountIds;
        private readonly ObjectCache<string, StrategyId> cachedStrategyIds;

        /// <summary>
        /// Initializes a new instance of the <see cref="MsgPackCommandSerializer"/> class.
        /// </summary>
        public MsgPackCommandSerializer()
        {
            this.cachedTraderIds = new ObjectCache<string, TraderId>(TraderId.FromString);
            this.cachedAccountIds = new ObjectCache<string, AccountId>(AccountId.FromString);
            this.cachedStrategyIds = new ObjectCache<string, StrategyId>(StrategyId.FromString);
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
                    package.Add(nameof(cmd.Order), OrderSerializer.Serialize(cmd.Order));
                    break;
                case SubmitAtomicOrder cmd:
                    package.Add(nameof(cmd.TraderId), cmd.TraderId.Value);
                    package.Add(nameof(cmd.StrategyId), cmd.StrategyId.Value);
                    package.Add(nameof(cmd.AccountId), cmd.AccountId.Value);
                    package.Add(nameof(cmd.PositionId), cmd.PositionId.Value);
                    package.Add(nameof(cmd.AtomicOrder.Entry), OrderSerializer.Serialize(cmd.AtomicOrder.Entry));
                    package.Add(nameof(cmd.AtomicOrder.StopLoss), OrderSerializer.Serialize(cmd.AtomicOrder.StopLoss));
                    package.Add(nameof(cmd.AtomicOrder.TakeProfit), OrderSerializer.SerializeNullable(cmd.AtomicOrder.TakeProfit));
                    break;
                case ModifyOrder cmd:
                    package.Add(nameof(cmd.TraderId), cmd.TraderId.Value);
                    package.Add(nameof(cmd.StrategyId), cmd.StrategyId.Value);
                    package.Add(nameof(cmd.AccountId), cmd.AccountId.Value);
                    package.Add(nameof(cmd.OrderId), cmd.OrderId.Value);
                    package.Add(nameof(cmd.ModifiedPrice), cmd.ModifiedPrice.ToString());
                    break;
                case CancelOrder cmd:
                    package.Add(nameof(cmd.TraderId), cmd.TraderId.Value);
                    package.Add(nameof(cmd.StrategyId), cmd.StrategyId.Value);
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
                        ObjectExtractor.AccountId(unpacked),
                        id,
                        timestamp);
                case nameof(SubmitOrder):
                    return new SubmitOrder(
                        ObjectExtractor.TraderId(unpacked),
                        ObjectExtractor.StrategyId(unpacked),
                        ObjectExtractor.AccountId(unpacked),
                        ObjectExtractor.PositionId(unpacked),
                        OrderSerializer.Deserialize(unpacked[nameof(SubmitOrder.Order)].AsBinary()),
                        id,
                        timestamp);
                case nameof(SubmitAtomicOrder):
                    return new SubmitAtomicOrder(
                        ObjectExtractor.TraderId(unpacked),
                        ObjectExtractor.StrategyId(unpacked),
                        ObjectExtractor.AccountId(unpacked),
                        ObjectExtractor.PositionId(unpacked),
                        new AtomicOrder(
                            OrderSerializer.Deserialize(unpacked[nameof(AtomicOrder.Entry)].AsBinary()),
                            OrderSerializer.Deserialize(unpacked[nameof(AtomicOrder.StopLoss)].AsBinary()),
                            OrderSerializer.DeserializeNullable(unpacked[nameof(AtomicOrder.TakeProfit)].AsBinary())),
                        id,
                        timestamp);
                case nameof(ModifyOrder):
                    return new ModifyOrder(
                        ObjectExtractor.TraderId(unpacked),
                        ObjectExtractor.StrategyId(unpacked),
                        ObjectExtractor.AccountId(unpacked),
                        ObjectExtractor.OrderId(unpacked),
                        ObjectExtractor.Price(unpacked[nameof(ModifyOrder.ModifiedPrice)].AsString()),
                        id,
                        timestamp);
                case nameof(CancelOrder):
                    return new CancelOrder(
                        ObjectExtractor.TraderId(unpacked),
                        ObjectExtractor.StrategyId(unpacked),
                        ObjectExtractor.AccountId(unpacked),
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
