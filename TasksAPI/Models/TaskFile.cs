using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TasksAPI.Models
{
    public class TaskFile
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(512)]
        public string Filename { get; set; }
        [MaxLength(50)]
        public string ContentType { get; set; }
        [NotMapped]
        public byte[] ContentBytes { get; set; }
        [NotMapped]
        [MaxLength(1024)]
        public string FilePath { get; set; }
        public int TaskId { get; set; }
        public TestTask Task { get; set; }
    }
}
