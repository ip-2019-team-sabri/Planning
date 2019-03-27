using System;
using System.Threading;
using IP_Planning_Broker;
using IP_Planning_Logger;

namespace IP_Planning
{
    class Program
    {
        static void Main(string[] args)
        {
            //Change this to your credentials.
            //Check https://docs.google.com/document/d/1juDXoeJSQxVjRHMO8k0yPMVFv3-7bJfH0QSp0yeEedQ/edit?usp=sharing

            Broker broker = new Broker("amqPlanning", "amqPlanning", "10.3.56.10", "Planning", 10000);

            Logger logger = Logger.Instance;
            logger.Welcome();

            while (!broker.IsConnected())
            {
                broker.OpenConnection();

                if (!broker.IsConnected())
                {
                    logger.Log("Retrying in 10s","info");
                }
            }

            string message;

            //Remove this code after testing
            Console.WriteLine("\nWelcome to hello server. This is only for testing purposes.");

            bool badInput = true;
            int maxcount = 0;
            int interval = 1000;

            while (badInput)
            {
                Console.WriteLine("Please enter the amount of messages you would like to send:");
                string input = Console.ReadLine();
                if(Int32.TryParse(input, out maxcount))
                {
                    badInput = false;
                }
                else
                {
                    logger.Log("Bad input.","error");
                }
            }

            badInput = true;

            while (badInput)
            {
                logger.Log("Please enter the interval between sending the messages in ms:","info");
                string input = Console.ReadLine();
                if (Int32.TryParse(input, out interval))
                {
                    badInput = false;
                }
                else
                {
                    Console.WriteLine("Bad input.");
                }
            }

            int counter = 1;
            bool sending = true;

            while (sending)
            {
                message = "Message nr " + counter;
                broker.NewMessage(message);
                counter++;
                Thread.Sleep(interval);
                if (counter > maxcount)
                    sending = false;
            }


            Thread.Sleep(1000);
            broker.CloseConnection();
            Console.WriteLine("Press enter to quit.");
            Console.ReadLine();
            //----
        }
    }
}

