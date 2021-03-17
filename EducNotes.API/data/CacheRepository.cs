using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EducNotes.API.Data;
using EducNotes.API.Helpers;
using EducNotes.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace EducNotes.API.data
{
  public class CacheRepository : ICacheRepository
  {
    private readonly DataContext _context;
    private readonly IMemoryCache _cache;

    public CacheRepository(DataContext context, IMemoryCache memoryCache)
    {
      _context = context;
      _cache = memoryCache;
    }

    public async Task<List<User>> GetUsers()
    {
      List<User> users = new List<User>();

      // Look for cache key.
      if (!_cache.TryGetValue(CacheKeys.Users, out users))
      {
        // Key not in cache, so get data.
        users = await LoadUsers();

        // Set cache options.
        var cacheEntryOptions = new MemoryCacheEntryOptions()
            // Keep in cache for this time, reset time if accessed.
            .SetSlidingExpiration(TimeSpan.FromDays(7));

        // Save data in cache.
        _cache.Set(CacheKeys.Users, users, cacheEntryOptions);
      }

      return users;
    }

    public async Task<List<User>> LoadUsers()
    {
      List<User> users = await _context.Users
              .Include(p => p.Photos)
              .Include(c => c.Class)
              .Include(i => i.EducLevel)
              .ToListAsync();

      // Set cache options.
      var cacheEntryOptions = new MemoryCacheEntryOptions()
          // Keep in cache for this time, reset time if accessed.
          .SetSlidingExpiration(TimeSpan.FromDays(7));

      // Save data in cache.
      _cache.Remove(CacheKeys.Users);
      _cache.Set(CacheKeys.Users, users, cacheEntryOptions);

      return users;
    }

    public async Task<List<TeacherCourse>> GetTeacherCourses()
    {
      List<TeacherCourse> teacherCourses = new List<TeacherCourse>();

      // Look for cache key.
      if (!_cache.TryGetValue(CacheKeys.TeacherCourses, out teacherCourses))
      {
        // Key not in cache, so get data.
        teacherCourses = await LoadTeacherCourses();

        // Set cache options.
        var cacheEntryOptions = new MemoryCacheEntryOptions()
            // Keep in cache for this time, reset time if accessed.
            .SetSlidingExpiration(TimeSpan.FromDays(7));

        // Save data in cache.
        _cache.Set(CacheKeys.TeacherCourses, teacherCourses, cacheEntryOptions);
      }

      return teacherCourses;
    }

    public async Task<List<TeacherCourse>> LoadTeacherCourses()
    {
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

    public async Task<List<ClassCourse>> GetClassCourses()
    {
      List<ClassCourse> classCourses = new List<ClassCourse>();

      // Look for cache key.
      if (!_cache.TryGetValue(CacheKeys.ClassCourses, out classCourses))
      {
        // Key not in cache, so get data.
        classCourses = await LoadClassCourses();

        // Set cache options.
        var cacheEntryOptions = new MemoryCacheEntryOptions()
            // Keep in cache for this time, reset time if accessed.
            .SetSlidingExpiration(TimeSpan.FromDays(7));

        // Save data in cache.
        _cache.Set(CacheKeys.ClassCourses, classCourses, cacheEntryOptions);
      }

      return classCourses;
    }

    public async Task<List<ClassCourse>> LoadClassCourses()
    {
      List<ClassCourse> classcourses = await _context.ClassCourses
                                          .Include(p => p.Class)
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
  }
}