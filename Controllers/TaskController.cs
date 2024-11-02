//using Microsoft.AspNetCore.Mvc;
//using System.Threading.Tasks;
//using TodoFrontend_MVC.Models;
//using TodoFrontend_MVC.Services;

//namespace TodoFrontend_MVC.Controllers
//{
//    public class TaskController : Controller
//    {
//        private readonly TaskService _taskService;

//        public TaskController(TaskService taskService)
//        {
//            _taskService = taskService;
//        }

//        //public async Task<IActionResult> GetTasks(int userId)
//        //{
//        //    var tasks = await _taskService.GetTasksByUserIdAsync(userId);
//        //    return View(tasks);
//        //}
//        public async Task<IActionResult> GetTasks()
//        {
//            // Retrieve UserId from session
//            int? userId = HttpContext.Session.GetInt32("UserId");

//            if (userId == null)
//            {
//                // Redirect to login if session expired or user is not logged in
//                return RedirectToAction("Login", "User");
//            }

//            // Fetch tasks for the logged-in user
//            var tasks = await _taskService.GetTasksByUserIdAsync(userId.Value);
//            ViewData["UserId"] = userId; // Pass UserId to the view for any additional actions

//            return View(tasks);
//        }



//        [HttpPost]
//        public async Task<IActionResult> ToggleTaskStatus(int id, int userId)
//        {
//            await _taskService.ToggleTaskStatusAsync(id);
//            return RedirectToAction("GetTasks", new { userId = userId });
//        }


//        [HttpGet]
//        public IActionResult AddTask()
//        {
//            // Create a new instance of TasksModel
//            var taskModel = new TasksModel
//            {
//                UserId = HttpContext.Session.GetInt32("UserId") ??0// Default to 0 if not set
//            };
//            return View(taskModel); // Pass the model to the view
//        }


//        [HttpPost]
//        public async Task<IActionResult> AddTask(TasksModel task)
//        {
//            if (!ModelState.IsValid)
//            {
//                // Log validation errors
//                var errors = ModelState.Values.SelectMany(v => v.Errors)
//                                                .Select(e => e.ErrorMessage)
//                                                .ToList();
//                foreach (var error in errors)
//                {
//                    Console.WriteLine(error); // or use your logging mechanism
//                }
//                return View(task); // Return view if there are validation errors
//            }

//            // Retrieve UserId from session and set it on the task model
//            int? userId = HttpContext.Session.GetInt32("UserId");

//            if (userId == null)
//            {
//                return RedirectToAction("Login", "User"); // Redirect to login if session expired or user is not logged in
//            }

//            task.UserId = userId.Value;
//            task.Status = false;  // Default task status to "incomplete"
//            task.CreatedDate = DateTime.Now;
//            task.CompletedDate = DateTime.Now;

//            var response = await _taskService.AddTaskAsync(task);
//            if (response.IsSuccessStatusCode)
//            {
//                return RedirectToAction("GetTasks", new { userId = task.UserId });
//            }

//            ModelState.AddModelError(string.Empty, "Failed to add task.");
//            return View(task);
//        }

//        [HttpGet]
//        public IActionResult UpdateTask()
//        {
//            return View(); // Displays the Add Task form
//        }

//        public async Task<IActionResult> UserTasks(int userId)
//        {
//            // Call the service to get tasks by user ID
//            var tasks = await _taskService.GetTasksByUserIdAsync(userId);

//            // Pass the tasks to the view for rendering
//            return View(tasks);
//        }






//        [HttpPost]
//        public async Task<IActionResult> DeleteTask(int id, int userId)
//        {
//            var response = await _taskService.DeleteTaskAsync(id);
//            if (response.IsSuccessStatusCode)
//            {
//                return RedirectToAction("GetTasks", new { userId });
//            }
//            ModelState.AddModelError(string.Empty, "Failed to delete task.");
//            return RedirectToAction("GetTasks", new { userId });
//        }

//        [HttpPost]
//        public async Task<IActionResult> UpdateTask(int id, TasksModel task)
//        {
//            if (!ModelState.IsValid)
//            {
//                return View(task); // Return view if there are validation errors
//            }

//            var response = await _taskService.UpdateTaskAsync(id, task);
//            if (response.IsSuccessStatusCode)
//            {
//                return RedirectToAction("GetTasks", new { userId = task.UserId });
//            }

//            ModelState.AddModelError(string.Empty, "Failed to update task.");
//            return View(task);
//        }

//        public async Task<IActionResult> EditTask(int id)
//        {
//            // Fetch the task by ID from the service
//            var tasks = await _taskService.GetTasksByUserIdAsync(id); // Adjust as needed
//            if (tasks == null)
//            {
//                return NotFound(); // Task not found
//            }
//            return View(tasks); // Return the task model to the edit view
//        }




//        //[HttpPost]
//        //public async Task<IActionResult> MarkTaskDone(int id, int userId)
//        //{
//        //    var response = await _taskService.MarkTaskDoneAsync(id);
//        //    if (response.IsSuccessStatusCode)
//        //    {
//        //        return RedirectToAction("GetTasks", new { userId });
//        //    }

//        //    ModelState.AddModelError(string.Empty, "Failed to mark task as done.");
//        //    return RedirectToAction("GetTasks", new { userId });
//        //}

//    }
//}





using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TodoFrontend_MVC.Models;
using TodoFrontend_MVC.Services;

namespace TodoFrontend_MVC.Controllers
{
    public class TaskController : Controller
    {
        private readonly TaskService _taskService;
        private readonly ILogger<TaskController> _logger;

        public TaskController(TaskService taskService, ILogger<TaskController> logger)
        {
            _taskService = taskService;
            _logger = logger;
        }

        //public async Task<IActionResult> GetTasks()
        //{
        //    int? userId = HttpContext.Session.GetInt32("UserId");

        //    if (userId == null)
        //    {
        //        return RedirectToAction("Login", "User");
        //    }

        //    var tasks = await _taskService.GetTasksByUserIdAsync(userId.Value);
        //    ViewData["UserId"] = userId;
        //    return View(tasks);
        //}



        public async Task<IActionResult> GetTasks()
        {
            try
            {
                var userId = HttpContext.Session.GetInt32("UserId");

                if (!userId.HasValue)
                {
                    _logger.LogWarning("User ID not found in session.");
                    ModelState.AddModelError("", "User not logged in.");
                    return RedirectToAction("Login");
                }

                _logger.LogInformation($"Getting tasks for user ID: {userId.Value}");

                var tasks = await _taskService.GetTasksByUserIdAsync(userId.Value);

                // Ensure tasks is not null; initialize to an empty list if it is
                if (tasks == null)
                {
                    _logger.LogWarning("No tasks found for user ID: {UserId}", userId.Value);
                    tasks = new List<TasksModel>(); // Initialize to an empty list
                }

                return View(tasks);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError($"HTTP Request Error: {ex.Message}");
                ModelState.AddModelError("", "Unable to fetch tasks at this time.");
                return View(new List<TasksModel>()); // Return an empty list to avoid null reference
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while getting tasks: {ex.Message}");
                ModelState.AddModelError("", "An unexpected error occurred.");
                return View(new List<TasksModel>()); // Return an empty list to avoid null reference
            }
        }



        [HttpPost]
        public async Task<IActionResult> ToggleTaskStatus(int id)
        {
            bool isSuccess = await _taskService.ToggleTaskStatusAsync(id);

            if (isSuccess)
            {
                return RedirectToAction("GetTasks");
            }

            ModelState.AddModelError(string.Empty, "Failed to toggle task status.");
            return RedirectToAction("GetTasks");
        }

        [HttpGet]
        public IActionResult AddTask()
        {
            var taskModel = new TasksModel
            {
                UserId = HttpContext.Session.GetInt32("UserId") ?? 0
            };
            return View(taskModel);
        }

        [HttpPost]
        public async Task<IActionResult> AddTask(TasksModel task)
        {
            if (!ModelState.IsValid)
            {
                return View(task);
            }

            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login", "User");
            }

            task.UserId = userId.Value;
            task.Status = false;
            task.CreatedDate = DateTime.Now;

            var response = await _taskService.AddTaskAsync(task);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("GetTasks");
            }

            ModelState.AddModelError(string.Empty, "Failed to add task.");
            return View(task);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var response = await _taskService.DeleteTaskAsync(id);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("GetTasks");
            }

            ModelState.AddModelError(string.Empty, "Failed to delete task.");
            return RedirectToAction("GetTasks");
        }

        [HttpGet]
        public async Task<IActionResult> EditTask(int id)
        {
            var task = await _taskService.GetTaskByIdAsync(id);
            if (task == null)
            {
                return NotFound();
            }
            return View(task);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateTask(int id, TasksModel task)
        {
            if (!ModelState.IsValid)
            {
                return View(task);
            }

            var response = await _taskService.UpdateTaskAsync(id, task);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("GetTasks");
            }

            ModelState.AddModelError(string.Empty, "Failed to update task.");
            return View(task);
        }
    }
}
