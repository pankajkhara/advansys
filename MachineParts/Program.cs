// See https://aka.ms/new-console-template for more information
using MachineParts;
using Microsoft.Extensions.Configuration;

namespace MachineParts
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                File.Delete("localdatabase.db"); // destroying every time to avoid
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.ToString());
            }
            
            var configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build();


            List<MillComponent> components = new List<MillComponent>();
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
                    mill.StartStream(); // test stream
                    components.Add(mill);
                    mill.Start(); // test Event
                    await mill.Caliberate(); // test Message
                    mill.MillJob(jobNumber++);
                    mill.Stop(); // test Event

                }

                MillComponent mill1 = new MillComponent("Mill" + i.ToString(), mainComponent);
                mill1.StartStream(); // test stream
                components.Add(mill1);
                mill1.Start();// Event
                mill1.MillJob(jobNumber++);
                mainComponent.CancelJob(); // test of prioritization and round trip command feedback
               

                //test stop stream and cleanup code
                //foreach (var mill in components)
                //{
                //    mill.StopStream();
                //    mainComponent.RemoveComponent(mill.Name);
                //}
            }
            else
            {
                Console.WriteLine("Check NumMills entry in appsettings.json");
            }
            Console.ReadKey();
        }
    }
}