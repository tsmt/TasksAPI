using System.ComponentModel.DataAnnotations;

namespace TasksAPI.Models
{
    public class TaskFile
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(512)]
        public string Filename { get; set; }
        public int TaskId { get; set; }
        public TestTask Task { get; set; }
    }
}
