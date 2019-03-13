using RabbitMQ.Client;
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


        private Timer aTimer;
        private AutoResetEvent autoEvent;
        private int publishInterval;
        private int idleInterval;

        List<string> messageQueue;
        private bool queueInUse;

        private IConnection connection;
        private IModel consumerChannel;
        private IModel publisherChannel;

        private string userName;
        private string password;
        private string hostName;
        private string queueName;

        public Broker(string userName, string password, string hostName, string queueName, int publishInterval, int idleInterval)
        {
            connected = false;

            this.userName = userName;
            this.password = password;
            this.hostName = hostName;
            this.queueName = queueName;

            this.publishInterval = publishInterval;
            this.idleInterval = idleInterval;

            messageQueue = new List<string>();
            queueInUse = false;
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

                Console.WriteLine("INFO: Success!");
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
            if (!queueInUse)
            {
                bool isSent = PublishMessage(msg);

                if (!isSent)
                {
                    messageQueue.Add(msg);
                    Console.WriteLine("INFO: Starting fallback queue timer...");

                    autoEvent = new AutoResetEvent(false);
                    aTimer = new Timer(PublishQueue, autoEvent, 0, idleInterval);

                    queueInUse = true;
                }
            }
            else
            {
                messageQueue.Add(msg);
            }
        }

        private void PublishQueue(Object stateInfo)
        {
            bool issent = true;

            while(messageQueue.Count > 0 && issent)
            {
                issent = PublishMessage(messageQueue[0]);
                if(issent)
                {
                    messageQueue.RemoveAt(0);
                }
                else
                {
                    Console.WriteLine("ERROR: Failed to publish queued messages. Pausing timer for " + idleInterval/1000 + "s.");
                    aTimer.Change(idleInterval, idleInterval);
                }
            }
            if (issent)
            {
                queueInUse = false;
                aTimer.Dispose();
                autoEvent.Dispose();
                GC.Collect();
            }
        }

        private bool PublishMessage(string msg)
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
    }
}
