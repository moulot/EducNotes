using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EducNotes.API.Dtos;
using EducNotes.API.Helpers;
using EducNotes.API.Models;

namespace EducNotes.API.Data
{
  public interface IEducNotesRepository
  {
    void Add<T>(T entity) where T : class;
    void Update<T>(T entity) where T : class;
    void Delete<T>(T entity) where T : class;
    void DeleteAll<T>(List<T> entities) where T : class;
    Task<bool> SaveAll();
    string GetAppSubDomain();
    Task<PagedList<User>> GetUsers(UserParams userParams);
    Task<User> GetUser(int id, bool isCurrentUser);
    Task<IEnumerable<User>> GetChildren(int parentId);
    Task<IEnumerable<User>> GetSiblings(int childId);
    Task<List<User>> GetParentsChildren(int motherId, int fatherId);
    Task<List<User>> GetParents(int childId);
    Task<List<UserForDetailedDto>> GetAccountChildren(int parentId);
    Task<IEnumerable<User>> GetClassStudents(int classId);
    Task<IEnumerable<Agenda>> GetClassAgenda(int classId, DateTime StartDate, DateTime EndDate);
    Task<IEnumerable<Agenda>> GetClassAgendaTodayToNDays(int classId, int toNbDays);
    Task<List<ScheduleForTimeTableDto>> GetClassSchedule(int classId);
    Task<IEnumerable<ClassLevelSchedule>> GetClassLevelSchedule(int classLevelId);
    Task<IEnumerable<CourseSkill>> GetCourseSkills(int courseId);
    Task<Agenda> GetAgenda(int agendaId);
    Task<Photo> GetPhoto(int id);
    Task<Photo> GetMainPhotoForUser(int userId);
    Task<Class> GetClass(int Id);
    List<int> GetWeekDays(DateTime date);
    Task<IEnumerable<ScheduleCourse>> GetScheduleDay(int classId, int day);
    Task<Like> GetLike(int userId, int recipientId);
    Task<bool> EmailExist(string email);
    Task<bool> UserNameExist(string userName, int currentUserId);
    Task<bool> UpdateChildren(ChildrenForEditDto users);
    Task<bool> AddTeacher(TeacherForEditDto user);
    Task<bool> AddEmployee(EmployeeForEditDto user);
    Task<bool> EditUserAccount(UserAccountForEditDto user);
    Task<IEnumerable<Agenda>> GetClassAgenda(int classId);
    Task<List<AgendaForListDto>> GetUserClassAgenda(int classId, DateTime startDate, DateTime endDate);
    Task<IEnumerable<UserType>> getUserTypes();
    Task<Course> GetCourse(int Id);
    Task<bool> SendEmail(EmailFormDto emailFormDto);
    bool SendSms(List<string> phoneNumbers, string content);

    Task<IEnumerable<City>> GetAllCities();
    Task<IEnumerable<District>> GetAllGetDistrictsByCityIdCities(int id);

    void AddUserLink(int userId, int parentId);

    Task<User> GetUserByEmail(string email);
    Task<bool> SendResetPasswordLink(User user, string code);
    Task<User> GetSingleUser(string userName);
    Task<List<UserCourseEvalsDto>> GetUserGrades(int userId, int classId);
    IEnumerable<ClassAgendaToReturnDto> GetAgendaListByDueDate(IEnumerable<Agenda> agendaItems);
    Task<List<TeacherClassesDto>> GetTeacherClasses(int teacherId);
    Task<List<Course>> GetTeacherCourses(int teacherId);
    Task<List<ClassesWithEvalsDto>> GetTeacherClassesWithEvalsByPeriod(int teacherId, int periodId);
    Task<List<EvaluationForListDto>> GetEvalsToCome(int classId);
    Task<EmailTemplate> GetEmailTemplate(int id);
    Task<SmsTemplate> GetSmsTemplate(int id);
    void Clickatell_SendSMS(clickatellParamsDto smsData);
    Task<List<Sms>> SetSmsDataForAbsences(List<AbsenceSmsDto> absences, int sessionId, int teacherId);
    Task<List<Sms>> SetSmsDataForNewGrade(List<EvalSmsDto> grades, string content, int teacherId);
    List<string> SendBatchSMS(List<Sms> smsData);
    double GetClassEvalAvg(List<UserEvaluation> classGrades, double maxGrade);
    Task<IEnumerable<ClassType>> GetClassTypes();
    Task<List<string>> GetEmails();
    Task<List<string>> GetUserNames();
    Task<List<ClassLevel>> GetLevels();
    Task sendOk(int userTypeId, int userId);
    Task<List<UserSpaCodeDto>> ParentSelfInscription(int parentId, List<UserForUpdateDto> userToUpdate);
    Task<int> GetAssignedChildrenCount(int parentId);
    Task<bool> SaveProductSelection(int userPid, int userId,List<ServiceSelectionDto> products);
    Task<List<Period>> GetPeriods();
    Task<Period> GetPeriodFromDate(DateTime date);
    Task<Session> GetSessionFromSchedule(ScheduleCourse course, DateTime sessionDate);
    Task<List<Class>> GetClassesByLevelId(int levelId);
    Task<IEnumerable<Theme>> ClassLevelCourseThemes(int classLevelId, int courseId);
    Task<IEnumerable<Lesson>> ClassLevelCourseLessons(int classLevelId, int courseId);
    Task<int> CreateLessonDoc(CourseShowingDto courseShowingDto);
    Task<bool> SendCourseShowingLink(int lessonDocId);
    Task<IEnumerable<Setting>> GetSettings();
    Task<IEnumerable<EducationLevelDto>> GetEducationLevelsWithClasses();
    Task<List<EducationLevel>> GetEducationLevels();
    Task<List<Class>> GetFreePrimaryClasses(int teacherId);
    Task<IEnumerable<SchoolDto>> GetSchools();
    Task<IEnumerable<CycleDto>> GetCycles();
    Task<IEnumerable<Email>> SetEmailDataForRegistration(IEnumerable<RegistrationEmailDto> emailData, string content, string RegDeadLine);
    Task<Order> GetOrder(int id);
    Task<Email> SetEmailForAccountUpdated(string subject, string content, string lastName, byte gender, string parentEmail, int userId);
    string GetUserIDNumber(int userId, string lastName, string firstName);
    string GetInvoiceNumber(int invoiceId);
    Task<IEnumerable<PaymentType>> GetPaymentTypes();
    Task<IEnumerable<ClassLevel>> GetClasslevels();
    Task<IEnumerable<Bank>> GetBanks();
    Task<Email> SetDataForConfirmTeacherEmail(ConfirmTeacherEmailDto emailData, string body, string subject);
    Task<List<FinOpDto>> GetOrderPayments(int orderId);
    Task<List<OrderLine>> GetOrderLines(int orderId);
    Task<List<User>> GetUsersByClasslevel(int levelId);
    Task<List<FinOpOrderLine>> GetChildPayments(int childId);
    Task<NextDueAmountDto> GetChildDueAmount(int orderLineId, decimal paidAmount);
    Task<List<ClassLevel>> GetActiveClassLevels();
    Task<List<Product>> GetActiveProducts();
    Task<List<OrderLinePaidDto>> GetOrderLinesPaid();
    Task<List<EventDto>> GetUserEvents(int userId);
    Task<List<NextCoursesByClassDto>> GetNextCoursesByClass(int teacherId);
    Task<List<UserScheduleNDaysDto>> GetTeacherScheduleNDays(int teacherId);
    string CalculateCourseTop(DateTime startHourMin, string startCourseHourMin);
    Task<List<Absence>> GetAbsencesByDate(DateTime date);
    Task<double> GetStudentAvg(int userId, int classId);
    Task<List<double>> GetEvalMinMax(int evalId);
    Task<double> GetEvalMax(int evalId);
    Task<double> GetClassAvg(int classId);
    Task<ClassAvgDto> GetClassAvgs(int classId);
    Task<ClassAvgDto> GetClassCourseAvgs(int courseId, int classId);
    Task<List<GradeDto>> GetStudentLastGrades(int userId, int nbOfGrades);
    Task<List<Course>> GetUserCourses(int classId);
    Task<User> GetUserByEmailAndLogin(string username, string email);
    Task<LateAmountsDto> GetLateAmountsDue();
    Task<LateAmountsDto> GetProductLateAmountsDue(int productId, int levelId);
    Task<LateAmountsDto> GetChildLateAmountsDue(int productId, int childId);
    Task<Boolean> SendTeacherConfirmEmail(int userId);
    Task<List<Token>> GetTokens();
    string ReplaceTokens(List<TokenDto> tokens, string content);
    List<TokenDto> GetMessageTokenValues(List<Token> tokens, UserToSendMsgDto user);
    Task<List<Token>> GetBroadcastTokens();
    MsgRecipientsDto GetMsgRecipientsForClasses(List<User> users, int msgChoice, Boolean sendToNotValidated);
    Task<MsgRecipientsDto> GetMsgRecipientsForUsers(List<User> users, List<int> educLevelIds, 
      List<int> schoolIds, List<int> classLevelIds, List<int> classIds, int msgChoice, Boolean sendToNotValidated);
    MsgRecipientsDto setRecipientsList(List<User> users, int msgChoice, Boolean sendToNotValidated);
    List<ClassDayCoursesDto> GetCoursesFromSchedules(IEnumerable<ScheduleForTimeTableDto> schedules);
    Task<Boolean> UserInRole(int userId, int roleId);
    // Boolean MenuExists(int menuItemId, List<MenuItem> menuItems);
    // MenuItem GetByMenuItemId(int menuItemId, List<MenuItem> menuItems);
    // MenuItem FindOrLoadParent(List<MenuItem> menuItems, int parentMenuItemId);
    // MenuItem GetByMenuItemName(string menuItemName, List<MenuItem> menuItems);
    // MenuItem GetTopMenuItem(string menuItemName, List<MenuItem> menuItems);
    Task<UserWithRolesDto> GetUserWithRoles(int userId);
    // Task<List<MenuItemDto>> GetUserMenu(int userId);
    Task<List<MenuItemDto>> GetUserTypeMenu(int userTypeId, int userId);
    // Task<Boolean> HasAccessToMenu(int userId, int menuItemId);
    Task<List<MenuCapabilitiesDto>> GetMenuCapabilities(int userTypeId, int userId);
    Task<ErrorDto> SaveRole(RoleDto user);
    Task<ErrorDto> SaveLevelTuitionFees(ScheduleDueDateFeeDto tuitionFeeData);
    Task<ErrorDto> SaveProductDeadLines(List<ProductDeadlineDto> productDeadLines);
    Task<List<OrderLinePaidDto>> GetChildOrderLinesPaid(int childId);
  }
}