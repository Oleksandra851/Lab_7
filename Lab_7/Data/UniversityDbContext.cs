using Lab_7.Models;
using Microsoft.EntityFrameworkCore;

namespace Lab_7.Data
{
    public class UniversityDbContext : DbContext
    {
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Position> Positions { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<TeacherSubject> TeacherSubjects { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(@"Server=(localdb)\MSSQLLocalDB;Database=UniversityDB;Integrated Security=True;TrustServerCertificate=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TeacherSubject>()
                .HasKey(ts => new { ts.TeacherID, ts.SubjectID });

            base.OnModelCreating(modelBuilder);
        }
    }
}