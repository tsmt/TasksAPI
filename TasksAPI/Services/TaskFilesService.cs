using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
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

        public async Task<List<TaskFile>> AttachFilesToTaskAsync(int taskId, List<IFormFile> files)
        {
            if (_tasksService.FindById(taskId) == null)
                throw new Exception($"task with id={taskId} not found");

            List<TaskFile> result = new List<TaskFile>();

            foreach (var file in files)
            {
                string uploadPath = _configuration.GetValue<string>("UploadPath");

                Directory.CreateDirectory(Path.Combine(uploadPath, taskId.ToString()));
                string filePath = Path.Combine(uploadPath, $"{taskId}\\{file.FileName}");

                TaskFile taskFile = new TaskFile()
                {
                    TaskId = taskId,
                    Filename = file.FileName,
                    ContentType = file.ContentType
                };

                using (var stream = System.IO.File.Create(filePath))
                {
                    await file.CopyToAsync(stream);
                }

                await _context.TaskFiles.AddAsync(taskFile);
                result.Add(taskFile);
            }
            
            await _context.SaveChangesAsync();
            return result;
        }

        public async Task DeleteTaskFilesAsync(int taskId)
        {
            TestTask testTask = null;

            testTask = await _tasksService.FindById(taskId);

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

        public async Task<TaskFile> DownloadFile(int fileId)
        {
            TaskFile taskFile = await FindById(fileId);
            if (taskFile == null)
                throw new Exception($"file with id={fileId} not found");

            string uploadPath = _configuration.GetValue<string>("UploadPath");
            string filePath = Path.Combine(uploadPath, $"{taskFile.TaskId}\\{taskFile.Filename}");

            if (String.IsNullOrEmpty(taskFile.ContentType))
            {
                var provider = new FileExtensionContentTypeProvider();
                if (!provider.TryGetContentType(filePath, out var contentType))
                {
                    taskFile.ContentType = "application/octet-stream";
                }
            }

            taskFile.ContentBytes = await File.ReadAllBytesAsync(filePath);
            taskFile.FilePath = Path.GetFileName(filePath);
            return taskFile;
        }

        public async Task<TaskFile> FindById(int fileId)
        {
            TaskFile taskFile = await _context.TaskFiles.FirstOrDefaultAsync(f => f.Id == fileId);
            return taskFile;
        }
    }
}
