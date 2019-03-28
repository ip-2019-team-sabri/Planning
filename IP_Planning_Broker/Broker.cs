using IP_Planning_Broker.Messages;
using IP_Planning_Logger;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Xml;
using System.Xml.Serialization;

namespace IP_Planning_Broker
{
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

        List<string> cachedMessages;
        private Timer cacheRetryTimer;
        private AutoResetEvent cacheAutoEvent;
        

        private Timer connectionRetryTimer;
        private AutoResetEvent connectionAutoEvent;

        //
        private const int connectionTimeoutInterval = 5000;
        private const int connectionRetryInterval = 30000;
        private const int cacheRetryInterval = 60000;

        Logger logger;

        public Broker(string userName, string password, string hostName, string queueName)
        {
            threadFrozen = false;

            this.userName = userName;
            this.password = password;
            this.hostName = hostName;
            this.queueName = queueName;

            cachedMessages = null;
            cacheRetryTimer = null;
            cacheAutoEvent = null;

            connectionRetryTimer = null;
            connectionAutoEvent = null;

            logger = Logger.Instance;
        }

    //####### IMPLEMENT ACTIONS FOR MESSAGES HERE ######

        //This is the method that gets called when a message arrives
        private void ProcessMessage(string message)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(message);

                //Get the type of the message
                string messageType = doc.DocumentElement.Name;

                logger.Log("Received message of type: " + messageType, "info");

                if (messageType == "PingMessage")
                {
                    logger.Log("Processing message", "info");
                    XmlSerializer serializer = new XmlSerializer(typeof(PingMessage));
                    XmlReader reader = new XmlNodeReader(doc);
                    PingMessage pingMessage = (PingMessage)serializer.Deserialize(reader);

                    //Do something with pingMessage

                }
                else if(messageType == "EventMessage")
                {
                    logger.Log("Processing message", "info");
                }
                else
                {
                    logger.Log("Discarding message", "info");
                }
            }
            catch (XmlException e)
            {
                logger.Log("Could not process message: " + e.GetType(), "error");
            }
        }

    //####### DO NOT EDIT THE CODE BELOW THIS COMMENT #######

        //These are the only methods you need to use the broker
        #region Public methods

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
            connectionAutoEvent = new AutoResetEvent(false);
            connectionRetryTimer = new Timer(TryConnection, connectionAutoEvent, 0, connectionRetryInterval);
            connectionAutoEvent.WaitOne();
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
        }

        public void NewMessage(string msg)
        {
            if (cachedMessages == null)
            {
                if (!PublishMessage(msg))
                {
                    CacheMessage(msg);
                }
            }
            else
            {
                CacheMessage(msg);
            }
        }

        #endregion

        #region Private methods
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

        private void CacheMessage(string msg)
        {
            if (cachedMessages == null)
            {
                EnableCache();
                cachedMessages.Add(msg);
                logger.Log("Message added to cache.", "info");
            }
            else
            {
                cachedMessages.Add(msg);
                logger.Log("Message added to cache.", "info");

                if (IsConnected())
                {
                    logger.Log("Freezing main thread.", "warning");
                    threadFrozen = true;
                    cacheRetryTimer.Change(0, -1);
                    cacheAutoEvent.WaitOne();
                    logger.Log("Main thread unfrozen.", "warning");
                }
            }
        }

        private void PublishCachedMessages(Object stateInfo)
        {
            logger.Log("Attempting to publish messages from fallback queue", "info");
            logger.Log("There are currently " + cachedMessages.Count + " messages in the fallback queue.", "info");

            while (cachedMessages.Count > 0 && IsConnected())
            {
                if (PublishMessage(cachedMessages[0]))
                {
                    cachedMessages.RemoveAt(0);
                }
            }
            if (threadFrozen && cachedMessages.Count == 0)
            {
                DisableCache();
            }
            else if (threadFrozen && cachedMessages.Count != 0)
            {
                logger.Log("Unfreezing main thread.", "warning");
                cacheAutoEvent.Set();
                threadFrozen = false;
            }
        }

        private void EnableCache()
        {
            logger.Log("Enabling fallback queue.", "info");

            cachedMessages = new List<string>();
            cacheAutoEvent = new AutoResetEvent(false);
            cacheRetryTimer = new Timer(PublishCachedMessages, cacheAutoEvent, 0, cacheRetryInterval);
        }

        private void DisableCache()
        {
            //Order is important!
            logger.Log("Disabling fallback queue.", "info");

            cacheRetryTimer.Dispose();
            cacheRetryTimer = null;

            cachedMessages = null;

            logger.Log("Unfreezing main thread.", "warning");
            cacheAutoEvent.Set();
            threadFrozen = false;

            cacheAutoEvent.Dispose();
            cacheAutoEvent = null;
        }

        private void TryConnection(Object stateInfo)
        {
            if (connection == null)
            {
                ConnectionFactory factory = new ConnectionFactory()
                {
                    UserName = userName,
                    Password = password,
                    HostName = hostName,
                    RequestedConnectionTimeout = connectionTimeoutInterval
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

                        ProcessMessage(message);
                    };

                    consumerChannel.BasicConsume(queue: queueName,
                                         autoAck: true,
                                         consumer: consumer);

                    logger.Log("Success!", "info");
                    DisableConnectioRetryTimer();
                }
                catch (BrokerUnreachableException e)
                {
                    logger.Log("Failed to connect to RabbitMQ server: " + e.GetType(), "error");
                    CloseConnection();
                    connectionAutoEvent.Set();
                }
                catch (OperationInterruptedException e)
                {
                    logger.Log("Failed to connect to RabbitMQ server: " + e.GetType(), "error");
                    CloseConnection();
                    connectionAutoEvent.Set();
                }
            }
        }

        private void DisableConnectioRetryTimer()
        {
            connectionRetryTimer.Dispose();

            connectionAutoEvent.Set();
            connectionAutoEvent.Dispose();

            connectionRetryTimer = null;
            connectionAutoEvent = null;
        }
        #endregion
    }
}
