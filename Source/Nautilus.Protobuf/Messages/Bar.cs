namespace Nautilus.ProtoBuf.Messages
{
    using System.Runtime.Serialization;
    using global::ProtoBuf;

    [ProtoContract]
    public class Bar
    {
        [DataMember(Order=1)]
        public string Symbol { get; set; }

        [DataMember(Order=2)]
        public decimal Open { get; set; }

        [DataMember(Order=3)]
        public decimal High { get; set; }

        [DataMember(Order=4)]
        public decimal Low { get; set; }

        [DataMember(Order=5)]
        public decimal Close { get; set; }

        [DataMember(Order=6)]
        public long Volume { get; set; }

        [DataMember(Order=7)]
        public string Timestamp { get; set; }
    }
}
