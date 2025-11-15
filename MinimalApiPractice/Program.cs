using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);
const string path = "todos.json";
var app = builder.Build();

static async Task<List<ToDo>> GetAllTasksAsync()
{
	if (!File.Exists(path)) return [];
	var json = await File.ReadAllTextAsync(path);
	return JsonSerializer.Deserialize<List<ToDo>>(json, options: new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? [];
}

app.MapGet("/todos", async () => await GetAllTasksAsync());

app.MapGet("/todos/{id}", async (int id) =>
{
	var tasks = await GetAllTasksAsync();
	var task = tasks.FirstOrDefault(t => t.Id == id);
	return task is not null ? Results.Ok(task) : Results.NotFound($"Задачи с id {id} не существует");
});

app.MapPost("/todos", async ([FromBody] ToDoReq toDoReq) =>
{
	var tasks = await GetAllTasksAsync();
	var newId = tasks.Count > 0 ? tasks.Max(t => t.Id) + 1 : 1;
	var task = new ToDo(tasks.Count > 0 ? tasks.Max(t => t.Id) + 1 : 1, toDoReq.Name, toDoReq.Description, toDoReq.IsDone);
	tasks.Add(task);
	await File.WriteAllTextAsync(path, JsonSerializer.Serialize(tasks));
	return Results.Created($"/todoitems/{task.Id}", task);
});

app.MapPut("/todos/{id}", async (int id, [FromBody] ToDoReq toDoReq) =>
{
	var tasks = await GetAllTasksAsync();
	var task = tasks.FirstOrDefault(t => t.Id == id);
	if(task == null) return Results.NotFound($"Задачи с id {id} не существует");
	task.Name = toDoReq.Name;
	task.Description = toDoReq.Description;
	task.IsDone = toDoReq.IsDone;
	await File.WriteAllTextAsync(path, JsonSerializer.Serialize(tasks));
	return Results.NoContent();
});

app.MapDelete("/todos/{id}", async (int id) =>
{	
	var tasks = await GetAllTasksAsync(); 
	var task = tasks.FirstOrDefault(t => t.Id == id);
	if (task == null) return Results.NotFound();
	tasks.Remove(task);
	await File.WriteAllTextAsync(path, JsonSerializer.Serialize(tasks));
	return Results.NoContent();
});

app.Run();

record ToDoReq(string Name, string Description, bool IsDone);

class ToDo(int id, string name, string description, bool isDone)
{
	public int Id { get; set; } = id;
	public string Name { get; set; } = name;
	public string Description { get; set; } = description;
	public bool IsDone { get; set; } = isDone;
}