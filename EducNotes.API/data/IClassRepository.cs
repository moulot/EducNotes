using System.Collections.Generic;
using System.Threading.Tasks;
using EducNotes.API.Models;

namespace EducNotes.API.Data
{
    public interface IClassRepository
    {
        Task<Class> GetClass(int Id);
        Task<IEnumerable<Schedule>> GetScheduleDay(int classId, int day);        
    }
}