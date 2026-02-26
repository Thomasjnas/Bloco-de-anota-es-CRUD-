using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi.Data;
using TodoApi.Models;

namespace TodoApi.Controllers;

[ApiController]
[Route("api/todo")]
public class TodoController : ControllerBase
{
    private readonly AppDbContext _db;

    // Injeção de dependência: o ASP.NET cria o AppDbContext pra gente.
    public TodoController(AppDbContext db)
    {
        _db = db;
    }

    // GET /api/todo -> lista todas as tarefas
    [HttpGet]
    public async Task<ActionResult<List<TodoItem>>> GetAll()
    {
        // AsNoTracking = mais rápido quando você só vai ler
        var tasks = await _db.Tasks.AsNoTracking().OrderByDescending(t => t.Id).ToListAsync();
        return Ok(tasks);
    }

    // POST /api/todo -> cria tarefa
    [HttpPost]
    public async Task<ActionResult<TodoItem>> Create([FromBody] TodoItem newTask)
    {
        if (string.IsNullOrWhiteSpace(newTask.Title))
            return BadRequest("Title não pode ser vazio.");

        newTask.Done = false;

        _db.Tasks.Add(newTask);            // adiciona no "contexto"
        await _db.SaveChangesAsync();      // salva no banco de verdade

        return Created($"/api/todo/{newTask.Id}", newTask);
    }

    // PUT /api/todo/{id} -> alterna Done
    [HttpPut("{id}")]
    public async Task<ActionResult<TodoItem>> ToggleDone(int id)
    {
        var task = await _db.Tasks.FirstOrDefaultAsync(t => t.Id == id);
        if (task == null) return NotFound();

        task.Done = !task.Done;            // altera em memória
        await _db.SaveChangesAsync();      // persiste no banco

        return Ok(task);
    }

    // PUT /api/todo/{id}/title -> edita o título
    [HttpPut("{id}/title")]
    public async Task<ActionResult<TodoItem>> UpdateTitle(int id, [FromBody] TodoItem updated)
    {
        var task = await _db.Tasks.FirstOrDefaultAsync(t => t.Id == id);
        if (task == null) return NotFound();

        if (string.IsNullOrWhiteSpace(updated.Title))
            return BadRequest("Title não pode ser vazio.");

        task.Title = updated.Title;
        await _db.SaveChangesAsync();

        return Ok(task);
    }

    // DELETE /api/todo/{id} -> remove tarefa
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var task = await _db.Tasks.FirstOrDefaultAsync(t => t.Id == id);
        if (task == null) return NotFound();

        _db.Tasks.Remove(task);
        await _db.SaveChangesAsync();

        return NoContent();
    }
}