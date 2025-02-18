using Confluent.Kafka;
using System.Text.Json;
using UserPermissionsN5.Common;

namespace UserPermissionsN5.Services
{
    public class KafkaProducerService : IMessageBroker
    {
        private readonly string _bootstrapServers;
        private readonly string _topic;

        public KafkaProducerService(IConfiguration configuration)
        {
            _bootstrapServers = configuration["Kafka:Host"] ?? Constants.KAFKA_DEFAULT_HOST;
            _topic = configuration["Kafka:Topic"] ?? Constants.KAFKA_DEFAULT_TOPIC;
        }

        public async Task SendMessageAsync(Guid id, string operation)
        {
            var config = new ProducerConfig { BootstrapServers = _bootstrapServers };

            using var producer = new ProducerBuilder<Null, string>(config).Build();

            var message = new { Id = id, NameOperation = operation };
            var messageJson = JsonSerializer.Serialize(message);

            await producer.ProduceAsync(_topic, new Message<Null, string> { Value = messageJson });
        }
    }

}
