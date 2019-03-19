using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace IP_Planning_Broker
{
    //test 2
    public class Broker
    {
        bool threadFrozen;

        private IConnection connection;
        private IModel consumerChannel;
        private IModel publisherChannel;

        private string userName;
        private string password;
        private string hostName;
        private string queueName;

        List<string> fbQueue;
        private Timer fbRetryTimer;
        private AutoResetEvent fbAutoEvent;
        private int fbRetryInterval;
        
        Logger logger;

        public Broker(string userName, string password, string hostName, string queueName, int publishInterval)
        {
            threadFrozen = false;

            this.userName = userName;
            this.password = password;
            this.hostName = hostName;
            this.queueName = queueName;

            fbQueue = null;
            fbRetryTimer = null;
            fbAutoEvent = null;
            fbRetryInterval = publishInterval;

            logger = new Logger();
        }

        public bool IsConnected()
        {
            if (connection != null)
            {
                return connection.IsOpen;
            }
            return false;
        }

        public void OpenConnection()
        {
            if (connection == null)
            {
                ConnectionFactory factory = new ConnectionFactory()
                {
                    UserName = userName,
                    Password = password,
                    HostName = hostName
                };

                try
                {
                    logger.Log("Connecting to RabbitMQ server...", "info");

                    connection = factory.CreateConnection();
                    consumerChannel = connection.CreateModel();
                    publisherChannel = connection.CreateModel();

                    var consumer = new EventingBasicConsumer(consumerChannel);
                    consumer.Received += (model, ea) =>
                    {
                        var body = ea.Body;
                        var message = Encoding.UTF8.GetString(body);
                        logger.Log("Received message: " + message, "info");
                    };

                    consumerChannel.BasicConsume(queue: queueName,
                                         autoAck: true,
                                         consumer: consumer);

                    logger.Log("Success!", "info");
                }
                catch (BrokerUnreachableException e)
                {
                    logger.Log("Failed to connect to RabbitMQ server. " + e.GetType(), "error");
                    CloseConnection();
                }
                catch (OperationInterruptedException e)
                {
                    logger.Log("Failed to connect to RabbitMQ server. " + e.GetType(), "error");
                    CloseConnection();
                }
            }
        }

        public void CloseConnection()
        {
            logger.Log("Closing connection...", "info");
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
        }

        public void NewMessage(string msg)
        {
            if (fbQueue == null)
            {
                if (!PublishMessage(msg))
                {
                    QueueMessage(msg);
                }
            }
            else
            {
                QueueMessage(msg);
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
                    logger.Log("Message sent.", "info");
                    return true;
                }
                else
                {
                    logger.Log("Trying to publish message while channel is not initialized.", "error");
                    return false;
                }
            }
            catch (Exception e)
            {
                logger.Log("Failed to send message. " + e.GetType(), "error");
                return false;
            }
        }

        private void QueueMessage(string msg)
        {
            if(fbQueue == null)
            {
                EnableFbQueue();
                fbQueue.Add(msg);
                logger.Log("Message added to fallback queue.", "info");
            }
            else
            {
                fbQueue.Add(msg);
                logger.Log("Message added to fallback queue.", "info");

                if (IsConnected())
                {
                    logger.Log("Freezing main thread.", "warning");
                    threadFrozen = true;
                    fbRetryTimer.Change(0, -1);
                    fbAutoEvent.WaitOne();
                    logger.Log("Main thread unfrozen.", "warning");
                }
            }
        }

        private void PublishFbQueue(Object stateInfo)
        {
            logger.Log("Attempting to publish messages from fallback queue", "info");
            logger.Log("There are currently " + fbQueue.Count + " messages in the fallback queue.", "info");

            while (fbQueue.Count > 0 && IsConnected())
            {
                if (PublishMessage(fbQueue[0]))
                {
                    fbQueue.RemoveAt(0);
                }
            }
            if(threadFrozen && fbQueue.Count == 0)
            {
                DisableFbQueue();
            }
            else if (threadFrozen && fbQueue.Count != 0)
            {
                logger.Log("Unfreezing main thread.", "warning");
                fbAutoEvent.Set();
                threadFrozen = false;
            }
        }

        private void EnableFbQueue()
        {
            logger.Log("Enabling fallback queue.", "info");

            fbQueue = new List<string>();
            fbAutoEvent = new AutoResetEvent(false);
            fbRetryTimer = new Timer(PublishFbQueue, fbAutoEvent, 0, fbRetryInterval);
        }

        private void DisableFbQueue()
        {
            //Order is important!
            logger.Log("Disabling fallback queue.", "info");

            fbRetryTimer.Dispose();
            fbRetryTimer = null;

            fbQueue = null;

            logger.Log("Unfreezing main thread.", "warning");
            fbAutoEvent.Set();
            threadFrozen = false;

            fbAutoEvent.Dispose();
            fbAutoEvent = null;
        }
    }
}
