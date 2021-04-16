using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EducNotes.API.data;
using EducNotes.API.Models;
using Microsoft.EntityFrameworkCore;

namespace EducNotes.API.Data
{
  public class ClassRepository : IClassRepository
  {
    private readonly DataContext _context;
    public readonly ICacheRepository _cache;

    public ClassRepository(DataContext context, ICacheRepository cache)
    {
      _cache = cache;
      _context = context;
    }

    public async Task<Class> GetClass(int Id)
    {
      return await _context.Classes.FirstOrDefaultAsync(c => c.Id == Id);
    }

    public async Task<IEnumerable<Schedule>> GetScheduleDay(int classId, int day)
    {
      List<Schedule> schedules = await _cache.GetSchedules();
      return schedules.Where(d => d.Day == day && d.Class.Id == classId)
                      .OrderBy(s => s.StartHourMin).ToList();
    }
  }
}