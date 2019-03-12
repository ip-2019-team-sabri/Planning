using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace IP_Planning_Broker
{
    //Has 2 methods:
    //SendMessage(string) for sending a message. (might take xml argument later on)
    //StartConsumer(). This starts the broker for consuming messages. Make sure you do not change the channel variable afterwards, as it will top consuming.

    //You can simply copy this class and add it to you code. It should work on the fly, using your own credentials.

    public class Broker
    {
        private IConnection connection;
        private IModel channel;

        private string queueName;

        public Broker(string userName, string password, string hostName, string queueName)
        {
            ConnectionFactory factory = new ConnectionFactory()
            {
                UserName = userName,
                Password = password,
                HostName = hostName
            };

            connection = factory.CreateConnection();
            channel = connection.CreateModel();
            this.queueName = queueName;
        }

        public void SendMessage(string msg)
        {
            //Body maken (in productie is dees uwen XML)
            var body = Encoding.UTF8.GetBytes(msg);

            //Properties opstellen (gewoon zo laten en youll be fine)
            var properties = channel.CreateBasicProperties();
            properties.Persistent = true;

            //Verzenden naar de default fanout exchange
            channel.BasicPublish(exchange: "amq.fanout",
                                 routingKey: "",
                                 basicProperties: properties,
                                 body: body);
        }

        public void StartConsumer()
        {
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body;
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine("Received message: {0}", message);
            };

            channel.BasicConsume(queue: queueName,
                                 autoAck: true,
                                 consumer: consumer);
        }
    }
}
