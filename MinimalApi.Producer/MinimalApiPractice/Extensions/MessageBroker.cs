using Confluent.Kafka;
using MinimalApiPractice.Services;

namespace MinimalApiPractice.Extensions;

public static class MessageBroker
{
	public static IServiceCollection AddKafka(this IServiceCollection services)
	{
		var kafkaConfig = new ProducerConfig
		{
			BootstrapServers = "localhost:9092"
		};
		services.AddSingleton(kafkaConfig);
		services.AddSingleton<IProducer<Null, string>>(sp => new ProducerBuilder<Null, string>(kafkaConfig).Build());
		services.AddScoped<KafkaService>();
		return services;
	}
}
