using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MachineParts
{
    internal class MillComponent : IComponent
    {
        int jobId_ = -1;
        int cancel_ = 0;
        private readonly object lock_ = new object();
        string[] status_ = new string[] { "MillStarted", "JobStart", "JobFinish", "MillStopped" };
        private SensorStream sensorStream_ = new SensorStream();
        void SetCancelFlag(int value)
        {
            lock (lock_)
            {
                cancel_ = value;
                Console.WriteLine($"Flag set to: {value}");
            }
        }

        int GetCancelFlag()
        {
            lock (lock_)
            {
                return cancel_;
            }
        }
        public void StartStream()
        {
            sensorStream_.Start();
        }
        public void StopStream()
        {
            sensorStream_.Stop();
        }
        public MillComponent(string name, MainComponent mainComponent) : base(mainComponent, name)
        {
            sensorStream_.temperatureCallBck_ += ProcessTemperatureStream;
            mainComponent_?.AddComponent(name, message => ReceiveMessage(message));
        }

        public override void ReceiveMessage(Message message)
        {
            if (!Running) return;
            if (message.Header.SenderName == this.Name) // receiver match sender 
            {
                return;
            }

            string jsonHeader = JsonSerializer.Serialize(message);
            Console.WriteLine($"{jsonHeader} received by {Name} at {DateTime.Now.ToString()}");
            if (message.Body.content.Contains("Cancel")) // just an example, better ways to this
            {
                SetCancelFlag(1);
            }

        }

        public override void SendMessage(Message message)
        {
            if (!Running) return;
            string jsonString = JsonSerializer.Serialize(message);
            Console.WriteLine($"{jsonString} received by {Name} at {DateTime.Now.ToString()}");
            mainComponent_?.SendMessage(message);
        }
        public override void Start()
        {
            Running = true;
            Message.MsgHeader msgHeader = new Message.MsgHeader(DateTime.Now, Name, Guid.NewGuid(), false, Message.Type.EEvent);
            Message.MsgBody msgBody = new Message.MsgBody($"{Name} {status_[0]}");
            Message message = new Message(msgHeader, msgBody);
            SendMessage(message);
        }

        public override void Stop()
        {
            Running = false;
            Message.MsgHeader msgHeader = new Message.MsgHeader(DateTime.Now, Name, Guid.NewGuid(), false, Message.Type.EEvent);
            Message.MsgBody msgBody = new Message.MsgBody($"{Name} {status_[3]}");
            Message message = new Message(msgHeader, msgBody);
            SendMessage(message);
        }

        public string MillJob(int jobId)
        {
            if (!Running) return "Mill is not yet started";
            if (jobId <= 0) return "Invalid job id";
            if (jobId_ > 0) return "Other job is in progress";

            Task.Run(() =>
            {
                DateTime start = DateTime.Now;
                {
                    jobId_ = jobId;
                    Message.MsgHeader msgHeader = new Message.MsgHeader(DateTime.Now, Name, Guid.NewGuid());
                    Message.MsgBody msgBody = new Message.MsgBody($"{Name} {status_[1]} job {jobId_}");
                    Message message = new Message(msgHeader, msgBody);
                    SendMessage(message);
                }

                for (int i = 0; i < 10; i++)
                {
                    Thread.Sleep(1000);
                    if (GetCancelFlag() == 1)
                    {
                        DateTime end = DateTime.Now;
                        Message.MsgHeader msgHeader = new Message.MsgHeader(DateTime.Now, Name, Guid.NewGuid());
                        Message.MsgBody msgBody = new Message.MsgBody($"{Name} {status_[2]} {jobId} as cancelled");
                        Message message = new Message(msgHeader, msgBody);
                        DBJob job = new DBJob(jobId_, start, end, Name, "Cancelled");
                        mainComponent_?.AddJobToDb(job);
                        SendMessage(message);
                        break;
                    }
                }

                if (GetCancelFlag() == 0)
                {
                    DateTime end = DateTime.Now;
                    Message.MsgHeader msgHeader = new Message.MsgHeader(DateTime.Now, Name, Guid.NewGuid());
                    Message.MsgBody msgBody = new Message.MsgBody($"{Name} {status_[2]} job {jobId_}");
                    Message message = new Message(msgHeader, msgBody);
                    SendMessage(message);
                    DBJob job = new DBJob(jobId_, start, end, Name, "Complete");
                    mainComponent_?.AddJobToDb(job);
                }
                jobId_ = -1;
                SetCancelFlag(0);

            });
            return string.Empty;
        }

        public async Task<bool> Caliberate()
        {
            if (!Running) return false;
            bool calibDone = false;

            await Task.Run(() =>
            {
                Message.MsgHeader msgHeader = new Message.MsgHeader(DateTime.Now, Name, Guid.NewGuid());
                Message.MsgBody msgBody = new Message.MsgBody($"{Name} have been put under caliberation");
                Message message = new Message(msgHeader, msgBody);
                Task.Delay(2000).Wait();
                SendMessage(message);
                calibDone = true;
            });

            return calibDone;
        }

        public void ProcessTemperatureStream(string dataStream)
        {
            Task.Run(() =>
            {
                Console.WriteLine($" {Name} Processing {dataStream}");

            });
        }

    }
}
