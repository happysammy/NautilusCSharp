namespace Nautilus.ProtoBuf.Messages
{
    using System.Runtime.Serialization;
    using global::ProtoBuf;

    [ProtoContract]
    public class Tick
    {
        [DataMember(Order=1)]
        public string Symbol { get; set; }

        [DataMember(Order=2)]
        public decimal Bid { get; set; }

        [DataMember(Order=3)]
        public decimal Ask { get; set; }

        [DataMember(Order=4)]
        public string Timestamp { get; set; }
    }
}
