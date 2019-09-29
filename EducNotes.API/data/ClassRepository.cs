using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EducNotes.API.Models;
using Microsoft.EntityFrameworkCore;

namespace EducNotes.API.Data
{
    public class ClassRepository : IClassRepository
    {
        private readonly DataContext _context;

        public ClassRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<Class> GetClass(int Id)
        {
            return await _context.Classes.FirstOrDefaultAsync(c => c.Id == Id);
        }

        public async Task<IEnumerable<Schedule>> GetScheduleDay(int classId, int day)
        {
            return await _context.Schedules
                .Include(i => i.Class)
                .Include(c => c.Course)
                .Where(d => d.Day == day && d.Class.Id == classId)
                .OrderBy(s => s.StartHourMin).ToListAsync();
        }
    }
}