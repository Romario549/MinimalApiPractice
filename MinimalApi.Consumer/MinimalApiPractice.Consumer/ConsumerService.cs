using Confluent.Kafka;
using System.Text.Json;

namespace MinimalApiPractice.Consumer;

public class ConsumerService : BackgroundService
{
	private readonly ILogger<ConsumerService> _logger;

	public ConsumerService(ILogger<ConsumerService> logger)
	{
		_logger = logger;
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		var config = new ConsumerConfig
		{
			BootstrapServers = "localhost:9092",
			GroupId = "todo-notification-group",
			AutoOffsetReset = AutoOffsetReset.Earliest
		};

		using var consumer = new ConsumerBuilder<Ignore, string>(config).Build();
		consumer.Subscribe("todo-created");

		_logger.LogInformation("Kafka Consumer подключен к топику todo-created");

		while (!stoppingToken.IsCancellationRequested)
		{
			try
			{
				var message = consumer.Consume(stoppingToken);

				if (message?.Message?.Value != null)
				{
					var decodedJson = System.Text.RegularExpressions.Regex.Unescape(message.Message.Value);
					var todoEvent = JsonSerializer.Deserialize<TodoEvent>(decodedJson);

					_logger.LogInformation($"Получено событие: {todoEvent!.EventType}");
					_logger.LogInformation($"Задача: {todoEvent.Data.Name}");
					_logger.LogInformation($"Описание: {todoEvent.Data.Description}");
					_logger.LogInformation($"Время: {todoEvent.Timestamp}");
					_logger.LogInformation("---");

				}
			}
			catch (ConsumeException ex)
			{
				_logger.LogError($"Ошибка Kafka: {ex.Error.Reason}");
				await Task.Delay(5000, stoppingToken);
			}
			catch (Exception ex)
			{
				_logger.LogError($"Общая ошибка: {ex.Message}");
				await Task.Delay(5000, stoppingToken);
			}
		}
	}
}

public record TodoEvent(Guid EventId, string EventType, DateTime Timestamp, ToDo Data);

public class ToDo
{
	public int Id { get; set; }
	public string Name { get; set; } = string.Empty;
	public string Description { get; set; } = string.Empty;
	public bool IsDone { get; set; }
}