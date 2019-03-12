using System;
using IP_Planning_Broker;

namespace IP_Planning
{
    class Program
    {
        static void Main(string[] args)
        {
            Broker broker = new Broker("amqPlanning","amqPlanning","10.3.56.10","Planning");

            broker.StartConsumer();

            string message;

            while (true)
            {
                message =  Console.ReadLine();
                broker.SendMessage(message);
            }
        }
    }
}
