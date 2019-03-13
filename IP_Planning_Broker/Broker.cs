using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System;
using System.Text;

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

        public Broker(string userName, string password, string hostName, string queueName)
        {
            this.userName = userName;
            this.password = password;
            this.hostName = hostName;
            this.queueName = queueName;

            ConnectionFactory factory = new ConnectionFactory()
            {
                UserName = userName,
                Password = password,
                HostName = hostName
            };

            try
            {
                connection = factory.CreateConnection();
                consumerChannel = connection.CreateModel();
                publisherChannel = connection.CreateModel();
            }
            catch (BrokerUnreachableException e)
            {
                Console.WriteLine("ERROR: Failed to initialize broker. " + e.Message + ".");
                CloseConnection();
            }
        }

        public void StartConsumer()
        {
            try
            {
                if (consumerChannel != null)
                {
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
                else
                {
                    Console.WriteLine("ERROR: Failed to start consumer. Trying to activate consumer while channel is not initialized.");
                }
            }
            catch(OperationInterruptedException e)
            {
                Console.WriteLine("ERROR: Failed to start consumer. " + e.Message);
                CloseConnection();
            }
        }

        public void SendMessage(string msg)
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
                }
                else
                {
                    Console.WriteLine("ERROR: Trying to publish message while channel is not initialized.");
                }
            }
            catch(Exception e)
            {
                Console.WriteLine("ERROR: Failed to send message. " + e.Message);
                CloseConnection();
            }
        }

        public void CloseConnection()
        {
            if (consumerChannel != null)
                consumerChannel.Dispose();

            if (publisherChannel != null)
                publisherChannel.Dispose();

            if (connection != null)
                connection.Dispose();
        }
    }
}
