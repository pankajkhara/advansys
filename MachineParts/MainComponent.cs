using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MachineParts
{
    internal class MainComponent
    {
        public MainComponent(string name)
        {
            Name = name;
        }
        public string Name { get; }
        private ConcurrentDictionary<string, Action<Message>> Components { get; } = new ConcurrentDictionary<string, Action<Message>>();
        private ConcurrentQueue<Message> MessagesNormal { get; } = new ConcurrentQueue<Message>();
        private ConcurrentQueue<Message> MessagesPriority { get; } = new ConcurrentQueue<Message>();
        private static readonly object lockObject = new object();
        IConfigurationRoot configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build();

        public bool AddComponent(string name , Action<Message> handleMessage)
        {
            bool add = false;
            if (!Components.ContainsKey(name))
            {
                Components.TryAdd(name, handleMessage);
                add = true;
            }
            return add;
        }

        public bool RemoveComponent(string name)
        {
            bool remove = false;
            if (Components.ContainsKey(name))
            {
                var kvp = new KeyValuePair<string, Action<Message>>(name, Components[name]);
                Components.TryRemove(kvp);
                remove = true;
            }
            return remove;
        }

        public void ReceiveMessage(Message message)
        {
            string jsonHeader = JsonSerializer.Serialize(message);
            if (message.Header.SenderName == this.Name)
            {
                Console.WriteLine($"{jsonHeader} acknowledgement recieved from my message {DateTime.Now.ToString()}");
                return;
            }

     
            Console.WriteLine($"{jsonHeader} received by main component at { DateTime.Now.ToString() }");
            if(message.Header.HighPriority)
                MessagesPriority.Enqueue(message);
            else
                MessagesNormal.Enqueue(message);
        }

        public void SendMessage(Message message)
        {
            if (Components.Count > 0)
            { 
                foreach (var handler in Components.Values)
                {
                    handler(message);
                }
            }        
            else
            {
                string jsonHeader = JsonSerializer.Serialize(message);
                Console.WriteLine($"No receivers for message: {jsonHeader}");
            }

        }
        public void CancelJob()
        {
            Message.MsgHeader msgHeader = new Message.MsgHeader(DateTime.Now, Name, Guid.NewGuid(),true, Message.Type.ECommnad,true);
            Message.MsgBody msgBody = new Message.MsgBody($"Cancel");
            Message message = new Message(msgHeader, msgBody);
            MessagesPriority.Enqueue(message);
        }

        public void Init()
        {
            Task.Run(() =>
            {
                while (true)
                {
                    while (!MessagesPriority.IsEmpty)
                    {
                        if (MessagesPriority.TryDequeue(out Message messagePr))
                        {
                            SendMessage(messagePr);
                        }
                    }
                    if (MessagesNormal.TryDequeue(out Message messageNr))
                    {
                        SendMessage(messageNr);
                    }
                }

            });
        }

        public void AddJobToDb(DBJob job)
        {
            try
            {
                lock (lockObject)
                {
                    using (var context = new ApplicationDbContext())
                    {
                        context.Database.EnsureCreated();
                        JobRepo repo = new JobRepo(context);
                        repo.Add(job);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

    }
}
