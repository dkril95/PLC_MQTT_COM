using System;
using System.Text;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;

class Program
{
    static async Task Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;

        bool isConnected = false;
        bool isSubscribed = false;

        var factory = new MqttFactory();
        var client = factory.CreateMqttClient();

        var options = new MqttClientOptionsBuilder()
            .WithClientId("SmartFactory_Client")
            .WithTcpServer("localhost", 1883)
            .Build();

        // === HANDLERY ===

        client.UseConnectedHandler(e =>
        {
            isConnected = true;
            Console.WriteLine("✅ MQTT connected");
        });

        client.UseDisconnectedHandler(async e =>
        {
            isConnected = false;
            isSubscribed = false;
            Console.WriteLine("⚠️ MQTT disconnected");
            await Task.Delay(2000);
        });

        client.UseApplicationMessageReceivedHandler(e =>
        {
            string payload = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
            int nullIndex = payload.IndexOf('\0');
            if (nullIndex >= 0)
                payload = payload.Substring(0, nullIndex);

            Console.WriteLine($"📥 [{e.ApplicationMessage.Topic}] {payload}");
        });

        Console.WriteLine("Type: START / STOP / RESET / EXIT");

        // === GŁÓWNA PĘTLA ===
        while (true)
        {
            Console.Write("> ");
            var cmd = Console.ReadLine()?.Trim().ToUpper();
            if (string.IsNullOrEmpty(cmd))
                continue;

            // === EXIT ===
            if (cmd == "EXIT")
            {
                if (isConnected)
                {
                     await client.PublishAsync(new MqttApplicationMessageBuilder()
                        .WithTopic("factory/command")
                        .WithPayload("EXIT")
                        .Build());

                    await client.DisconnectAsync();
                }

                Console.WriteLine("👋 Application closed");
                break;
            }

            // === START ===
            if (cmd == "START")
            {
                if (!isConnected)
                {
                    await client.ConnectAsync(options);
                }

                if (!isSubscribed)
                {
                    await client.SubscribeAsync("factory/data");
                    isSubscribed = true;
                    Console.WriteLine("📡 Communication STARTED");
                }

                await client.PublishAsync(new MqttApplicationMessageBuilder()
                    .WithTopic("factory/command")
                    .WithPayload("START")
                    .Build());

                continue;
            }

            // === STOP ===
            if (cmd == "STOP")
            {
                if (isSubscribed)
                {
                    await client.UnsubscribeAsync("factory/data");
                    isSubscribed = false;
                    Console.WriteLine("⛔ Communication STOPPED");
                }

                if (isConnected)
                {
                    await client.PublishAsync(new MqttApplicationMessageBuilder()
                        .WithTopic("factory/command")
                        .WithPayload("STOP")
                        .Build());
                }

                continue;
            }

            // === RESET ===
            if (cmd == "RESET")
            {
                if (isConnected)
                {
                    await client.PublishAsync(new MqttApplicationMessageBuilder()
                        .WithTopic("factory/command")
                        .WithPayload("RESET")
                        .Build());

                    Console.WriteLine("🔄 RESET sent to PLC");
                }
                else
                {
                    Console.WriteLine("⚠️ MQTT not connected");
                }

                continue;
            }

            Console.WriteLine("❓ Unknown command");
        }
    }
}