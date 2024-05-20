// See https://aka.ms/new-console-template for more information
using MachineParts;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;

namespace MachineParts
{
    public class Program
    {
        
        static async Task Main(string[] args)
        {
            var configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build();

            // test encrption decrption so far not integrate with message structure 
            string originalText = "a secret message!";
            string? key = configuration["EncryptKey"];

            if (!string.IsNullOrEmpty(key))
            {

                // Generate a secure random key for AES-128
                using (Aes aes = Aes.Create())
                {
                    string encryptedText = EncryptService.Encrypt(key, originalText);
                    Console.WriteLine("Encrypted Text: " + encryptedText);

                    string decryptedText = EncryptService.Decrypt(key, encryptedText);
                    Console.WriteLine("Decrypted Text: " + decryptedText);
                }
            }

            try
            {
                File.Delete("localdatabase.db"); // destroying every time to avoid
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.ToString());
            }

            List<MillComponent> components = new List<MillComponent>();
            int jobNumber = 10000;
            string? numMillsStr = configuration["NumMills"];// no of mills can be configured via this setting
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