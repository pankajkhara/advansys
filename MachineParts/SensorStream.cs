using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MachineParts
{
    public class SensorStream
    {
        private Timer? timer_; // used timer to simulate stream
        public event Action<string>? temperatureCallBck_;
        double temeperature = 25;
        public void Start()
        {
            timer_ = new Timer(Generate, null, 0, 1000);
        }
        public void Stop()
        {
            timer_?.Change(Timeout.Infinite, 0);
            timer_?.Dispose();
        }

        private void Generate(object state)
        {
            temeperature++;
            if (temeperature > 60)
                temeperature = 25;
            var temp = $" {temeperature} Temperature (celsisus)  at {DateTime.Now}";
            temperatureCallBck_?.Invoke(temp);
        }
    }
}
