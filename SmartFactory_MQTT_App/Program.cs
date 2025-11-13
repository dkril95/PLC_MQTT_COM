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

        Console.WriteLine("🔌 Starting MQTT Client...");
        var factory = new MqttFactory();
        var client = factory.CreateMqttClient();

        // Konfiguracja połączenia z brokerem
        var options = new MqttClientOptionsBuilder()
            .WithClientId("SmartFactory_Client")
            .WithTcpServer("localhost", 1883) // broker Mosquitto
            .Build();

        // Po połączeniu — subskrybuj topic PLC
        client.UseConnectedHandler(async e =>
        {
            Console.WriteLine("✅ Connected to MQTT Broker!");
            await client.SubscribeAsync("factory/data");
            Console.WriteLine("📡 Subscribed to topic: factory/data");
            Console.WriteLine("➡️ You can type commands (START, STOP, RESET)");
        });

        // Odbiór danych z PLC
        client.UseApplicationMessageReceivedHandler(e =>
        {
            string topic = e.ApplicationMessage.Topic;
            string payload = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
            int nullIndex = payload.IndexOf('\0');
            if (nullIndex >= 0)
                payload = payload.Substring(0, nullIndex);
            
            Console.WriteLine($"📥 [{topic}] {payload}");
        });

        // Obsługa błędów połączenia
        client.UseDisconnectedHandler(async e =>
        {
            Console.WriteLine("⚠️ Disconnected. Reconnecting...");
            await Task.Delay(TimeSpan.FromSeconds(5));
            await client.ConnectAsync(options);
        });

        // Połącz z brokerem
        await client.ConnectAsync(options);
        Console.WriteLine("🔗 Waiting for messages...");

        // Tryb interaktywny: wysyłanie komend
        while (true)
        {
            var cmd = Console.ReadLine()?.ToUpper()?.Trim();
            if (string.IsNullOrWhiteSpace(cmd))
                continue;

            if (cmd == "EXIT" || cmd == "QUIT")
            {
                Console.WriteLine("👋 Exiting...");
                break;
            }

            var message = new MqttApplicationMessageBuilder()
                .WithTopic("factory/command")
                .WithPayload(cmd)
                .WithExactlyOnceQoS()
                .Build();

            await client.PublishAsync(message);
            Console.WriteLine($"➡️ Sent command: {cmd}");
        }

        await client.DisconnectAsync();
    }
}