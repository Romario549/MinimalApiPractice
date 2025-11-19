public class ToDo(int id, string name, string description, bool isDone)
{
	public int Id { get; set; } = id;
	public string Name { get; set; } = name;
	public string Description { get; set; } = description;
	public bool IsDone { get; set; } = isDone;
}