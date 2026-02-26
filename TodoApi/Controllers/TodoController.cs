using Microsoft.AspNetCore.Mvc;
using TodoApi.Models;

namespace TodoApi.Controllers;

[ApiController]
[Route("api/todo")]
public class TodoController : ControllerBase
{
    // Lista em memória (some ao reiniciar a API)
    private static readonly List<TodoItem> Tasks = new();

    // GET /api/todo  -> listar tudo
    [HttpGet]
    public ActionResult<List<TodoItem>> GetAll()
    {
        return Ok(Tasks);
    }

    // POST /api/todo -> criar
    [HttpPost]
    public ActionResult<TodoItem> Create([FromBody] TodoItem newTask)
    {
        if (string.IsNullOrWhiteSpace(newTask.Title))
            return BadRequest("Title não pode ser vazio.");

        var nextId = Tasks.Count == 0 ? 1 : Tasks.Max(t => t.Id) + 1;
        newTask.Id = nextId;

        Tasks.Add(newTask);

        return Created($"/api/todo/{newTask.Id}", newTask);
    }

    // PUT /api/todo/{id} -> alterna Done
    [HttpPut("{id}")]
    public ActionResult<TodoItem> ToggleDone(int id)
    {
        var task = Tasks.FirstOrDefault(t => t.Id == id);
        if (task == null) return NotFound();

        task.Done = !task.Done;
        return Ok(task);
    }

    // PUT /api/todo/{id}/title -> edita o título
    [HttpPut("{id}/title")]
    public ActionResult<TodoItem> UpdateTitle(int id, [FromBody] TodoItem updated)
    {
        var task = Tasks.FirstOrDefault(t => t.Id == id);
        if (task == null) return NotFound();

        if (string.IsNullOrWhiteSpace(updated.Title))
            return BadRequest("Title não pode ser vazio.");

        task.Title = updated.Title;
        return Ok(task);
    }

    // DELETE /api/todo/{id} -> apagar
    [HttpDelete("{id}")]
    public ActionResult Delete(int id)
    {
        var task = Tasks.FirstOrDefault(t => t.Id == id);
        if (task == null) return NotFound();

        Tasks.Remove(task);
        return NoContent();
    }
}
