using Microsoft.EntityFrameworkCore;
using TSCA_Todo.Arashi256.Endpoints;
using TSCA_Todo.Arashi256.Models;

var builder = WebApplication.CreateBuilder(args);
// Check if we are running tests
var isTesting = builder.Environment.EnvironmentName == "Testing";
if (!isTesting)
{
    builder.Services.AddDbContext<TodoDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("ConnectionString") ?? throw new InvalidOperationException("Connection string not found.")));
}
var app = builder.Build();
app.UseDefaultFiles(); // Serve index.html when navigating to "/"
app.UseStaticFiles(); // Enable wwwroot files
app.MapEndpoints();
app.Run();

public partial class Program { }