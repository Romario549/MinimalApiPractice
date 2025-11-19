using MinimalApiPractice.Consumer;

var host = Host.CreateDefaultBuilder(args)
	.ConfigureServices((context, services) =>
	{
		services.AddHostedService<ConsumerService>();
	})
	.Build();

Console.WriteLine("Kafka Consumer Service запускается...");
await host.RunAsync();