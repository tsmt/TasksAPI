using Microsoft.EntityFrameworkCore;
using TasksAPI.Data;
using TasksAPI.Enums;
using TasksAPI.Models;

namespace TasksAPI.Services
{
    public class TestTasksService
    {
        private readonly WebApiContext _context;

        public TestTasksService(WebApiContext context)
        {
            _context = context;
        }

        public async Task<List<TestTask>> GetALlTasks()
        {
            var testTasks = await _context.TestTasks.AsNoTracking().Include(t => t.Files).ToListAsync();
            return testTasks;
        }

        public async Task<TestTask> CreateTestTaskAsync(string taskName, TaskState state)
        {
            if (!String.IsNullOrEmpty(taskName))
            {
                TestTask task = new TestTask()
                {
                    Name = taskName,
                    Date = DateTime.UtcNow,
                    State = state
                };

                _context.TestTasks.Add(task);
                await _context.SaveChangesAsync();

                return task;
            }
            return null;
        }        

        public TestTask FindById(int id)
        {
            return _context.TestTasks.Include(x => x.Files).FirstOrDefault(x => x.Id == id);
        }

        public async Task<TestTask> UpdateTestTaskAsync(TestTask newTestTask)
        {
            TestTask testTask = FindById(newTestTask.Id);

            if (testTask == null)
                throw new Exception($"task with id={newTestTask.Id} not found");

            testTask.Name = newTestTask.Name;
            testTask.Date = newTestTask.Date;
            testTask.State = newTestTask.State;

            _context.TestTasks.Update(testTask);
            await _context.SaveChangesAsync();

            return testTask;
        }

        public async Task DeleteTestTaskAsync(int taskId)
        {
            TestTask testTask = FindById(taskId);

            if (testTask == null)
                throw new Exception($"task with id={taskId} not found");

            //await _taskFilesService.DeleteTaskFilesAsync(taskId);
            _context.TestTasks.Remove(testTask);
            await _context.SaveChangesAsync();
        }
    }
}
