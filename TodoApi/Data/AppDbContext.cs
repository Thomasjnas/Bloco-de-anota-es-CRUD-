using Microsoft.EntityFrameworkCore;
using TodoApi.Models;

namespace TodoApi.Data;

// DbContext = a "sessão" do EF com o banco.
// Ele sabe quais tabelas existem (DbSet) e como mapear.
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // DbSet = representa uma "tabela" no EF.
    // No banco ela vira uma tabela (ex: tasks).
    public DbSet<TodoItem> Tasks => Set<TodoItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Mapeamento explícito (profissional) para controlar nome de tabela/colunas.
        modelBuilder.Entity<TodoItem>(entity =>
        {
            // Nome da tabela no PostgreSQL
            entity.ToTable("tasks");

            // Chave primária
            entity.HasKey(t => t.Id);

            // Colunas
            entity.Property(t => t.Id)
                  .HasColumnName("id");

            entity.Property(t => t.Title)
                  .HasColumnName("title")
                  .IsRequired();

            entity.Property(t => t.Done)
                  .HasColumnName("done")
                  .HasDefaultValue(false);
        });
    }
}