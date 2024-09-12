using Microsoft.EntityFrameworkCore;
using TaskManagement.Models;

namespace TaskManagement.Data
{
    public class TaskDbcontext : DbContext
    {
        public TaskDbcontext(DbContextOptions<TaskDbcontext> options) : base(options)
        {
        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Tasks> Tasks { get; set; }
        public DbSet<EmployeeProject> EmployeeProjects { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // One-to-many relationship between Employee and EmployeeProject
            modelBuilder.Entity<Employee>()
                .HasMany(e => e.EmployeeProjects)
                .WithOne(ep => ep.Employee)
                .HasForeignKey(ep => ep.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);

            // One-to-many relationship between Project and Task
            modelBuilder.Entity<Project>()
                .HasMany(p => p.Tasks)
                .WithOne(t => t.Project)
                .HasForeignKey(t => t.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            // One-to-many relationship between Project and EmployeeProject
            modelBuilder.Entity<Project>()
                .HasMany(p => p.EmployeeProjects)
                .WithOne(ep => ep.Project)
                .HasForeignKey(ep => ep.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            // Change the relationship for Task to avoid cascade paths
            modelBuilder.Entity<EmployeeProject>()
                .HasOne(ep => ep.Task)
                .WithMany()
                .HasForeignKey(ep => ep.TaskId)
                .OnDelete(DeleteBehavior.NoAction);  // Use NO ACTION to prevent cascading delete

            // Composite key for EmployeeProject
            modelBuilder.Entity<EmployeeProject>()
                .HasKey(ep => new { ep.EmployeeId, ep.ProjectId });
        }
    }
}
