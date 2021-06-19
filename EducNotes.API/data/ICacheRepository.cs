using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EducNotes.API.Models;

namespace EducNotes.API.data
{
  public interface ICacheRepository
  {
    Task<List<User>> GetUsers();
    Task<List<User>> GetStudents();
    Task<List<User>> GetParents();
    Task<List<User>> GetTeachers();
    Task<List<User>> GetEmployees();
    Task<List<User>> LoadUsers();
    Task<List<TeacherCourse>> GetTeacherCourses();
    Task<List<TeacherCourse>> LoadTeacherCourses();
    Task<List<ClassCourse>> GetClassCourses();
    Task<List<ClassCourse>> LoadClassCourses();
    Task<List<ClassLevel>> GetClassLevels();
    Task<List<ClassLevel>> LoadClassLevels();
    Task<List<ClassLevelProduct>> GetClassLevelProducts();
    Task<List<ClassLevelProduct>> LoadClassLevelProducts();
    Task<List<Class>> GetClasses();
    Task<List<Class>> LoadClasses();
    Task<List<EducationLevel>> GetEducLevels();
    Task<List<EducationLevel>> LoadEducLevels();
    Task<List<School>> GetSchools();
    Task<List<School>> LoadSchools();
    Task<List<Cycle>> GetCycles();
    Task<List<Cycle>> LoadCycles();
    Task<List<Course>> GetCourses();
    Task<List<Course>> LoadCourses();
    Task<List<ClassType>> GetClassTypes();
    Task<List<ClassType>> LoadClassTypes();
    Task<List<ClassLevelClassType>> GetCLClassTypes();
    Task<List<ClassLevelClassType>> LoadCLClassTypes();
    Task<List<EmailTemplate>> GetEmailTemplates();
    Task<List<EmailTemplate>> LoadEmailTemplates();
    Task<List<SmsTemplate>> GetSmsTemplates();
    Task<List<SmsTemplate>> LoadSmsTemplates();
    Task<List<Setting>> GetSettings();
    Task<List<Setting>> LoadSettings();
    Task<List<Token>> GetTokens();
    Task<List<Token>> LoadTokens();
    Task<List<ProductDeadLine>> GetProductDeadLines();
    Task<List<ProductDeadLine>> LoadProductDeadLines();
    Task<List<Role>> GetRoles();
    Task<List<Role>> LoadRoles();
    Task<List<Order>> GetOrders();
    Task<List<Order>> LoadOrders();
    Task<List<OrderLine>> GetOrderLines();
    Task<List<OrderLine>> LoadOrderLines();
    Task<List<OrderLineDeadline>> GetOrderLineDeadLines();
    Task<List<OrderLineDeadline>> LoadOrderLineDeadLines();
    Task<List<UserLink>> GetUserLinks();
    Task<List<UserLink>> LoadUserLinks();
    Task<List<FinOp>> GetFinOps();
    Task<List<FinOp>> LoadFinOps();
    Task<List<FinOpOrderLine>> GetFinOpOrderLines();
    Task<List<FinOpOrderLine>> LoadFinOpOrderLines();
    Task<List<Cheque>> GetCheques();
    Task<List<Cheque>> LoadCheques();
    Task<List<Bank>> GetBanks();
    Task<List<Bank>> LoadBanks();
    Task<List<PaymentType>> GetPaymentTypes();
    Task<List<PaymentType>> LoadPaymentTypes();
    Task<List<Product>> GetProducts();
    Task<List<Product>> LoadProducts();
    Task<List<ProductType>> GetProductTypes();
    Task<List<ProductType>> LoadProductTypes();
    Task<List<UserType>> GetUserTypes();
    Task<List<UserType>> LoadUserTypes();
    Task<List<Menu>> GetMenus();
    Task<List<Menu>> LoadMenus();
    Task<List<MenuItem>> GetMenuItems();
    Task<List<MenuItem>> LoadMenuItems();
    Task<List<Capability>> GetCapabilities();
    Task<List<Capability>> LoadCapabilities();
    Task<List<Schedule>> GetSchedules();
    Task<List<Schedule>> LoadSchedules();
    Task<List<ScheduleCourse>> GetScheduleCourses();
    Task<List<ScheduleCourse>> LoadScheduleCourses();
    Task<List<Agenda>> GetAgendas();
    Task<List<Agenda>> LoadAgendas();
    Task<List<Session>> GetSessions();
    Task<List<Session>> LoadSessions();
    Task<List<Event>> GetEvents();
    Task<List<Event>> LoadEvents();
    Task<List<CourseType>> GetCourseTypes();
    Task<List<CourseType>> LoadCourseTypes();
    Task<List<Conflict>> GetConflicts();
    Task<List<Conflict>> LoadConflicts();
    Task<List<CourseConflict>> GetCourseConflicts();
    Task<List<CourseConflict>> LoadCourseConflicts();
    Task<List<UserRole>> GetUserRoles();
    Task<List<UserRole>> LoadUserRoles();
    Task<List<Country>> GetCountries();
    Task<List<Country>> LoadCountries();
    Task<List<City>> GetCities();
    Task<List<City>> LoadCities();
    Task<List<District>> GetDistricts();
    Task<List<District>> LoadDistricts();
    Task<List<MaritalStatus>> GetMaritalStatus();
    Task<List<MaritalStatus>> LoadMaritalStatus();
    Task<List<Photo>> GetPhotos();
    Task<List<Photo>> LoadPhotos();
    Task<List<RoleCapability>> GetRoleCapabilities();
    Task<List<RoleCapability>> LoadRoleCapabilities();
  }
}