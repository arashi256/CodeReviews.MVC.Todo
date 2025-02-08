using System.Text.Json;
using TSCA_Todo.Arashi256.Models;
using System.Text;

namespace TodoTests
{
    [TestClass]
    public class TestTodoApi
    {
        private TestWebApplicationFactory _factory = null!;
        private HttpClient _client = null!;

        [TestInitialize]
        public void Initialize()
        {
            _factory = new TestWebApplicationFactory();
            _client = _factory.CreateClient();
        }

        [TestCleanup]
        public void Cleanup()
        {
            if (_client != null) _client.Dispose();
            if (_factory != null) _factory.Dispose();
        }

        // GET /todoitems (GetAllTodos)
        [TestMethod]
        public async Task GetAllTodos_ShouldReturnAllTodos()
        {
            // Act
            var response = await _client.GetAsync("/todoitems");
            // Assert
            Assert.IsTrue(response.IsSuccessStatusCode);
            var responseContent = await response.Content.ReadAsStringAsync();
            var todos = JsonSerializer.Deserialize<List<TodoItemDto>>(responseContent);
            Assert.IsNotNull(todos);
            Assert.IsTrue(todos.Count == 3);
        }

        // POST /todoitems
        [TestMethod]
        public async Task PostTodo_ShouldCreateNewTodo()
        {
            // Arrange
            var newTodo = new { Name = "New Test Todo 4", IsComplete = false };
            var content = new StringContent(JsonSerializer.Serialize(newTodo), Encoding.UTF8, "application/json");
            // Act
            var response = await _client.PostAsync("/todoitems", content);
            // Assert
            Assert.IsTrue(response.IsSuccessStatusCode);
            var responseString = await response.Content.ReadAsStringAsync();
            Assert.IsTrue(responseString.Contains("New Test Todo 4")); // Check if the todo was created
        }

        // GET /todoitems/complete
        [TestMethod]
        public async Task GetCompleteTodos_ShouldReturnOnlyCompletedTodos()
        {
            // Act
            var response = await _client.GetAsync("/todoitems/complete");
            var responseContent = await response.Content.ReadAsStringAsync();
            var completedTodos = JsonSerializer.Deserialize<List<TodoItemDto>>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            // Assert
            Assert.IsTrue(response.IsSuccessStatusCode, $"Expected success but got {response.StatusCode}");
            Assert.IsNotNull(completedTodos, "Deserialized object is null");
            Assert.IsTrue(completedTodos.Any(), "Expected at least one completed todo but got an empty list.");
            Assert.IsTrue(completedTodos.All(t => t.IsComplete), "Not all returned todos are completed.");
        }

        // DELETE /todoitems/{id}
        [TestMethod]
        public async Task DeleteTodo_ShouldRemoveTodo()
        {
            // Arrange
            var todoId = 1; // This ID exists in the seeded data
            // Act
            var response = await _client.DeleteAsync($"/todoitems/{todoId}");
            // Assert
            Assert.IsTrue(response.IsSuccessStatusCode);
            // Verify the deletion
            var getResponse = await _client.GetAsync($"/todoitems/{todoId}");
            Assert.AreEqual(System.Net.HttpStatusCode.NotFound, getResponse.StatusCode);
        }

        // GET /todoitems/{id}
        [TestMethod]
        public async Task GetTodo_ShouldReturnSpecificTodo()
        {
            // Arrange
            var todoId = 1; // This ID should exist in the seeded data.
            // Act
            var response = await _client.GetAsync($"/todoitems/{todoId}");
            var responseContent = await response.Content.ReadAsStringAsync();
            // Deserialize response
            var todo = JsonSerializer.Deserialize<TodoItemDto>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            // Assert
            Assert.IsTrue(response.IsSuccessStatusCode, $"Expected success but got {response.StatusCode}");
            Assert.IsNotNull(todo, "Deserialized object is null");
            Assert.AreEqual("Test Todo 1", todo.Name, $"Expected 'Test Todo 1' but got '{todo?.Name}'");
        }

        // PUT /todoitems/{id}
        [TestMethod]
        public async Task UpdateTodo_ShouldModifyTodo()
        {
            // Arrange
            var todoId = 2; // This ID should exist in seeded data
            var updatedTodo = new { Id = todoId, Name = "Updated Test Todo", IsComplete = true };
            var content = new StringContent(JsonSerializer.Serialize(updatedTodo), Encoding.UTF8, "application/json");
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            // Act
            var response = await _client.PutAsync($"/todoitems/{todoId}", content);
            var responseContent = await response.Content.ReadAsStringAsync();
            // Assert Update Request
            Assert.IsTrue(response.IsSuccessStatusCode, $"Expected success but got {response.StatusCode}");
            var getResponse = await _client.GetAsync($"/todoitems/{todoId}");
            var getResponseContent = await getResponse.Content.ReadAsStringAsync();
            // Deserialize response
            var todo = JsonSerializer.Deserialize<TodoItemDto>(getResponseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            // Assert
            Assert.IsNotNull(todo, "Deserialized object is null");
            Assert.AreEqual("Updated Test Todo", todo.Name, $"Expected 'Updated Test Todo' but got '{todo?.Name}'");
            Assert.IsTrue(todo.IsComplete, "Expected 'IsComplete' to be true");
        }
    }
}