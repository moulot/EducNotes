using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EducNotes.API.Data;
using EducNotes.API.Helpers;
using EducNotes.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;

namespace EducNotes.API.data {
  public class CacheRepository : ICacheRepository {
    private readonly DataContext _context;
    private readonly IMemoryCache _cache;
    int teacherTypeId, parentTypeId, studentTypeId, adminTypeId;
    public readonly IConfiguration _config;

    public CacheRepository (DataContext context, IConfiguration config, IMemoryCache memoryCache) {
      _config = config;
      _context = context;
      _cache = memoryCache;
      teacherTypeId = _config.GetValue<int> ("AppSettings:teacherTypeId");
      parentTypeId = _config.GetValue<int> ("AppSettings:parentTypeId");
      adminTypeId = _config.GetValue<int> ("AppSettings:adminTypeId");
      studentTypeId = _config.GetValue<int> ("AppSettings:studentTypeId");
    }

    public async Task<List<User>> GetUsers () {
      List<User> users = new List<User> ();

      // Look for cache key.
      if (!_cache.TryGetValue (CacheKeys.Users, out users)) {
        // Key not in cache, so get data.
        users = await LoadUsers ();
      }

      return users;
    }

    public async Task<List<User>> GetStudents() {
      List<User> students = (await GetUsers()).Where(u => u.UserTypeId == studentTypeId).ToList();
      return students;
    }

    public async Task<List<User>> GetParents() {
      List<User> parents = (await GetUsers()).Where(u => u.UserTypeId == parentTypeId).ToList();
      return parents;
    }

    public async Task<List<User>> GetTeachers() {
      List<User> teachers = (await GetUsers()).Where(u => u.UserTypeId == teacherTypeId).ToList();
      return teachers;
    }

    public async Task<List<User>> LoadUsers () {
      List<User> users = await _context.Users
        .Include (p => p.Photos)
        .Include (c => c.Class)
        .Include (i => i.EducLevel)
        .Include (c => c.ClassLevel)
        .Include (c => c.District)
        .Include (c => c.City)
        .Include (i => i.UserType)
        .ToListAsync ();

      // Set cache options.
      var cacheEntryOptions = new MemoryCacheEntryOptions ()
        // Keep in cache for this time, reset time if accessed.
        .SetSlidingExpiration (TimeSpan.FromDays (7));

      // Save data in cache.
      _cache.Remove (CacheKeys.Users);
      _cache.Set (CacheKeys.Users, users, cacheEntryOptions);

      return users;
    }

    public async Task<List<TeacherCourse>> GetTeacherCourses() {
      List<TeacherCourse> teacherCourses = new List<TeacherCourse>();

      // Look for cache key.
      if(!_cache.TryGetValue(CacheKeys.TeacherCourses, out teacherCourses)) {
        // Key not in cache, so get data.
        teacherCourses = await LoadTeacherCourses();
      }

      return teacherCourses;
    }

    public async Task<List<TeacherCourse>> LoadTeacherCourses() {
      List<TeacherCourse> teachercourses = await _context.TeacherCourses
                                                  .Include(p => p.Course)
                                                  .ToListAsync();

      // Set cache options.
      var cacheEntryOptions = new MemoryCacheEntryOptions()
        // Keep in cache for this time, reset time if accessed.
        .SetSlidingExpiration(TimeSpan.FromDays(7));

      // Save data in cache.
      _cache.Remove(CacheKeys.TeacherCourses);
      _cache.Set(CacheKeys.TeacherCourses, teachercourses, cacheEntryOptions);

      return teachercourses;
    }

    public async Task<List<ClassCourse>> GetClassCourses() {
      List<ClassCourse> classCourses = new List<ClassCourse>();

      // Look for cache key.
      if(!_cache.TryGetValue (CacheKeys.ClassCourses, out classCourses)) {
        // Key not in cache, so get data.
        classCourses = await LoadClassCourses();
      }

      return classCourses;
    }

    public async Task<List<ClassCourse>> LoadClassCourses() {
      List<ClassCourse> classcourses = await _context.ClassCourses
        // .Include(i => i.Teacher).ThenInclude(i => i.Photos)
        // .Include(i => i.Class).ThenInclude(i => i.ClassLevel).ThenInclude(i => i.EducationLevel)
        // .Include(i => i.Class).ThenInclude(i => i.Students)
        // .Include(i => i.Course)
        .Include(i => i.Class)
        .ToListAsync();

      // Set cache options.
      var cacheEntryOptions = new MemoryCacheEntryOptions()
        // Keep in cache for this time, reset time if accessed.
        .SetSlidingExpiration(TimeSpan.FromDays(7));

      // Save data in cache.
      _cache.Remove(CacheKeys.ClassCourses);
      _cache.Set(CacheKeys.ClassCourses, classcourses, cacheEntryOptions);

      return classcourses;
    }

    public async Task<List<ClassLevel>> GetClassLevels () {
      List<ClassLevel> classLevels = new List<ClassLevel> ();

      // Look for cache key.
      if (!_cache.TryGetValue(CacheKeys.ClassLevels, out classLevels)) {
        // Key not in cache, so get data.
        classLevels = await LoadClassLevels();
      }

      return classLevels;
    }

    public async Task<List<ClassLevel>> LoadClassLevels () {
      List<ClassLevel> classLevels = await _context.ClassLevels.ToListAsync();

      // Set cache options.
      var cacheEntryOptions = new MemoryCacheEntryOptions ()
        // Keep in cache for this time, reset time if accessed.
        .SetSlidingExpiration (TimeSpan.FromDays (7));

      // Save data in cache.
      _cache.Remove (CacheKeys.ClassLevels);
      _cache.Set (CacheKeys.ClassLevels, classLevels, cacheEntryOptions);

      return classLevels;
    }

    public async Task<List<Class>> GetClasses() {
      List<Class> classes = new List<Class>();

      // Look for cache key.
      if (!_cache.TryGetValue (CacheKeys.Classes, out classes)) {
        // Key not in cache, so get data.
        classes = await LoadClasses();
      }

      return classes;
    }

    public async Task<List<Class>> LoadClasses() {
      List<Class> classes = await _context.Classes
        .OrderBy(o => o.Name)
        .ToListAsync();

      // Set cache options.
      var cacheEntryOptions = new MemoryCacheEntryOptions()
        // Keep in cache for this time, reset time if accessed.
        .SetSlidingExpiration(TimeSpan.FromDays (7));

      // Save data in cache.
      _cache.Remove (CacheKeys.Classes);
      _cache.Set (CacheKeys.Classes, classes, cacheEntryOptions);

      return classes;
    }

    public async Task<List<EducationLevel>> GetEducLevels()
    {
      List<EducationLevel> educLevels = new List<EducationLevel>();

      // Look for cache key.
      if(!_cache.TryGetValue (CacheKeys.EducLevels, out educLevels))
      {
        // Key not in cache, so get data.
        educLevels = await LoadEducLevels();
      }

      return educLevels;
    }

    public async Task<List<EducationLevel>> LoadEducLevels()
    {
      List<EducationLevel> educLevels = await _context.EducationLevels
                                              .OrderBy(o => o.Name)
                                              .ToListAsync();

      // Set cache options.
      var cacheEntryOptions = new MemoryCacheEntryOptions()
        // Keep in cache for this time, reset time if accessed.
        .SetSlidingExpiration(TimeSpan.FromDays(7));

      // Save data in cache.
      _cache.Remove (CacheKeys.EducLevels);
      _cache.Set (CacheKeys.EducLevels, educLevels, cacheEntryOptions);

      return educLevels;
    }

    public async Task<List<School>> GetSchools()
    {
      List<School> schools = new List<School>();
      // Look for cache key.
      if (!_cache.TryGetValue (CacheKeys.Schools, out schools)) {
        // Key not in cache, so get data.
        schools = await LoadSchools();
      }
      return schools;
    }

    public async Task<List<School>> LoadSchools()
    {
      List<School> schools = await _context.Schools
                                              .OrderBy(o => o.Name)
                                              .ToListAsync();

      // Set cache options.
      var cacheEntryOptions = new MemoryCacheEntryOptions()
        // Keep in cache for this time, reset time if accessed.
        .SetSlidingExpiration(TimeSpan.FromDays(7));

      // Save data in cache.
      _cache.Remove(CacheKeys.Schools);
      _cache.Set(CacheKeys.Schools, schools, cacheEntryOptions);

      return schools;
    }

    public async Task<List<Cycle>> GetCycles()
    {
      List<Cycle> cycles = new List<Cycle>();
      // Look for cache key.
      if (!_cache.TryGetValue (CacheKeys.Cycles, out cycles))
      {
        // Key not in cache, so get data.
        cycles = await LoadCycles();
      }
      return cycles;
    }

    public async Task<List<Cycle>> LoadCycles()
    {
      List<Cycle> cycles = await _context.Cycles.OrderBy(o => o.Name).ToListAsync();

      // Set cache options.
      var cacheEntryOptions = new MemoryCacheEntryOptions()
        // Keep in cache for this time, reset time if accessed.
        .SetSlidingExpiration(TimeSpan.FromDays(7));
      
      // Save data in cache.
      _cache.Remove(CacheKeys.Cycles);
      _cache.Set(CacheKeys.Cycles, cycles, cacheEntryOptions);

      return cycles;
    }
  }
}