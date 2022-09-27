using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using TasksAPI.Models;
using TasksAPI.Services;

namespace TasksAPI.Controllers
{
    public class TaskFilesController : Controller
    {
        private readonly ILogger<TestTasksController> _logger;
        private readonly TaskFilesService _filesService;

        public TaskFilesController(ILogger<TestTasksController> logger, TaskFilesService filesService)
        {
            _logger = logger;
            _filesService = filesService;
        }

        /// <summary>
        /// Добавляет файлы к задаче
        /// </summary>
        /// <param name="taskId">Id задачи</param>
        /// <param name="files">список файлов</param>
        /// <returns>результат выполнения запроса</returns>
        [HttpPost("AttachFilesToTask")]
        [RequestSizeLimit(long.MaxValue)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AttachFilesToTask(int taskId, [Required] List<IFormFile> files)
        {
            if (files?.Count() > 0)
            {
                List<TaskFile> result;

                try
                {
                    result = await _filesService.AttachFilesToTaskAsync(taskId, files);
                    return Ok(result);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
            else
            {
                return BadRequest("no file to upload");
            }
        }

        /// <summary>
        /// Удаление всех файлов из задачи
        /// </summary>
        /// <param name="taskId">Id задачи</param>
        /// <returns></returns>
        [HttpDelete("DeleteTaskFiles")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteTaskFiles(int taskId)
        {
            try
            {
                await _filesService.DeleteTaskFilesAsync(taskId);
                return Ok("all files removed");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Скачивание файла
        /// </summary>
        /// <param name="fileId">id файла</param>
        /// <returns></returns>
        [HttpGet("DownloadFile")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DownloadFile(int fileId)
        {
            try
            {
                TaskFile taskFile = await _filesService.DownloadFile(fileId);
                return File(taskFile.ContentBytes, taskFile.ContentType, taskFile.FilePath);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
