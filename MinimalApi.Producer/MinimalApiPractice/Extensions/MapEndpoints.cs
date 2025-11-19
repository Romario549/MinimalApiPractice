using Microsoft.AspNetCore.Mvc;
using MinimalApiPractice.Services;
using System.Text.Json;

namespace MinimalApiPractice.Extensions;

public static class MapEndpoints
{
	public static IEndpointRouteBuilder AddMapEndpoints(
		this IEndpointRouteBuilder app,
		string path)
	{

		app.MapGet("/todos", async () => await GetAllTasksAsync(path));

		app.MapGet("/todos/{id}", async (int id) =>
		{
			var tasks = await GetAllTasksAsync(path);
			var task = tasks.FirstOrDefault(t => t.Id == id);
			return task is not null ? Results.Ok(task) : Results.NotFound($"Задачи с id {id} не существует");
		});

		app.MapPost("/todos", async ([FromBody] ToDoReq toDoReq, KafkaService kafka) =>
		{
			var tasks = await GetAllTasksAsync(path);
			var newId = tasks.Count > 0 ? tasks.Max(t => t.Id) + 1 : 1;
			var task = new ToDo(tasks.Count > 0 ? tasks.Max(t => t.Id) + 1 : 1, toDoReq.Name, toDoReq.Description, toDoReq.IsDone);
			tasks.Add(task);
			await File.WriteAllTextAsync(path, JsonSerializer.Serialize(tasks));
			await kafka.SendToDoEventAsync("todo-created", "TodoCreated", task);
			return Results.Created($"/todoitems/{task.Id}", task);
		});

		app.MapPut("/todos/{id}", async (int id, [FromBody] ToDoReq toDoReq, KafkaService kafka) =>
		{
			var tasks = await GetAllTasksAsync(path);
			var task = tasks.FirstOrDefault(t => t.Id == id);
			if (task == null) return Results.NotFound($"Задачи с id {id} не существует");
			task.Name = toDoReq.Name;
			task.Description = toDoReq.Description;
			task.IsDone = toDoReq.IsDone;
			await File.WriteAllTextAsync(path, JsonSerializer.Serialize(tasks));
			await kafka.SendToDoEventAsync("todo-updated", "TodoUpdated", task);
			return Results.NoContent();
		});

		app.MapDelete("/todos/{id}", async (int id) =>
		{
			var tasks = await GetAllTasksAsync(path);
			var task = tasks.FirstOrDefault(t => t.Id == id);
			if (task == null) return Results.NotFound();
			tasks.Remove(task);
			await File.WriteAllTextAsync(path, JsonSerializer.Serialize(tasks));
			return Results.NoContent();
		});
		return app;
	}
	static async Task<List<ToDo>> GetAllTasksAsync(string path)
	{
		if (!File.Exists(path)) return [];
		var json = await File.ReadAllTextAsync(path);
		return JsonSerializer.Deserialize<List<ToDo>>(json, options: new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? [];
	}
}
