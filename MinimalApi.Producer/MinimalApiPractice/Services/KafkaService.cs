using Confluent.Kafka;
using System.Diagnostics;
using System.Text.Json;

namespace MinimalApiPractice.Services;

public class KafkaService
{
	private readonly IProducer<Null, string> _producer;

	public KafkaService(IProducer<Null, string> producer)
	{
		_producer = producer;
	}

	public async Task SendToDoEventAsync(string topic, string eventType, ToDo toDo)
	{
		var message = new
		{
			EventId = Guid.NewGuid(),
			EventType = eventType,
			Timestamp = DateTime.UtcNow,
			Data = toDo
		};
		var json = JsonSerializer.Serialize(message);
		await _producer.ProduceAsync(topic, new Message<Null, string> { Value = json });
	}
}
