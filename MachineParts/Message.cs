using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MachineParts
{
    public record Message
    {
        public enum Type
        {
            EMessage,
            EEvent,
            ECommnad
        }

        public record MsgHeader(
        DateTime DateTimeStamp,
        string SenderName,
        Guid ID,
        bool HighPriority = false,
        Type MessageType = Type.EMessage,
        bool NeedFeedback = false,
        bool Encode = true);

        public record MsgBody( string content);

        public MsgHeader Header { get;}
        public MsgBody Body { get; }

        public Message(string sender, string content)
        {
            if (sender == null) throw new ArgumentNullException();
            if(string.IsNullOrEmpty(content)) throw new ArgumentNullException();

            Header = new MsgHeader(DateTime.Now,sender,Guid.NewGuid());
            Body = new MsgBody(content);
         
        }

        public Message(MsgHeader header, MsgBody body)
        {
            Header = header ?? throw new ArgumentNullException(nameof(header));
            Body = body ?? throw new ArgumentNullException(nameof(body));
        }
    }
}
