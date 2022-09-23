using TasksAPI.Data;
using TasksAPI.Models;

namespace TasksAPI.Services
{
    public class TaskFilesService
    {
        private readonly WebApiContext _context;
        private readonly IConfiguration _configuration;
        private readonly TestTasksService _tasksService;

        public TaskFilesService(WebApiContext context, IConfiguration configuration, TestTasksService tasksService)
        {
            _context = context;
            _configuration = configuration;
            _tasksService = tasksService;
        }

        public async Task AttachFilesToTaskAsync(int taskId, List<IFormFile> files)
        {
            if (_tasksService.FindById(taskId) == null)
                throw new Exception($"task with id={taskId} not found");

            foreach (var file in files)
            {
                string uploadPath = _configuration.GetValue<string>("UploadPath");

                Directory.CreateDirectory(Path.Combine(uploadPath, taskId.ToString()));
                string filePath = Path.Combine(uploadPath, $"{taskId}\\{file.FileName}");

                TaskFile taskFile = new TaskFile()
                {
                    TaskId = taskId,
                    Filename = file.FileName
                };

                using (var stream = System.IO.File.Create(filePath))
                {
                    await file.CopyToAsync(stream);
                }

                await _context.TaskFiles.AddAsync(taskFile);
            }
            
            await _context.SaveChangesAsync();
        }

        public async Task DeleteTaskFilesAsync(int taskId)
        {
            TestTask testTask = null;

            testTask = _tasksService.FindById(taskId);

            if (testTask == null)
                throw new Exception($"task with id={taskId} not found");

            if (testTask.Files?.Count > 0)
            {
                string uploadPath = _configuration.GetValue<string>("UploadPath");

                for (int i = 0; i < testTask.Files.Count; i++)
                {
                    string filePath = Path.Combine(uploadPath, $"{taskId}\\{testTask.Files[i].Filename}");

                    if (File.Exists(filePath))
                        File.Delete(filePath);
                    
                    _context.TaskFiles.Remove(testTask.Files[i]);
                }

                await _context.SaveChangesAsync();
            }
        }
    }
}
