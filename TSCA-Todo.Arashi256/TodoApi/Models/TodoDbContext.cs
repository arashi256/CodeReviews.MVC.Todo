using Microsoft.EntityFrameworkCore;

namespace TSCA_Todo.Arashi256.Models
{
    public class TodoDbContext : DbContext
    {
        public TodoDbContext(DbContextOptions<TodoDbContext> options) : base(options) { }
        public DbSet<Todo> Todos => Set<Todo>();
    }
}