﻿using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IP_Planning_Broker
{
    public class Broker
    {
        public bool connected;

        Thread t;
        List<string> messageQueue;

        private IConnection connection;
        private IModel consumerChannel;
        private IModel publisherChannel;

        private string userName;
        private string password;
        private string hostName;
        private string queueName;

        public Broker(string userName, string password, string hostName, string queueName)
        {
            connected = false;

            this.userName = userName;
            this.password = password;
            this.hostName = hostName;
            this.queueName = queueName;

            t = new Thread(new ThreadStart(heartbeat));
            t.Start();

            messageQueue = new List<string>();
        }

        public void OpenConnection()
        {
            ConnectionFactory factory = new ConnectionFactory()
            {
                UserName = userName,
                Password = password,
                HostName = hostName
            };

            Console.WriteLine("INFO: Connecting to RabbitMQ server...");

            try
            {
                connected = true;

                connection = factory.CreateConnection();
                consumerChannel = connection.CreateModel();
                publisherChannel = connection.CreateModel();

                var consumer = new EventingBasicConsumer(consumerChannel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body);
                    Console.WriteLine("Received message: {0}", message);
                };

                consumerChannel.BasicConsume(queue: queueName,
                                     autoAck: true,
                                     consumer: consumer);
            }
            catch (BrokerUnreachableException e)
            {
                Console.WriteLine("ERROR: Failed to open connection. " + e.Message + ".");
                CloseConnection();
            }
            catch (OperationInterruptedException e)
            {
                Console.WriteLine("ERROR: Failed to open connection. " + e.Message);
                CloseConnection();
            }

            //foreach(var msg in messageBuffer)
            //{

            //}
        }

        public void CloseConnection()
        {
            if (consumerChannel != null)
            {
                consumerChannel.Close();
                consumerChannel.Dispose();
                consumerChannel = null;
            }

            if (publisherChannel != null)
            {
                publisherChannel.Close();
                publisherChannel.Dispose();
                publisherChannel = null;
            }

            if (connection != null)
            {
                connection.Close();
                connection.Dispose();
                connection = null;
            }
            connected = false;
        }

        public void QueueMessage(string msg)
        {
            messageQueue.Add(msg);
        }

        private void heartbeat()
        {
            while (true)
            {
                SendMessages();
                Thread.Sleep(1000);
            }
        }

        private void SendMessages()
        {
            bool issent = true;

            while(messageQueue.Count > 0 && issent)
            {
                issent = SendMessage(messageQueue[0]);
                if(issent)
                {
                    messageQueue.RemoveAt(0);
                }
            }
        }

        private bool SendMessage(string msg)
        {
            try
            {
                if (publisherChannel != null)
                {
                    var body = Encoding.UTF8.GetBytes(msg);

                    var properties = publisherChannel.CreateBasicProperties();
                    properties.Persistent = true;

                    publisherChannel.BasicPublish(exchange: "amq.fanout",
                                         routingKey: "",
                                         basicProperties: properties,
                                         body: body);
                    return true;
                }
                else
                {
                    Console.WriteLine("ERROR: Trying to publish message while channel is not initialized.");
                    return false;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR: Failed to send message. " + e.Message);
                return false;
            }
        }

        //private async Task BufferMessage(string msg)
        //{
        //    bufferCount++;

        //    bool isSent = false;
        //    Console.WriteLine("Buffering new message. Currently there are " + bufferCount + " messages buffered.");

        //    while (!isSent)
        //    {
        //        if (publisherChannel != null)
        //        {
        //            var body = Encoding.UTF8.GetBytes(msg);

        //            var properties = publisherChannel.CreateBasicProperties();
        //            properties.Persistent = true;

        //            publisherChannel.BasicPublish(exchange: "amq.fanout",
        //                                 routingKey: "",
        //                                 basicProperties: properties,
        //                                 body: body);
        //        }
        //    }
        //    Console.WriteLine("Buffered message sent.");
        //}

        //public async Task SendMessage(string msg)
        //{
        //    try
        //    {
        //        if (publisherChannel != null)
        //        {
        //            var body = Encoding.UTF8.GetBytes(msg);

        //            var properties = publisherChannel.CreateBasicProperties();
        //            properties.Persistent = true;

        //            publisherChannel.BasicPublish(exchange: "amq.fanout",
        //                                 routingKey: "",
        //                                 basicProperties: properties,
        //                                 body: body);
        //        }
        //        else
        //        {
        //            Console.WriteLine("ERROR: Trying to publish message while channel is not initialized.");
        //            BufferMessage(msg);
        //        }
        //    }
        //    catch(Exception e)
        //    {
        //        Console.WriteLine("ERROR: Failed to send message. " + e.Message);
        //        BufferMessage(msg);
        //    }
        //}
    }
}
