using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MachineParts
{
    internal abstract class IComponent
    {
        protected MainComponent? mainComponent_;
        public string Name { get; }
        public bool Running { get; set; } = false;
        public abstract void Start();
        public abstract void Stop();
        public abstract void SendMessage(Message Message);
        public abstract void ReceiveMessage(Message Message);
        protected IComponent(MainComponent mainComponent_, string name)
        {
            this.mainComponent_ = mainComponent_;
            Name = name;
        }
    }
}
