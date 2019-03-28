using System;
using System.IO;
using System.Threading;
using System.Xml.Serialization;
using IP_Planning_Broker;
using IP_Planning_Broker.Messages;
using IP_Planning_Logger;

namespace IP_Planning
{
    class Program
    {
        static void Main(string[] args)
        {

//########## Set up the broker ##########

            //Change this to your credentials.
            //Check https://docs.google.com/document/d/1juDXoeJSQxVjRHMO8k0yPMVFv3-7bJfH0QSp0yeEedQ/edit?usp=sharing

            Broker broker = new Broker("amqPlanning", "amqPlanning", "10.3.56.10", "Planning");

            //Check the "Logs" directory for the logfiles if you encounter any problems
            Logger logger = Logger.Instance;
            logger.Welcome();

            broker.OpenConnection();

//########## Example of sending a message ##########

            //Create a new message object
            PingMessage ping = new PingMessage
            {
                header = new PingMessageHeader
                {
                    timestamp = DateTime.Now,
                    versie = "1"
                },
                body = new PingMessageBody
                {
                    //To create a new GUID use this ONE LINE of code INSTEAD OF A CENTRALIZED STUPID DATABASE
                    pingUUID = Guid.NewGuid().ToString()
                }
            };

            //Serialize the object and convert it to a string
            XmlSerializer mySerializer = new XmlSerializer(typeof(PingMessage));
            StringWriter writer = new StringWriter();
            mySerializer.Serialize(writer, ping);
            string message = writer.ToString();

            //Send the message to the broker
            broker.NewMessage(message);

            
            Console.WriteLine("Press enter to quit.");
            Console.ReadLine();

            broker.CloseConnection();
            //----
        }
    }
}

