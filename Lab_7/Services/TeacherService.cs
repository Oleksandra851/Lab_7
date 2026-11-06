using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lab_7.Data;
using Lab_7.Models;
using Microsoft.EntityFrameworkCore;

namespace Lab_7.Services
{
    public class TeacherService
    {
        public async Task<List<Teacher>> GetTeachersAsync()
        {
            using var context = new UniversityDbContext();
            return await context.Teachers
                .Include(t => t.Position)
                .Include(t => t.TeacherSubjects)
                    .ThenInclude(ts => ts.Subject)
                .ToListAsync();
        }

        public async Task<Teacher?> GetTeacherByIdAsync(int id)
        {
            using var context = new UniversityDbContext();
            return await context.Teachers
                .Include(t => t.Position)
                .Include(t => t.TeacherSubjects)
                    .ThenInclude(ts => ts.Subject)
                .FirstOrDefaultAsync(t => t.TeacherID == id);
        }

        public async Task<List<Position>> GetPositionsAsync()
        {
            using var context = new UniversityDbContext();
            return await context.Positions.ToListAsync();
        }

        public async Task SaveTeacherAsync(Teacher teacher)
        {
            using var context = new UniversityDbContext();
            if (teacher.TeacherID == 0)
            {
                context.Teachers.Add(teacher);
            }
            else
            {
                context.Teachers.Update(teacher);
            }
            await context.SaveChangesAsync();
        }

        public async Task DeleteTeacherAsync(int id)
        {
            using var context = new UniversityDbContext();
            var teacher = await context.Teachers
                .Include(t => t.TeacherSubjects)
                .FirstOrDefaultAsync(t => t.TeacherID == id);

            if (teacher != null)
            {
                if (teacher.TeacherSubjects.Any())
                {
                    context.TeacherSubjects.RemoveRange(teacher.TeacherSubjects);
                }
                context.Teachers.Remove(teacher);
                await context.SaveChangesAsync();
            }
        }
    }
}