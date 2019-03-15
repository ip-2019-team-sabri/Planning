using System;
using System.Threading;
using IP_Planning_Broker;

namespace IP_Planning
{
    class Program
    {
        static void Main(string[] args)
        {
            //Change this to your credentials.
            //Check https://docs.google.com/document/d/1juDXoeJSQxVjRHMO8k0yPMVFv3-7bJfH0QSp0yeEedQ/edit?usp=sharing

            Broker broker = new Broker("amqPlanning", "amqPlanning", "10.3.56.10", "Planning", 10000);

            while (!broker.IsConnected())
            {
                broker.OpenConnection();

                if (!broker.IsConnected())
                {
                    Console.Write("INFO: Retrying in 10s.");
                    Thread.Sleep(3333);
                    Console.Write(".");
                    Thread.Sleep(3333);
                    Console.Write(".");
                    Thread.Sleep(3333);
                    Console.Write("\n");
                }
            }

            string message;

            //Remove this code after testing
            Console.WriteLine("\nWelcome to hello server. This is only for testing purposes.");

            int counter = 1;
            int maxcount = 1000;
            bool sending = true;

            while (sending)
            {
                message = "Message nr " + counter;
                broker.NewMessage(message);
                counter++;
                Thread.Sleep(100);
                if (counter > maxcount)
                    sending = false;
            }

            Console.WriteLine("Press enter to quit.");
            Console.ReadLine();
            //----
        }
    }
}

