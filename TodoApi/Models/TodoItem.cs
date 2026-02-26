namespace TodoApi.Models;

// Modelo que representa a tarefa.
public class TodoItem
{
    public int Id { get; set; }          // ID (chave primária)
    public string Title { get; set; } = ""; // Texto da tarefa
    public bool Done { get; set; } = false; // Concluída?
}