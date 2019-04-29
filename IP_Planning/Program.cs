﻿using Google.Apis.Calendar.v3.Data;
using IP_Planning_GoogleAPI;
using IP_Planning_Database;
using System;
using System.Collections.Generic;
using MessageBroker;

namespace IP_Planning
{
    class Program
    {
        static void Main(string[] args)
        {
            Log log = Log.Instance;
            log.Welcome();
            log.ShowDebugMessages(true);
            log.LogMessage("Hello world!", "debug");
            log.LogMessage("Hello world!", "info");
            log.LogMessage("Hello world!", "warning");
            log.LogMessage("Hello world!", "error");

            Connection connection = Connection.Instance;

            IMessageHandler messageHandler = new MessageHandler();

            connection.OpenConnection("amqFacturatie", "amqFacturatie", "10.3.56.10", "Facturatie", messageHandler);

            Publisher publisher = Publisher.Instance;
            //GoogleAPI googleAPI = new GoogleAPI();

            //var kal = googleAPI.createCalendar("testNiewKalender"); WERKT
            //googleAPI.createEvent(); //WERKT
            //googleAPI.deleteEvent("99710u3s357dfjev9eb9ah797k_20190326T180123Z"); //WERKT
            //googleAPI.deleteCalendar("aggrpeemmreezrd@gmail.com"); WERKT
            //googleAPI.updateEvent("dit is een upgedate description lol", "aggrpeemmreezrd@gmail.com", "btsrhr4coft927041nlucsro70"); //WERKT
            //googleAPI.updateCalendar("dit is een upgedate description", "aggrpeemmreezrd@gmail.com"); //WERKT
            //googleAPI.getEvents("aggrpeemmreezrd@gmail.com"); //WERKT
            //googleAPI.getCalendars("aggrpeemmreezrd@gmail.com"); //WERKT


            //DBConnect db = new DBConnect();

            //db.insertWerknemer("3354DGDHDHTEST", "82736464TEST"); //WERKT
            //db.deleteWerknemer("3354DGDHDHTEST"); // WERKT
            //db.insertTaak("lol", "lol", "lolilol"); //WERKT
            //db.deleteTaak("lol"); //WERKT
            //db.insertLocatie("locUUID", "calUUID"); //WERKT
            //db.deleteLocatie("locUUID"); //WERKT
            //db.insertSessie("sesid", "bla", "bla"); //WERKT
            //db.deleteSessie("sesid"); //WERKT
            //db.insertEvent("evenID", "eve"); //WERKT
            //db.deleteEvent("evenID"); //WERKT

            Console.ReadKey();        

            ////Change this to your credentials.
            ////Check https://docs.google.com/document/d/1juDXoeJSQxVjRHMO8k0yPMVFv3-7bJfH0QSp0yeEedQ/edit?usp=sharing

            //Broker broker = new Broker("amqPlanning", "amqPlanning", "10.3.56.10", "Planning", 10000);

            //while (!broker.IsConnected())
            //{
            //    broker.OpenConnection();

            //    if (!broker.IsConnected())
            //    {
            //        Console.Write("INFO: Retrying in 10s.");
            //        Thread.Sleep(3333);
            //        Console.Write(".");
            //        Thread.Sleep(3333);
            //        Console.Write(".");
            //        Thread.Sleep(3333);
            //        Console.Write("\n");
            //    }
            //}

            //string message;

            ////Remove this code after testing
            //Console.WriteLine("\nWelcome to hello server. This is only for testing purposes.");

            //bool badInput = true;
            //int maxcount = 0;
            //int interval = 1000;

            //while (badInput)
            //{
            //    Console.WriteLine("Please enter the amount of messages you would like to send:");
            //    string input = Console.ReadLine();
            //    if(Int32.TryParse(input, out maxcount))
            //    {
            //        badInput = false;
            //    }
            //    else
            //    {
            //        Console.WriteLine("Bad input.");
            //    }
            //}

            //badInput = true;

            //while (badInput)
            //{
            //    Console.WriteLine("Please enter the interval between sending the messages in ms:");
            //    string input = Console.ReadLine();
            //    if (Int32.TryParse(input, out interval))
            //    {
            //        badInput = false;
            //    }
            //    else
            //    {
            //        Console.WriteLine("Bad input.");
            //    }
            //}

            //int counter = 1;
            //bool sending = true;

            //while (sending)
            //{
            //    message = "Message nr " + counter;
            //    broker.NewMessage(message);
            //    counter++;
            //    Thread.Sleep(interval);
            //    if (counter > maxcount)
            //        sending = false;
            //}
            //Thread.Sleep(1000);
            //broker.CloseConnection();
            //Console.WriteLine("Press enter to quit.");
            //Console.ReadLine();
            //----
        }
    }
}

