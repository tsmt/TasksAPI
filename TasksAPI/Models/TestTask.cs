using System.ComponentModel.DataAnnotations;
using TasksAPI.Enums;

namespace TasksAPI.Models
{
    public class TestTask
    {
        public int Id { get; set; }

        public DateTime Date { get; set; }

        [Required]
        [MaxLength(256)]
        public string Name { get; set; }

        [Required]
        public TaskState State { get; set; }

        public List<TaskFile> Files { get; set; }
    }
}
