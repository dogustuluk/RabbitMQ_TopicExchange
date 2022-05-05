using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;

namespace RabbitMQ.subscriber2
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory();
            factory.Uri = new Uri("amqps://nxdranwu:n_3nr-xZlXx0NoCWuFP05gTqZfp7_hwK@sparrow.rmq.cloudamqp.com/nxdranwu");

            using var connection = factory.CreateConnection();

            var channel = connection.CreateModel();

            channel.ExchangeDeclare("logs-topic", durable: true, type: ExchangeType.Topic);

            channel.BasicQos(0, 1, false);

            var consumer = new EventingBasicConsumer(channel);

            var queueName = channel.QueueDeclare().QueueName;

            var routeKey = "Info.#";

            channel.QueueBind(queueName, "logs-topic", routeKey);

            channel.BasicConsume(queueName, false, consumer);

            Console.WriteLine("loglar dinleniyor");

            consumer.Received += (object sender, BasicDeliverEventArgs e) =>
            {
                var message = Encoding.UTF8.GetString(e.Body.ToArray());

                Thread.Sleep(1500);

                Console.WriteLine("Gelen mesaj: " + message);

                channel.BasicAck(e.DeliveryTag, false);
            };

            Console.ReadLine();
        }

    }
}
