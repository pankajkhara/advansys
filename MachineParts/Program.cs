// See https://aka.ms/new-console-template for more information
using MachineParts;
using Microsoft.Extensions.Configuration;

namespace MachineParts
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            int jobNumber = 10000;
            string? numMillsStr = configuration["NumMills"];
            if (!string.IsNullOrEmpty(numMillsStr))
            {
                var numMills = int.Parse(numMillsStr);
                MainComponent mainComponent = new MainComponent("main");

                mainComponent.Init();
                int i = 0;
                for (; i < numMills - 1; i++)
                {
                    MillComponent mill = new MillComponent("Mill" + i.ToString(), mainComponent);
                    mill.Start();
                    await mill.Caliberate();
                    mill.MillJob(jobNumber++);
                    mill.Stop();
                }

                MillComponent mill1 = new MillComponent("Mill" + i.ToString(), mainComponent);
                mill1.Start();
                mill1.MillJob(jobNumber++);
                mainComponent.CancelJob();
                mill1.Stop();
            }
            else
            {
                Console.WriteLine("Check NumMills entry in appsettings.json");
            }
            Console.ReadKey();
        }
    }
}