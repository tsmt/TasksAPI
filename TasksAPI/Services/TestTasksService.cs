using Microsoft.EntityFrameworkCore;
using TasksAPI.Data;
using TasksAPI.Enums;
using TasksAPI.Extensions;
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
        public async Task<PagedList<TestTask>> GetALlTasks(int pageNum, int pageSize)
        {
            var testTasks = _context.TestTasks.AsNoTracking().Include(t => t.Files);
            PagedList<TestTask> list = new PagedList<TestTask>();
            var result = await list.ToPagedListAsync(testTasks, pageNum, pageSize);
            return result;
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

        public async Task<TestTask> FindById(int id)
        {
            TestTask testTask = await _context.TestTasks.Include(x => x.Files).FirstOrDefaultAsync(x => x.Id == id);
            return testTask;
        }

        public async Task<TestTask> UpdateTestTaskAsync(TestTask newTestTask)
        {
            TestTask testTask = await FindById(newTestTask.Id);

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
            TestTask testTask = await FindById(taskId);

            if (testTask == null)
                throw new Exception($"task with id={taskId} not found");

            //await _taskFilesService.DeleteTaskFilesAsync(taskId);
            _context.TestTasks.Remove(testTask);
            await _context.SaveChangesAsync();
        }
    }
}
