using Confluent.Kafka;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

public class KafkaConsumer
{
    private static HashSet<string> processedKeys = new HashSet<string>(); 

    public async Task StartConsuming()
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = "localhost:9092", 
            GroupId = "order-consumer-group",
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        using (var consumer = new ConsumerBuilder<string, string>(config).Build())
        {
            consumer.Subscribe("order-topic");

            try
            {
                while (true)
                {
                    var consumeResult = consumer.Consume();
                    var key = consumeResult.Message.Key;  
                    var payloadJson = consumeResult.Message.Value;  

                    if (!processedKeys.Contains(key))
                    {
                        processedKeys.Add(key);

                        var payload = System.Text.Json.JsonSerializer.Deserialize<bbOrderPayload>(payloadJson);

                        Console.WriteLine($"Processing order with OrderKey: {payload.OrderKey}");

                        foreach (var item in payload.LineItems)
                        {
                            Console.WriteLine($"Processing Line Item: {item.ItemNumber}, Quantity: {item.Quantity}");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Duplicate message detected for key: {key}. Skipping...");
                    }
                }
            }
            catch (ConsumeException e)
            {
                Console.WriteLine($"Error occurred: {e.Error.Reason}");
            }
            finally
            {
                consumer.Close();
            }
        }
    }
}

class Program
{
    static async Task Main(string[] args)
    {
        var consumer = new KafkaConsumer();
        await consumer.StartConsuming();

        Console.WriteLine("Press any key to exit...");

        Console.ReadKey();
    }
}
