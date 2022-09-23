using Microsoft.EntityFrameworkCore;
using TasksAPI.Models;

namespace TasksAPI.Data
{
    public class WebApiContext : DbContext
    {
        public DbSet<TestTask> TestTasks { get; set; }
        public DbSet<TaskFile> TaskFiles { get; set; }

        public WebApiContext(DbContextOptions<WebApiContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TestTask>().ToTable("TestTasks").HasKey(t => t.Id);
            modelBuilder.Entity<TaskFile>().ToTable("TaskFiles").HasKey(f => f.Id);

            modelBuilder.Entity<TaskFile>()
                .HasOne(t => t.Task)
                .WithMany(f => f.Files)
                .HasForeignKey(t => t.TaskId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TestTask>()
                .Property(s => s.State)
                .HasConversion<byte>();
        }
    }
}
