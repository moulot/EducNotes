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
    public string subDomain;
      private CacheKeys ck;


    public CacheRepository(DataContext context, IConfiguration config, IMemoryCache memoryCache)
    {
      _config = config;
      _context = context;
      _cache = memoryCache;
      teacherTypeId = _config.GetValue<int>("AppSettings:teacherTypeId");
      parentTypeId = _config.GetValue<int>("AppSettings:parentTypeId");
      adminTypeId = _config.GetValue<int>("AppSettings:adminTypeId");
      studentTypeId = _config.GetValue<int>("AppSettings:studentTypeId");
      ck = new CacheKeys(context);
    }

    public async Task<List<User>> GetUsers()
    {
      List<User> users = new List<User>();

      // Look for cache key.
      if(!_cache.TryGetValue(ck.Users, out users))
      {
        // Key not in cache, so get data.
        users = await LoadUsers();
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

    public async Task<List<User>> LoadUsers()
    {
      List<User> users = await _context.Users
                              .Include(p => p.Photos)
                              .Include(c => c.Class)
                              .Include(i => i.EducLevel)
                              .Include(c => c.ClassLevel)
                              .Include(c => c.District)
                              .Include(c => c.City)
                              .Include(i => i.UserType)
                              .ToListAsync();

      // Set cache options.
      var cacheEntryOptions = new MemoryCacheEntryOptions ()
        // Keep in cache for this time, reset time if accessed.
        .SetSlidingExpiration (TimeSpan.FromDays (7));

      // Save data in cache.
       _cache.Remove(ck.Users);
       _cache.Set(ck.Users, users, cacheEntryOptions);

      return users;
    }

    public async Task<List<TeacherCourse>> GetTeacherCourses() {
      List<TeacherCourse> teacherCourses = new List<TeacherCourse>();

      // Look for cache key.
      if(!_cache.TryGetValue(ck.TeacherCourses, out teacherCourses)) {
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
      _cache.Remove(ck.TeacherCourses);
      _cache.Set(ck.TeacherCourses, teachercourses, cacheEntryOptions);

      return teachercourses;
    }

    public async Task<List<ClassCourse>> GetClassCourses()
    {
      List<ClassCourse> classCourses = new List<ClassCourse>();

      // Look for cache key.
      if(!_cache.TryGetValue (ck.ClassCourses, out classCourses)) {
        // Key not in cache, so get data.
        classCourses = await LoadClassCourses();
      }

      return classCourses;
    }

    public async Task<List<ClassCourse>> LoadClassCourses()
    {
      List<ClassCourse> classcourses = await _context.ClassCourses
        .Include(i => i.Teacher).ThenInclude(i => i.Photos)
        .Include(i => i.Class).ThenInclude(i => i.ClassLevel).ThenInclude(i => i.EducationLevel)
        .Include(i => i.Course)
        .Include(i => i.Class)
        .ToListAsync();

      // Set cache options.
      var cacheEntryOptions = new MemoryCacheEntryOptions()
        // Keep in cache for this time, reset time if accessed.
        .SetSlidingExpiration(TimeSpan.FromDays(7));

      // Save data in cache.
      _cache.Remove(ck.ClassCourses);
      _cache.Set(ck.ClassCourses, classcourses, cacheEntryOptions);

      return classcourses;
    }

    public async Task<List<ClassLevel>> GetClassLevels()
    {
      List<ClassLevel> classLevels = new List<ClassLevel>();

      // Look for cache key.
      if (!_cache.TryGetValue(ck.ClassLevels, out classLevels))
      {
        // Key not in cache, so get data.
        classLevels = await LoadClassLevels();
      }

      return classLevels;
    }

    public async Task<List<ClassLevel>> LoadClassLevels () {
      List<ClassLevel> classLevels = await _context.ClassLevels
                                            .Include(i => i.Inscriptions)
                                            .OrderBy(o => o.DsplSeq)
                                            .ToListAsync();

      // Set cache options.
      var cacheEntryOptions = new MemoryCacheEntryOptions ()
        // Keep in cache for this time, reset time if accessed.
        .SetSlidingExpiration (TimeSpan.FromDays (7));

      // Save data in cache.
      _cache.Remove (ck.ClassLevels);
      _cache.Set (ck, classLevels, cacheEntryOptions);

      return classLevels;
    }

    public async Task<List<ClassLevelProduct>> GetClassLevelProducts()
    {
      List<ClassLevelProduct> levelproducts = new List<ClassLevelProduct>();

      // Look for cache key.
      if (!_cache.TryGetValue(ck.ClassLevelProducts, out levelproducts))
      {
        // Key not in cache, so get data.
        levelproducts = await LoadClassLevelProducts();
      }

      return levelproducts;
    }

    public async Task<List<ClassLevelProduct>> LoadClassLevelProducts ()
    {
      List<ClassLevelProduct> levelproducts = await _context.ClassLevelProducts.ToListAsync();

      // Set cache options.
      var cacheEntryOptions = new MemoryCacheEntryOptions ()
        // Keep in cache for this time, reset time if accessed.
        .SetSlidingExpiration (TimeSpan.FromDays (7));

      // Save data in cache.
      _cache.Remove (ck.ClassLevelProducts);
      _cache.Set (ck.ClassLevelProducts, levelproducts, cacheEntryOptions);

      return levelproducts;
    }

    public async Task<List<Class>> GetClasses()
    {
      List<Class> classes = new List<Class>();

      // Look for cache key.
      if (!_cache.TryGetValue (ck.Classes, out classes))
      {
        // Key not in cache, so get data.
        classes = await LoadClasses();
      }

      return classes;
    }

    public async Task<List<Class>> LoadClasses()
    {
      List<Class> classes = await _context.Classes
                                  .Include(i => i.ClassType)
                                  .Include(i => i.ClassLevel).ThenInclude(i => i.EducationLevel)
                                  .OrderBy(o => o.Name).ToListAsync();

      // Set cache options.
      var cacheEntryOptions = new MemoryCacheEntryOptions()
        // Keep in cache for this time, reset time if accessed.
        .SetSlidingExpiration(TimeSpan.FromDays (7));

      // Save data in cache.
      _cache.Remove (ck.Classes);
      _cache.Set (ck.Classes, classes, cacheEntryOptions);

      return classes;
    }

    public async Task<List<EducationLevel>> GetEducLevels()
    {
      List<EducationLevel> educLevels = new List<EducationLevel>();

      // Look for cache key.
      if(!_cache.TryGetValue (ck.EducLevels, out educLevels))
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
      _cache.Remove (ck.EducLevels);
      _cache.Set(ck.EducLevels, educLevels, cacheEntryOptions);

      return educLevels;
    }

    public async Task<List<School>> GetSchools()
    {
      List<School> schools = new List<School>();
      // Look for cache key.
      if (!_cache.TryGetValue (ck.Schools, out schools)) {
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
      _cache.Remove(ck.Schools);
      _cache.Set(ck.Schools, schools, cacheEntryOptions);

      return schools;
    }

    public async Task<List<Cycle>> GetCycles()
    {
      List<Cycle> cycles = new List<Cycle>();
      // Look for cache key.
      if (!_cache.TryGetValue (ck.Cycles, out cycles))
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
      _cache.Remove(ck.Cycles);
      _cache.Set(ck.Cycles, cycles, cacheEntryOptions);

      return cycles;
    }

    public async Task<List<Course>> GetCourses()
    {
      List<Course> courses = new List<Course>();
      // Look for cache key.
      if (!_cache.TryGetValue (ck.Courses, out courses))
      {
        // Key not in cache, so get data.
        courses = await LoadCourses();
      }
      return courses;
    }

    public async Task<List<Course>> LoadCourses()
    {
      List<Course> courses = await _context.Courses.OrderBy(o => o.Name).ToListAsync();

      // Set cache options.
      var cacheEntryOptions = new MemoryCacheEntryOptions()
        // Keep in cache for this time, reset time if accessed.
        .SetSlidingExpiration(TimeSpan.FromDays(7));
      
      // Save data in cache.
      _cache.Remove(ck.Courses);
      _cache.Set(ck.Courses, courses, cacheEntryOptions);

      return courses;
    }

    public async Task<List<ClassType>> GetClassTypes()
    {
      List<ClassType> classtypes = new List<ClassType>();
      // Look for cache key.
      if (!_cache.TryGetValue(ck.ClassTypes, out classtypes))
      {
        // Key not in cache, so get data.
        classtypes = await LoadClassTypes();
      }
      return classtypes;
    }

    public async Task<List<ClassType>> LoadClassTypes()
    {
      List<ClassType> classtypes = await _context.ClassTypes.OrderBy(o => o.Name).ToListAsync();

      // Set cache options.
      var cacheEntryOptions = new MemoryCacheEntryOptions()
        // Keep in cache for this time, reset time if accessed.
        .SetSlidingExpiration(TimeSpan.FromDays(7));
      
      // Save data in cache.
      _cache.Remove(ck.ClassTypes);
      _cache.Set(ck.ClassTypes, classtypes, cacheEntryOptions);

      return classtypes;
    }

    public async Task<List<ClassLevelClassType>> GetCLClassTypes()
    {
      List<ClassLevelClassType> clclasstypes = new List<ClassLevelClassType>();
      // Look for cache key.
      if (!_cache.TryGetValue(ck.CLClassTypes, out clclasstypes))
      {
        // Key not in cache, so get data.
        clclasstypes = await LoadCLClassTypes();
      }
      return clclasstypes;
    }

    public async Task<List<ClassLevelClassType>> LoadCLClassTypes()
    {
      List<ClassLevelClassType> clclasstypes = await _context.ClassLevelClassTypes
                                                    .Include(i => i.ClassType)
                                                    .Include(i => i.ClassLevel)
                                                    .OrderBy(o => o.ClassType.Name).ToListAsync();

      // Set cache options.
      var cacheEntryOptions = new MemoryCacheEntryOptions()
        // Keep in cache for this time, reset time if accessed.
        .SetSlidingExpiration(TimeSpan.FromDays(7));
      
      // Save data in cache.
      _cache.Remove(ck.CLClassTypes);
      _cache.Set(ck.CLClassTypes, clclasstypes, cacheEntryOptions);

      return clclasstypes;
    }

    public async Task<List<EmailTemplate>> GetEmailTemplates()
    {
      List<EmailTemplate> templates = new List<EmailTemplate>();
      // Look for cache key.
      if (!_cache.TryGetValue(ck.EmailTemplates, out templates))
      {
        // Key not in cache, so get data.
        templates = await LoadEmailTemplates();
      }
      return templates;
    }

    public async Task<List<EmailTemplate>> LoadEmailTemplates()
    {
      List<EmailTemplate> templates = await _context.EmailTemplates.OrderBy(o => o.Name).ToListAsync();

      // Set cache options.
      var cacheEntryOptions = new MemoryCacheEntryOptions()
        // Keep in cache for this time, reset time if accessed.
        .SetSlidingExpiration(TimeSpan.FromDays(7));
      
      // Save data in cache.
      _cache.Remove(ck.EmailTemplates);
      _cache.Set(ck.EmailTemplates, templates, cacheEntryOptions);

      return templates;
    }

    public async Task<List<SmsTemplate>> GetSmsTemplates()
    {
      List<SmsTemplate> templates = new List<SmsTemplate>();
      // Look for cache key.
      if (!_cache.TryGetValue(ck.SmsTemplates, out templates))
      {
        // Key not in cache, so get data.
        templates = await LoadSmsTemplates();
      }
      return templates;
    }

    public async Task<List<SmsTemplate>> LoadSmsTemplates()
    {
      List<SmsTemplate> templates = await _context.SmsTemplates.OrderBy(o => o.Name).ToListAsync();

      // Set cache options.
      var cacheEntryOptions = new MemoryCacheEntryOptions()
        // Keep in cache for this time, reset time if accessed.
        .SetSlidingExpiration(TimeSpan.FromDays(7));
      
      // Save data in cache.
      _cache.Remove(ck.SmsTemplates);
      _cache.Set(ck.SmsTemplates, templates, cacheEntryOptions);

      return templates;
    }

    public async Task<List<Setting>> GetSettings()
    {
      List<Setting> settings = new List<Setting>();
      // Look for cache key.
      if (!_cache.TryGetValue(ck.Settings, out settings))
      {
        // Key not in cache, so get data.
        settings = await LoadSettings();
      }
      return settings;
    }

    public async Task<List<Setting>> LoadSettings()
    {
      List<Setting> settings = await _context.Settings.OrderBy(o => o.DisplayName).ToListAsync();

      // Set cache options.
      var cacheEntryOptions = new MemoryCacheEntryOptions()
        // Keep in cache for this time, reset time if accessed.
        .SetSlidingExpiration(TimeSpan.FromDays(7));
      
      // Save data in cache.
      _cache.Remove(ck.Settings);
      _cache.Set(ck.Settings, settings, cacheEntryOptions);

      return settings;
    }

    public async Task<List<Token>> GetTokens()
    {
      List<Token> tokens = new List<Token>();
      // Look for cache key.
      if (!_cache.TryGetValue(ck.Tokens, out tokens))
      {
        // Key not in cache, so get data.
        tokens = await LoadTokens();
      }
      return tokens;
    }

    public async Task<List<Token>> LoadTokens()
    {
      List<Token> tokens = await _context.Tokens.OrderBy(o => o.Name).ToListAsync();

      // Set cache options.
      var cacheEntryOptions = new MemoryCacheEntryOptions()
        // Keep in cache for this time, reset time if accessed.
        .SetSlidingExpiration(TimeSpan.FromDays(7));
      
      // Save data in cache.
      _cache.Remove(ck.Tokens);
      _cache.Set(ck.Tokens, tokens, cacheEntryOptions);

      return tokens;
    }

    public async Task<List<ProductDeadLine>> GetProductDeadLines()
    {
      List<ProductDeadLine> productdeadlines = new List<ProductDeadLine>();
      // Look for cache key.
      if (!_cache.TryGetValue(ck.ProductDeadLines, out productdeadlines))
      {
        // Key not in cache, so get data.
        productdeadlines = await LoadProductDeadLines();
      }
      return productdeadlines;
    }

    public async Task<List<ProductDeadLine>> LoadProductDeadLines()
    {
      List<ProductDeadLine> productdeadlines = await _context.ProductDeadLines.ToListAsync();

      // Set cache options.
      var cacheEntryOptions = new MemoryCacheEntryOptions()
        // Keep in cache for this time, reset time if accessed.
        .SetSlidingExpiration(TimeSpan.FromDays(7));
      
      // Save data in cache.
      _cache.Remove(ck.ProductDeadLines);
      _cache.Set(ck.ProductDeadLines, productdeadlines, cacheEntryOptions);

      return productdeadlines;
    }

    public async Task<List<Role>> GetRoles()
    {
      List<Role> roles = new List<Role>();

      // Look for cache key.
      if(!_cache.TryGetValue(ck.Roles, out roles))
      {
        // Key not in cache, so get data.
        roles = await LoadRoles();
      }

      return roles;
    }

    public async Task<List<Role>> LoadRoles()
    {
      List<Role> roles = await _context.Roles.OrderBy(o => o.Name).ToListAsync();

      // Set cache options.
      var cacheEntryOptions = new MemoryCacheEntryOptions()
        // Keep in cache for this time, reset time if accessed.
        .SetSlidingExpiration(TimeSpan.FromDays(7));

      // Save data in cache.
      _cache.Remove(ck.Roles);
      _cache.Set(ck.Roles, roles, cacheEntryOptions);

      return roles;
    }

    public async Task<List<Order>> GetOrders()
    {
      List<Order> orders = new List<Order>();

      // Look for cache key.
      if(!_cache.TryGetValue(ck.Orders, out orders))
      {
        // Key not in cache, so get data.
        orders = await LoadOrders();
      }

      return orders;
    }

    public async Task<List<Order>> LoadOrders()
    {
      List<Order> orders = await _context.Orders
                                  .Include(i => i.Child)
                                  .Include (i => i.Mother).ThenInclude(i => i.UserType)
                                  .Include (i => i.Father).ThenInclude(i => i.UserType)
                                  .ToListAsync();

      // Set cache options.
      var cacheEntryOptions = new MemoryCacheEntryOptions()
        // Keep in cache for this time, reset time if accessed.
        .SetSlidingExpiration(TimeSpan.FromDays(7));

      // Save data in cache.
      _cache.Remove(ck.Orders);
      _cache.Set(ck.Orders, orders, cacheEntryOptions);

      return orders;
    }

    public async Task<List<OrderLine>> GetOrderLines()
    {
      List<OrderLine> lines = new List<OrderLine>();

      // Look for cache key.
      if(!_cache.TryGetValue(ck.OrderLines, out lines))
      {
        // Key not in cache, so get data.
        lines = await LoadOrderLines();
      }

      return lines;
    }

    public async Task<List<OrderLine>> LoadOrderLines()
    {
      List<OrderLine> lines = await _context.OrderLines
                                    .Include(i => i.Order)
                                    .Include(i => i.Product)
                                    .Include(i => i.Child).ThenInclude(i => i.Photos)
                                    .Include(i => i.ClassLevel)
                                    .Include(i => i.Product)
                                    .ToListAsync();

      // Set cache options.
      var cacheEntryOptions = new MemoryCacheEntryOptions()
        // Keep in cache for this time, reset time if accessed.
        .SetSlidingExpiration(TimeSpan.FromDays(7));

      // Save data in cache.
      _cache.Remove(ck.OrderLines);
      _cache.Set(ck.OrderLines, lines, cacheEntryOptions);

      return lines;
    }

    public async Task<List<OrderLineDeadline>> GetOrderLineDeadLines()
    {
      List<OrderLineDeadline> linedeadlines = new List<OrderLineDeadline>();

      // Look for cache key.
      if(!_cache.TryGetValue(ck.OrderLineDeadLines, out linedeadlines))
      {
        // Key not in cache, so get data.
        linedeadlines = await LoadOrderLineDeadLines();
      }

      return linedeadlines;
    }

    public async Task<List<OrderLineDeadline>> LoadOrderLineDeadLines()
    {
      List<OrderLineDeadline> linedeadlines = await _context.OrderLineDeadlines
                                                    .Include(i => i.OrderLine)
                                                    .ToListAsync();

      // Set cache options.
      var cacheEntryOptions = new MemoryCacheEntryOptions()
        // Keep in cache for this time, reset time if accessed.
        .SetSlidingExpiration(TimeSpan.FromDays(7));

      // Save data in cache.
      _cache.Remove(ck.OrderLineDeadLines);
      _cache.Set(ck.OrderLineDeadLines, linedeadlines, cacheEntryOptions);

      return linedeadlines;
    }

    public async Task<List<UserLink>> GetUserLinks()
    {
      List<UserLink> userlinks = new List<UserLink>();

      // Look for cache key.
      if(!_cache.TryGetValue(ck.UserLinks, out userlinks))
      {
        // Key not in cache, so get data.
        userlinks = await LoadUserLinks();
      }

      return userlinks;
    }

    public async Task<List<UserLink>> LoadUserLinks()
    {
      List<UserLink> userlinks = await _context.UserLinks.ToListAsync();

      // Set cache options.
      var cacheEntryOptions = new MemoryCacheEntryOptions()
        // Keep in cache for this time, reset time if accessed.
        .SetSlidingExpiration(TimeSpan.FromDays(7));

      // Save data in cache.
      _cache.Remove(ck.UserLinks);
      _cache.Set(ck.UserLinks, userlinks, cacheEntryOptions);

      return userlinks;
    }

    public async Task<List<FinOp>> GetFinOps()
    {
      List<FinOp> finops = new List<FinOp>();

      // Look for cache key.
      if(!_cache.TryGetValue(ck.FinOps, out finops))
      {
        // Key not in cache, so get data.
        finops = await LoadFinOps();
      }

      return finops;
    }

    public async Task<List<FinOp>> LoadFinOps()
    {
      List<FinOp> finops = await _context.FinOps
                                  .Include(i => i.Cheque).ThenInclude(i => i.Bank)
                                  .Include(i => i.PaymentType)
                                  .Include(i => i.Invoice)
                                  .Include(i => i.FromBank)
                                  .Include(i => i.FromBankAccount)
                                  .Include(i => i.FromCashDesk)
                                  .Include(i => i.ToBankAccount)
                                  .Include(i => i.ToCashDesk)
                                  .ToListAsync();

      // Set cache options.
      var cacheEntryOptions = new MemoryCacheEntryOptions()
        // Keep in cache for this time, reset time if accessed.
        .SetSlidingExpiration(TimeSpan.FromDays(7));

      // Save data in cache.
      _cache.Remove(ck.FinOps);
      _cache.Set(ck.FinOps, finops, cacheEntryOptions);

      return finops;
    }

    public async Task<List<FinOpOrderLine>> GetFinOpOrderLines()
    {
      List<FinOpOrderLine> finoporderlines = new List<FinOpOrderLine>();

      // Look for cache key.
      if(!_cache.TryGetValue(ck.FinOpOrderLines, out finoporderlines))
      {
        // Key not in cache, so get data.
        finoporderlines = await LoadFinOpOrderLines();
      }

      return finoporderlines;
    }

    public async Task<List<FinOpOrderLine>> LoadFinOpOrderLines()
    {
      List<FinOpOrderLine> finoporderlines = await _context.FinOpOrderLines
                                                    .Include(d => d.Invoice)
                                                    .Include(o => o.OrderLine).ThenInclude(p => p.Product)
                                                    .Include(o => o.OrderLine).ThenInclude(c => c.Child)
                                                    .Include(i => i.FinOp).ThenInclude(i => i.Cheque).ThenInclude(i => i.Bank)
                                                    .Include(i => i.FinOp).ThenInclude(i => i.PaymentType)
                                                    .Include(i => i.FinOp).ThenInclude(i => i.FromBankAccount)
                                                    .Include(i => i.FinOp).ThenInclude(i => i.ToBankAccount)
                                                    .Include(i => i.FinOp).ThenInclude(i => i.FromCashDesk)
                                                    .Include(i => i.FinOp).ThenInclude(i => i.ToCashDesk)
                                                    .ToListAsync();

      // Set cache options.
      var cacheEntryOptions = new MemoryCacheEntryOptions()
        // Keep in cache for this time, reset time if accessed.
        .SetSlidingExpiration(TimeSpan.FromDays(7));

      // Save data in cache.
      _cache.Remove(ck.FinOpOrderLines);
      _cache.Set(ck.FinOpOrderLines, finoporderlines, cacheEntryOptions);

      return finoporderlines;
    }

    public async Task<List<Cheque>> GetCheques()
    {
      List<Cheque> cheques = new List<Cheque>();

      // Look for cache key.
      if(!_cache.TryGetValue(ck.Cheques, out cheques))
      {
        // Key not in cache, so get data.
        cheques = await LoadCheques();
      }

      return cheques;
    }

    public async Task<List<Cheque>> LoadCheques()
    {
      List<Cheque> cheques = await _context.Cheques.ToListAsync();

      // Set cache options.
      var cacheEntryOptions = new MemoryCacheEntryOptions()
        // Keep in cache for this time, reset time if accessed.
        .SetSlidingExpiration(TimeSpan.FromDays(7));

      // Save data in cache.
      _cache.Remove(ck.Cheques);
      _cache.Set(ck.Cheques, cheques, cacheEntryOptions);

      return cheques;
    }

    public async Task<List<Bank>> GetBanks()
    {
      List<Bank> banks = new List<Bank>();

      // Look for cache key.
      if(!_cache.TryGetValue(ck.Banks, out banks))
      {
        // Key not in cache, so get data.
        banks = await LoadBanks();
      }

      return banks;
    }

    public async Task<List<Bank>> LoadBanks()
    {
      List<Bank> banks = await _context.Banks.OrderBy(o => o.Name).ToListAsync();

      // Set cache options.
      var cacheEntryOptions = new MemoryCacheEntryOptions()
        // Keep in cache for this time, reset time if accessed.
        .SetSlidingExpiration(TimeSpan.FromDays(7));

      // Save data in cache.
      _cache.Remove(ck.Banks);
      _cache.Set(ck.Banks, banks, cacheEntryOptions);

      return banks;
    }

    public async Task<List<PaymentType>> GetPaymentTypes()
    {
      List<PaymentType> paymentTypes = new List<PaymentType>();

      // Look for cache key.
      if(!_cache.TryGetValue(ck.PaymentTypes, out paymentTypes))
      {
        // Key not in cache, so get data.
        paymentTypes = await LoadPaymentTypes();
      }

      return paymentTypes;
    }

    public async Task<List<PaymentType>> LoadPaymentTypes()
    {
      List<PaymentType> paymentTypes = await _context.PaymentTypes.OrderBy(o => o.Name).ToListAsync();

      // Set cache options.
      var cacheEntryOptions = new MemoryCacheEntryOptions()
        // Keep in cache for this time, reset time if accessed.
        .SetSlidingExpiration(TimeSpan.FromDays(7));

      // Save data in cache.
      _cache.Remove(ck.PaymentTypes);
      _cache.Set(ck.PaymentTypes, paymentTypes, cacheEntryOptions);

      return paymentTypes;
    }

    public async Task<List<Product>> GetProducts()
    {
      List<Product> products = new List<Product>();

      // Look for cache key.
      if(!_cache.TryGetValue(ck.Products, out products))
      {
        // Key not in cache, so get data.
        products = await LoadProducts();
      }

      return products;
    }

    public async Task<List<Product>> LoadProducts()
    {
      List<Product> products = await _context.Products.OrderBy(o => o.Name).ToListAsync();

      // Set cache options.
      var cacheEntryOptions = new MemoryCacheEntryOptions()
        // Keep in cache for this time, reset time if accessed.
        .SetSlidingExpiration(TimeSpan.FromDays(7));

      // Save data in cache.
      _cache.Remove(ck.Products);
      _cache.Set(ck.Products, products, cacheEntryOptions);

      return products;
    }

    public async Task<List<UserType>> GetUserTypes()
    {
      List<UserType> usertypes = new List<UserType>();

      // Look for cache key.
      if(!_cache.TryGetValue(ck.UserTypes, out usertypes))
      {
        // Key not in cache, so get data.
        usertypes = await LoadUserTypes();
      }

      return usertypes;
    }

    public async Task<List<UserType>> LoadUserTypes()
    {
      List<UserType> usertypes = await _context.UserTypes
                                        .Include(i => i.Users)
                                        .ToListAsync();

      // Set cache options.
      var cacheEntryOptions = new MemoryCacheEntryOptions()
        // Keep in cache for this time, reset time if accessed.
        .SetSlidingExpiration(TimeSpan.FromDays(7));

      // Save data in cache.
      _cache.Remove(ck.UserTypes);
      _cache.Set(ck.UserTypes, usertypes, cacheEntryOptions);

      return usertypes;
    }
  }
}