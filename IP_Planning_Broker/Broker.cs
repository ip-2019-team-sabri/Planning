using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace IP_Planning_Broker
{
    public class Broker
    {
        private IConnection connection;
        private IModel consumerChannel;
        private IModel publisherChannel;

        private string userName;
        private string password;
        private string hostName;
        private string queueName;

        private int retryInterval;
        private Timer retryTimer;
        private AutoResetEvent autoEvent;

        List<string> fallbackQueue;

        Logger logger;

        public Broker(string userName, string password, string hostName, string queueName, int publishInterval)
        {
            this.userName = userName;
            this.password = password;
            this.hostName = hostName;
            this.queueName = queueName;

            this.retryInterval = publishInterval;

            fallbackQueue = null;
            retryTimer = null;
            autoEvent = null;

            logger = new Logger();
        }

        //Returns true if the broker is connected to the rabbitmq server.
        public bool IsConnected()
        {
            if (connection != null)
            {
                return connection.IsOpen;
            }
            return false;
        }

        //Connects to the rabbitmq server using the given credentials.
        //Use GetConnectionStatus to check if the connection succeeded.
        //You should make sure to repeat this method after a given interval when connection fails.
        //Once connected the connection will automatically reconnect when there is a temporary outage.
        public void OpenConnection()
        {
            if (!IsConnected())
            {
                ConnectionFactory factory = new ConnectionFactory()
                {
                    UserName = userName,
                    Password = password,
                    HostName = hostName
                };

                logger.Log("Connecting to RabbitMQ server...", "info");

                try
                {
                    connection = factory.CreateConnection();
                    consumerChannel = connection.CreateModel();
                    publisherChannel = connection.CreateModel();

                    var consumer = new EventingBasicConsumer(consumerChannel);
                    consumer.Received += (model, ea) =>
                    {
                        var body = ea.Body;
                        var message = Encoding.UTF8.GetString(body);
                        logger.Log("Received message: "+ message, "info");
                    };

                    consumerChannel.BasicConsume(queue: queueName,
                                         autoAck: true,
                                         consumer: consumer);

                    logger.Log("Success!", "info");
                }
                catch (BrokerUnreachableException e)
                {
                    logger.Log("Failed to open connection. " + e.GetType(), "error");
                    CloseConnection();
                }
                catch (OperationInterruptedException e)
                {
                    logger.Log("Failed to open connection. " + e.GetType(), "error");
                    CloseConnection();
                }
            }
        }

        //Carefully closes all channes and connections.
        //This is mandatory to avoid memory leaks.
        public void CloseConnection()
        {
            logger.Log("Closing connection.", "info");
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

        //Tries to send the given  message to the rabbitmq server.
        //If the broker fails to send te message, it will locally queue the message in the fallback queue.
        public void QueueMessage(string msg)
        {
            logger.Log("Queueing message.", "info");
            if (fallbackQueue == null)
            {
                if (!PublishMessage(msg))
                {
                    logger.Log("Switching to fallback queue.", "warning");
                    EnableFallbackQueue();
                    fallbackQueue.Add(msg);
                    logger.Log("Message added to fallback queue.", "info");
                }
            }
            else
            {
                fallbackQueue.Add(msg);
                logger.Log("Message added to fallback queue.", "info");

                if (IsConnected())
                {
                    retryTimer.Change(0, -1);
                    logger.Log("Freezing main thread.", "warning");
                    autoEvent.WaitOne();
                    logger.Log("Switching back to live publishing.", "warning");
                }
            }
        }


        //Do not use the private methods
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

        private void EnableFallbackQueue()
        {
            logger.Log("Enabling fallback queue.", "info");
            fallbackQueue = new List<string>();
            autoEvent = new AutoResetEvent(false);
            retryTimer = new Timer(PublishFallbackQueue, autoEvent, retryInterval, retryInterval);
        }

        private void DisableFallbackQueue()
        {
            logger.Log("Disabling fallback queue.", "info");
            retryTimer.Dispose();
            autoEvent.Dispose();
            fallbackQueue = null;
            logger.Log("Unfreezing main thread.", "warning");
            autoEvent.Set();
            GC.Collect();
        }

        private void PublishFallbackQueue(Object stateInfo)
        {
            int startCount = fallbackQueue.Count;

            logger.Log("Attempting to publish messages from fallback queue", "info");
            logger.Log("There are currently " + fallbackQueue.Count + " messages in the fallback queue.", "info");
            while (fallbackQueue.Count > 0 && IsConnected())
            {
                if (PublishMessage(fallbackQueue[0]))
                {
                    fallbackQueue.RemoveAt(0);
                }
            }
            if (fallbackQueue.Count == 0)
            {
                logger.Log("Successfully sent messages.", "info");
                DisableFallbackQueue();
            }
            else if(startCount > fallbackQueue.Count)
            {
                logger.Log("Failed to send all messages, trying again in " + retryInterval / 1000 + "s.", "info");
                logger.Log("Unfreezing main thread.", "warning");
                autoEvent.Set();
            }
            else
            {
                logger.Log("Failed to send messages.");
            }
        }
    }
}
