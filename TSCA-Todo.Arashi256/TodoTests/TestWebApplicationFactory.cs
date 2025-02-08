using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TSCA_Todo.Arashi256.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection.Extensions;

public class TestWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptors = services.Where(d => d.ServiceType == typeof(DbContextOptions<TodoDbContext>)).ToList();
            foreach (var descriptor in descriptors)
            {
                services.Remove(descriptor);
            }
            services.RemoveAll(typeof(TodoDbContext));
            services.AddDbContext<TodoDbContext>(options => options.UseInMemoryDatabase("TODO"));
            using var scope = services.BuildServiceProvider().CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<TodoDbContext>();
            SeedTestData(context);
        });
    }

    private static void SeedTestData(TodoDbContext context)
    {
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();
        if (!context.Todos.Any())
        {
            context.Todos.AddRange(
                new Todo { Id = 1, Name = "Test Todo 1", IsComplete = false },
                new Todo { Id = 2, Name = "Test Todo 2", IsComplete = true },
                new Todo { Id = 3, Name = "Test Todo 3", IsComplete = false }
            );
            context.SaveChanges();
        }
    }
}