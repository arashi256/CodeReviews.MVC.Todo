 using Microsoft.EntityFrameworkCore;
using TSCA_Todo.Arashi256.Models;

namespace TSCA_Todo.Arashi256.Endpoints
{
    public static class Endpoints
    {
        public static WebApplication MapEndpoints(this WebApplication app)
        {
            RouteGroupBuilder todoItems = app.MapGroup("/todoitems");
            todoItems.MapGet("/", GetAllTodos);
            todoItems.MapGet("/complete", GetCompleteTodos);
            todoItems.MapGet("/{id}", GetTodo);
            todoItems.MapPost("/", CreateTodo);
            todoItems.MapPut("/{id}", UpdateTodo);
            todoItems.MapDelete("/{id}", DeleteTodo);
            return app;
        }

        static async Task<IResult> GetAllTodos(TodoDbContext db)
        {
            return TypedResults.Ok(await db.Todos.Select(x => new TodoItemDto(x)).ToArrayAsync());
        }

        static async Task<IResult> GetCompleteTodos(TodoDbContext db)
        {
            return TypedResults.Ok(await db.Todos.Where(t => t.IsComplete).Select(x => new TodoItemDto(x)).ToListAsync());
        }

        static async Task<IResult> GetTodo(int id, TodoDbContext db)
        {
            var todo = await db.Todos.FindAsync(id);
            return todo is not null ? Results.Ok(new TodoItemDto(todo)) : Results.NotFound();
        }

        static async Task<IResult> CreateTodo(TodoItemDto todoItemDTO, TodoDbContext db)
        {
            var todoItem = new Todo
            {
                IsComplete = todoItemDTO.IsComplete,
                Name = todoItemDTO.Name
            };
            db.Todos.Add(todoItem);
            await db.SaveChangesAsync();
            todoItemDTO = new TodoItemDto(todoItem);
            return TypedResults.Created($"/todoitems/{todoItem.Id}", todoItemDTO);
        }

        static async Task<IResult> UpdateTodo(int id, TodoItemDto todoItemDTO, TodoDbContext db)
        {
            var todo = await db.Todos.FindAsync(id);
            if (todo is null) return TypedResults.NotFound();
            todo.Name = todoItemDTO.Name;
            todo.IsComplete = todoItemDTO.IsComplete;
            await db.SaveChangesAsync();
            return TypedResults.NoContent();
        }

        static async Task<IResult> DeleteTodo(int id, TodoDbContext db)
        {
            if (await db.Todos.FindAsync(id) is Todo todo)
            {
                db.Todos.Remove(todo);
                await db.SaveChangesAsync();
                return TypedResults.NoContent();
            }
            return TypedResults.NotFound();
        }
    }
}