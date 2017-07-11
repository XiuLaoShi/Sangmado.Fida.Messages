using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Serialization;
using ProtoBuf;

namespace Sangmado.Fida.Messages
{
    [ProtoContract(SkipConstructor = false, UseProtoMembersOnly = true)]
    [Serializable]
    [XmlType(TypeName = "Message")]
    public class MessageEnvelope
    {
        #region Constructors

        private static readonly DateTime _unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

        public MessageEnvelope()
        {
            this.MessageID = Guid.NewGuid().ToString();
            this.CreatedTime = DateTime.UtcNow;
        }

        #endregion

        #region Header

        [ProtoMember(1)]
        public string MessageID { get; set; }
        [ProtoMember(2)]
        public string MessageType { get; set; }
        [ProtoMember(5, Name = @"CreatedTime")]
        public long SerializedCreatedTime { get; set; }
        [ProtoIgnore]
        public DateTime CreatedTime
        {
            get
            {
                return _unixEpoch.AddTicks(this.SerializedCreatedTime);
            }
            set
            {
                this.SerializedCreatedTime = value.Subtract(_unixEpoch).Ticks;
            }
        }

        [ProtoMember(10)]
        public string CorrelationID { get; set; }
        [ProtoMember(11, Name = @"CorrelationTime")]
        public long SerializedCorrelationTime { get; set; }
        [ProtoIgnore]
        public DateTime CorrelationTime
        {
            get
            {
                return _unixEpoch.AddTicks(this.SerializedCorrelationTime);
            }
            set
            {
                this.SerializedCorrelationTime = value.Subtract(_unixEpoch).Ticks;
            }
        }

        [ProtoMember(30)]
        public Endpoint Source { get; set; }
        [ProtoMember(31)]
        public Endpoint Target { get; set; }

        #endregion

        #region Payload

        [ProtoMember(80)]
        public byte[] Body { get; set; }

        #endregion

        #region Methods

        public MessageEnvelope<T> ConvertTo<T>()
        {
            var envelope = new MessageEnvelope<T>();
            envelope.CopyFrom(this);
            return envelope;
        }

        public static MessageEnvelope NewFrom<T>(MessageEnvelope<T> source)
        {
            var envelope = new MessageEnvelope();
            envelope.CopyFrom(source);
            return envelope;
        }

        public void CopyFrom<T>(MessageEnvelope<T> source)
        {
            this.MessageID = source.MessageID;
            this.CreatedTime = source.CreatedTime;

            this.MessageType = source.MessageType;
            this.CorrelationID = source.CorrelationID;
            this.CorrelationTime = source.CorrelationTime;

            this.Source = source.Source == null ? null : source.Source.Clone();
            this.Target = source.Target == null ? null : source.Target.Clone();
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture,
                "MessageType[{0}], Source[{1}], Target[{2}], BodySize[{3}], "
                + "MessageID[{4}], CorrelationID[{5}], CorrelationTime[{6}], CreatedTime[{7}]",
                this.MessageType,
                this.Source,
                this.Target,
                Body == null ? 0 : Body.Length,
                this.MessageID,
                this.CorrelationID,
                this.CorrelationTime.ToString(@"yyyy-MM-dd HH:mm:ss.fffffff"),
                this.CreatedTime.ToString(@"yyyy-MM-dd HH:mm:ss.fffffff"));
        }

        #endregion
    }

    [Serializable]
    [XmlType(TypeName = "Message")]
    public sealed class MessageEnvelope<T>
    {
        #region Constructors

        private static readonly DateTime _unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

        public MessageEnvelope()
        {
            this.MessageID = Guid.NewGuid().ToString();
            this.CreatedTime = DateTime.UtcNow;
            this.MessageType = typeof(T).Name;
        }

        #endregion

        #region Header

        public string MessageID { get; set; }
        public string MessageType { get; set; }
        public DateTime CreatedTime { get; set; }

        public string CorrelationID { get; set; }
        public DateTime CorrelationTime { get; set; }

        public Endpoint Source { get; set; }
        public Endpoint Target { get; set; }

        #endregion

        #region Payload

        [XmlIgnore]
        public T Message { get; set; }

        #endregion

        #region Methods

        public MessageEnvelope ConvertToNonGeneric()
        {
            var envelope = new MessageEnvelope();
            envelope.CopyFrom(this);
            return envelope;
        }

        public static MessageEnvelope<T> NewFrom(MessageEnvelope source)
        {
            var envelope = new MessageEnvelope<T>();
            envelope.CopyFrom(source);
            return envelope;
        }

        public void CopyFrom(MessageEnvelope source)
        {
            this.MessageID = source.MessageID;
            this.CreatedTime = source.CreatedTime;

            this.MessageType = source.MessageType;
            this.CorrelationID = source.CorrelationID;
            this.CorrelationTime = source.CorrelationTime;

            this.Source = source.Source == null ? null : source.Source.Clone();
            this.Target = source.Target == null ? null : source.Target.Clone();
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture,
                "MessageType[{0}], Source[{1}], Target[{2}], "
                + "MessageID[{3}], CorrelationID[{4}], CorrelationTime[{5}], CreatedTime[{6}]",
                this.MessageType,
                this.Source,
                this.Target,
                this.MessageID,
                this.CorrelationID,
                this.CorrelationTime.ToString(@"yyyy-MM-dd HH:mm:ss.fffffff"),
                this.CreatedTime.ToString(@"yyyy-MM-dd HH:mm:ss.fffffff"));
        }

        #endregion
    }
}
