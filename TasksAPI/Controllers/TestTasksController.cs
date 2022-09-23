﻿using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using TasksAPI.Enums;
using TasksAPI.Models;
using TasksAPI.Services;

namespace TasksAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestTasksController : ControllerBase
    {
        private readonly ILogger<TestTasksController> _logger;
        private readonly TestTasksService _taskService;
        private readonly TaskFilesService _filesService;

        public TestTasksController(ILogger<TestTasksController> logger, 
            TestTasksService taskService, TaskFilesService filesService)
        {
            _logger = logger;
            _taskService = taskService;
            _filesService = filesService;
        }


        /// <summary>
        /// Получение списка всех задач
        /// </summary>
        /// <returns>список задач</returns>
        [HttpGet("GetAllTasks")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllTasks()
        {
            List<TestTask> testTasks = await _taskService.GetALlTasks();
            return Ok(testTasks);
        }


        /// <summary>
        /// Создание задачи
        /// </summary>
        /// <param name="Name">наименование</param>
        /// <param name="state">состояние</param>
        /// <returns></returns>
        [HttpPost("CreateTask")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateTestTask(string Name, TaskState state)
        {
            var newTask = await _taskService.CreateTestTaskAsync(Name, state);
            return Ok(newTask);
        }


        /// <summary>
        /// Обновление задачи
        /// </summary>
        /// <param name="testTask"></param>
        /// <returns></returns>
        [HttpPut("UpdateTask")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateTestTask([FromBody] TestTask testTask)
        {
            try
            {
                TestTask tt = await _taskService.UpdateTestTaskAsync(testTask);
                return Ok(tt);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Удаление задачи
        /// </summary>
        /// <param name="taskId">Id задачи</param>
        /// <returns></returns>
        [HttpDelete("DeleteTask")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteTask(int taskId)
        {
            try
            {
                await _taskService.DeleteTestTaskAsync(taskId);
                return Ok($"task id={taskId} removed");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
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
                try
                {
                    await _filesService.AttachFilesToTaskAsync(taskId, files);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }

                return Ok($"{files.Count} files uploaded");
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
    }
}