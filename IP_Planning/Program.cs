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

            Broker broker = new Broker("amqPlanning", "amqPlanning", "10.3.56.10", "Planning");

            broker.StartConsumer();


            string message;

            //Remove this code after testing
            Console.WriteLine("Welcome to hello server. This is only for testing purposes.");

            while (true)
            {
                Console.WriteLine("Enter a message:");
                message = Console.ReadLine();
                broker.SendMessage(message);

                Thread.Sleep(250);
            }
            broker.CloseConnection();

            Console.WriteLine("Press enter to quit.");
            Console.ReadLine();
            //----
        }
    }
}

