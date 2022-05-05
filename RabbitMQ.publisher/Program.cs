using RabbitMQ.Client;
using System;
using System.Linq;
using System.Text;

namespace RabbitMQ.publisher
{
    public enum LogNames
    {
        Critical = 1,
        Error = 2,
        Warning = 3,
        Info = 4
    }
    class Program
    {
        static void Main(string[] args)
        {
            //routeKey'de string ifadeler gönderiyoruz. >>>Critical.Error.Warning veya Warning.Info.Critical gibi.
            //bu örnekte kuyruk oluşturma işlemini consumerlara(subscriber) bırakıyoruz çünkü varyasyon çok fazla.
            //yıldız(*) ifadesi herhangi bir şey olabileceğini temsil etmektedir. Örneğin ortadaki ifadenin "Error" oluğ baştaki ve sonrakinin herhangi bir ifade olmasını istersek
            //>>>> *.Error.* şeklinde yazmamız gerekir.
            //# ifadesi başta ya da sondakinin ilgili parametre olmasını sağlar >>> #.Warning
            //Topic Exchange çok detaylı bir route'lama yapılmak istendiğinde kullanılabilecek bir exchange tipidir.
            var factory = new ConnectionFactory();
            factory.Uri = new Uri("amqps://nxdranwu:n_3nr-xZlXx0NoCWuFP05gTqZfp7_hwK@sparrow.rmq.cloudamqp.com/nxdranwu");
            
            using var connection = factory.CreateConnection();

            var channel = connection.CreateModel();

            channel.ExchangeDeclare("logs-topic", durable: true, type: ExchangeType.Topic);

            Random rnd = new Random();
            Enumerable.Range(1, 100).ToList().ForEach(x =>
             {
                 LogNames log1 = (LogNames)rnd.Next(1, 5);
                 LogNames log2 = (LogNames)rnd.Next(1, 5);
                 LogNames log3 = (LogNames)rnd.Next(1, 5);
                 var routeKey = $"{log1}.{log2}.{log3}";

                 var message = $"log-type: {log1}-{log2}-{log3}";
                 var messageBody = Encoding.UTF8.GetBytes(message);

                 channel.BasicPublish("logs-topic", routeKey, null, messageBody);

                 Console.WriteLine($"log gönderilmiştir {message}");
             });

            Console.ReadLine();
            
        }
    }
}
