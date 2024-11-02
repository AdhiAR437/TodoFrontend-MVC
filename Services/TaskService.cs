
//using System.Net.Http.Json;
//using System.Threading.Tasks;
//using TodoFrontend_MVC.Models;
//using System.Net.Http;
//using System.Text.Json;


//namespace TodoFrontend_MVC.Services
//{
//    public class TaskService
//    {
//        private readonly HttpClient _httpClient;

//        public TaskService(HttpClient httpClient)
//        {
//            _httpClient = httpClient;
//            _httpClient.BaseAddress = new Uri("https://localhost:7110/");  // Backend API base address
//        }

//        public async Task<HttpResponseMessage> AddTaskAsync(TasksModel task)
//        {
//            return await _httpClient.PostAsJsonAsync("api/Task", task);
//        }

//        public async Task<HttpResponseMessage> DeleteTaskAsync(int id)
//        {
//            return await _httpClient.DeleteAsync($"api/Task/{id}");
//        }

//        public async Task<HttpResponseMessage> UpdateTaskAsync(int id, TasksModel task)
//        {
//            return await _httpClient.PutAsJsonAsync($"api/Task/{id}", task);
//        }


//        //public async Task<HttpResponseMessage> MarkTaskDoneAsync(int id)
//        //{
//        //    return await _httpClient.PutAsync($"api/Task/mark-done/{id}", null);
//        //}

//        public async Task<bool> ToggleTaskStatusAsync(int taskId)
//        {
//            var response = await _httpClient.PutAsync($"api/Task/toggle-status/{taskId}", null);

//            return response.IsSuccessStatusCode;
//        }
//        //public async Task<List<TasksModel>> GetTasksByUserIdAsync(int userId)
//        //{
//        //    var response = await _httpClient.GetAsync($"api/Task/{userId}");

//        //    if (response.IsSuccessStatusCode)
//        //    {
//        //        var json = await response.Content.ReadAsStringAsync();
//        //        return JsonSerializer.Deserialize<List<TasksModel>>(json);
//        //    }

//        //    return new List<TasksModel>();
//        //}

//        public async Task<List<TasksModel>> GetTasksByUserIdAsync(int userId)
//        {
//            return await _httpClient.GetFromJsonAsync<List<TasksModel>>($"api/Task/{userId}");
//        }

//    }
//}






using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using System.Text.Json;
using TodoFrontend_MVC.Models;

namespace TodoFrontend_MVC.Services
{
    public class TaskService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<TaskService> _logger; // Add a logger
        public TaskService(HttpClient httpClient, ILogger<TaskService> logger)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://localhost:7110/api/Task/");  // Backend API base address
            _logger = logger;
        }

        public async Task<HttpResponseMessage> AddTaskAsync(TasksModel task)
        {
            return await _httpClient.PostAsJsonAsync("", task);
        }

        public async Task<HttpResponseMessage> DeleteTaskAsync(int id)
        {
            return await _httpClient.DeleteAsync($"{id}");
        }

        //public async Task<HttpResponseMessage> UpdateTaskAsync(int id, TasksModel task)
        //{
        //    return await _httpClient.PutAsJsonAsync($"{id}", task);
        //}

       
        public async Task<bool> UpdateTaskAsync(TasksModel updatedTask)
        {
            // Ensure all fields in `updatedTask` are populated, especially the `Id`
            if (updatedTask == null || updatedTask.Id == 0)
            {
                // Handle error or log an issue for debugging
                return false;
            }

            var response = await _httpClient.PutAsJsonAsync($"byId/{updatedTask.Id}", updatedTask);
            return response.IsSuccessStatusCode;
        }


        public async Task<bool> ToggleTaskStatusAsync(int taskId)
        {
            var response = await _httpClient.PutAsync($"toggle-status/{taskId}", null);
            return response.IsSuccessStatusCode;
        }

        //public async Task<List<TasksModel>> GetTasksByUserIdAsync(int userId)
        //{
        //    return await _httpClient.GetFromJsonAsync<List<TasksModel>>($"{userId}");
        //}

        public async Task<List<TasksModel>> GetTasksByUserIdAsync(int userId)
        {
            try
            {
                _logger.LogInformation($"Fetching tasks for user ID: {userId}"); // Log the userId

                var response = await _httpClient.GetFromJsonAsync<List<TasksModel>>($"{userId}");

                _logger.LogInformation("Tasks fetched successfully."); // Log success

                return response;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError($"HTTP Request Error: {ex.Message}"); // Log the HTTP error
                throw; // Rethrow the exception if you want to handle it higher up
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while fetching tasks: {ex.Message}"); // Log any other exceptions
                throw; // Rethrow the exception
            }
        }

        public async Task<TasksModel> GetTaskByIdAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<TasksModel>($"byId/{id}");
        }

    }
}
