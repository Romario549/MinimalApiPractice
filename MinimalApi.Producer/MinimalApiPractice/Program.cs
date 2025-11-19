using MinimalApiPractice.Extensions;

const string path = "todos.json";

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddKafka();

var app = builder.Build();

app.AddMapEndpoints(path);

app.Run();