using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using TasksAPI.Enums;
using TasksAPI.Extensions;
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

        public TestTasksController(ILogger<TestTasksController> logger, TestTasksService taskService)
        {
            _logger = logger;
            _taskService = taskService;
        }

        /// <summary>
        /// Получение постраничного списка всех задач
        /// </summary>
        /// <param name="pageNum">номер страницы</param>
        /// <param name="pageSize">по сколько задач выводим</param>
        /// <returns></returns>
        [HttpGet("GetAllTasks")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllTasks(int pageNum = 1, int pageSize = 10)
        {
            //IQueryable<TestTask> query = _taskService.GetALlTasks();
            //List<TestTask> testTasks = await PagedList<TestTask>.ToPagedListAsync(query, pageNum, pageSize);

            var testTasks = await _taskService.GetALlTasks(pageNum, pageSize);
            return Ok(testTasks);
        }

        /// <summary>
        /// Получает задачу
        /// </summary>
        /// <param name="taskId">id задачи</param>
        /// <returns>возвращает задачу</returns>
        [HttpGet("GetTask")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetTask(int taskId)
        {
            TestTask testTask = await _taskService.FindById(taskId);
            
            if (testTask != null)
                return Ok(testTask);
            return BadRequest($"task with id={taskId} not found");
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
    }
}
