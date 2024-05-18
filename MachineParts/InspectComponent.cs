using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MachineParts
{
    internal class InspectComponent : IComponent
    {
        public InspectComponent(string name, MainComponent mainComponent) : base(mainComponent, name)
        {
            mainComponent_?.AddComponent(name, message => ReceiveMessage(message));
        }

        public override void ReceiveMessage(Message message)
        {
            if (!Running) return;
            if (!message.Header.NeedFeedback && message.Header.SenderName == this.Name) return;

            string jsonHeader = JsonSerializer.Serialize(message);
            Console.WriteLine($"{jsonHeader} received by {Name} at {DateTime.Now.ToString()}");
        }

        public override void SendMessage(Message message)
        {
            if (!Running) return;
            string jsonHeader = JsonSerializer.Serialize(message);
            Console.WriteLine($"{jsonHeader} received by {Name} at {DateTime.Now.ToString()}");
            mainComponent_?.SendMessage(message);
        }
        public override void Start()
        {
            Running = true;
            Message.MsgHeader msgHeader = new Message.MsgHeader(DateTime.Now, Name, Guid.NewGuid());
            Message.MsgBody msgBody = new Message.MsgBody($"{Name} have started processing");
            Message message = new Message(msgHeader, msgBody);
            SendMessage(message);
        }

        public override void Stop()
        {
            Running = false;
            Message.MsgHeader msgHeader = new Message.MsgHeader(DateTime.Now, Name, Guid.NewGuid());
            Message.MsgBody msgBody = new Message.MsgBody($"{Name} have stopped processing");
            Message message = new Message(msgHeader, msgBody);
            SendMessage(message);
        }
    }
}
