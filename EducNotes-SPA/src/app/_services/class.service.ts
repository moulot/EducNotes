import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { HttpClient, HttpParams } from '@angular/common/http';
import { User } from '../_models/user';
import { Observable } from 'rxjs';
import { Agenda } from '../_models/agenda';
import { Course } from '../_models/course';
import { Class } from '../_models/class';
import { Absence } from '../_models/absence';
import { Period } from '../_models/period';
import { ScheduleData } from '../_models/scheduleData';

@Injectable({
  providedIn: 'root'
})
export class ClassService {
  baseUrl = environment.apiUrl + 'classes/';

  constructor(private http: HttpClient) { }

  getClass(classId): Observable<Class> {
    return this.http.get<Class>(this.baseUrl + classId);
  }

  getCourseWithTeacher(classId): Observable<User[]> {
    return this.http.get<User[]>(this.baseUrl + classId + '/CourseWithTeacher');
  }

  getClassTeachers(classId): Observable<User[]> {
    return this.http.get<User[]>(this.baseUrl + classId + '/teachers');
  }

  getTodayToNDaysAgenda(classId, toNbDays) {
    return this.http.get(this.baseUrl + classId + '/TodayToNDaysAgenda/' + toNbDays);
  }

  getClassCurrWeekAgenda(classId) {
    return this.http.get(this.baseUrl + classId + '/CurrWeekAgenda');
  }

  getClassMovedWeekAgenda(classId, agendaParams) {

    let params = new HttpParams();
    params = params.append('dueDate', agendaParams.dueDate);
    params = params.append('moveWeek', agendaParams.moveWeek);
    return this.http.get(this.baseUrl + classId + '/MovedWeekAgenda', { params });
  }

  getPDFFromHtml(data: any) {
    return this.http.post(this.baseUrl + 'HtmlToPDF', data);
  }

  getWeekDaysByDate(agendaParams): Observable<number[]> {

    let params = new HttpParams();
    params = params.append('dueDate', agendaParams.dueDate);

    return this.http.get<number[]>(this.baseUrl + 'GetWeekDays', { params });
  }

  getScheduleNDays(classId) {
    return this.http.get(this.baseUrl + classId + '/ScheduleNDays');
  }

  // getSchedule(id) {
  //   return this.http.get(this.baseUrl + 'Schedule/' + id);
  // }

  getSessionData(scheduleId) {
    return this.http.get(this.baseUrl + 'SessionData/' + scheduleId);
  }

  getSessionFromSchedule(scheduleId) {
    return this.http.get(this.baseUrl + 'Schedule/' + scheduleId + '/Session');
  }

  getSession(id) {
    return this.http.get(this.baseUrl + 'Sessions/' + id);
  }

  getCallSheetStudents(classId) {
    return this.http.get(this.baseUrl + classId + '/CallSheet/Students');
  }

  saveCallSheet(sessionId: number, absences: Absence[]) {
    return this.http.put(this.baseUrl + 'saveCallSheet/' + sessionId, absences);
  }

  getSanctions() {
    return this.http.get(this.baseUrl + 'GetSanctions');
  }

  getClassAbsences(classId) {
    return this.http.get(this.baseUrl + classId + '/Absences');
  }

  getAbsencesBySession(sessionId) {
    return this.http.get(this.baseUrl + 'absences/' + sessionId);
  }

  getClassSanctions(classId) {
    return this.http.get(this.baseUrl + classId + '/ClassSanctions');
  }

  getClassTimeTable(classId) {
    return this.http.get(this.baseUrl + classId + '/TimeTable');
  }

  getClassScheduleByDay(classId) {
    return this.http.get(this.baseUrl + classId + '/ScheduleByDay');
  }

  delCourseFromSchedule(scheduleId) {
    return this.http.put(this.baseUrl + 'DelCourseFromSchedule/' + scheduleId, {});
  }

  getScheduleToday(classId: Number) {
    return this.http.get(this.baseUrl + classId + '/schedule/today');
  }

  // pour recuperer les eleves d'une classe a partir de classId
  getClassStudents(classId): Observable<User[]> {
    return this.http.get<User[]>(this.baseUrl + classId + '/Students');
  }

  getScheduleDay(classId, day) {
    return this.http.get(this.baseUrl + classId + '/schedule/' + day);
  }

  getAgendaItemById(classId, agendaId) {
    return this.http.get(this.baseUrl + classId + '/agendas/' + agendaId);
  }

  getAgendaItemByData(agendaParams): Observable<Agenda> {

    let params = new HttpParams();

    params = params.append('classId', agendaParams.classId);
    params = params.append('courseId', agendaParams.courseId);
    params = params.append('dueDate', agendaParams.dueDate);

    return this.http.get<Agenda>(this.baseUrl + 'GetAgendaItem', { params });
  }

  getClassAgenda(classId): Observable<Agenda[]> {
    return this.http.get<Agenda[]>(this.baseUrl + classId + '/ClassAgenda');
  }

  // saveAgendaItem(id: number, agendaItem: Agenda) {
  //   return this.http.put(this.baseUrl + id + '/SaveAgenda', agendaItem);
  // }

  saveAgendaItem(agendaItem: Agenda) {
    return this.http.put(this.baseUrl + 'SaveAgenda', agendaItem);
  }

  getClassCourses(classId) {
    return this.http.get(this.baseUrl + classId + '/ClassCourses');
  }

  getEducLevels() {
    return this.http.get(this.baseUrl + 'EducLevels');
  }

  getClassCoursesWithAgenda(classId, daysToNow, daysFromNow) {
    return this.http.get(this.baseUrl + classId + '/CoursesWithAgenda/f/' + daysToNow + '/t/' + daysFromNow);
  }

  getClassAgendaNbDays(classId) {
    return this.http.get(this.baseUrl + classId + '/AgendaNbDays');
  }

  getClassAgendaByDate(classId, agendaParams) {

    let params = new HttpParams();
    params = params.append('currentDate', agendaParams.currentDate);
    params = params.append('nbDays', agendaParams.nbDays);
    params = params.append('isMovingPeriod', agendaParams.isMovingPeriod);

    return this.http.get(this.baseUrl + classId + '/AgendaByDate', { params });
  }

  classAgendaSetDone(agendaId, isDone) {
    return this.http.get(this.baseUrl + 'Agenda/' + agendaId + '/SetTask/' + isDone);
  }

  getClassesByLevel() {
    return this.http.get(this.baseUrl + 'ClassesByLevel');
  }

  getFreePrimaryClasses(teacherId, educLevelId) {
    return this.http.get(this.baseUrl + teacherId + '/FreePrimaryClasses/' + educLevelId);
  }

  getAllClasses() {
    return this.http.get<any[]>(this.baseUrl + 'AllClasses');
  }

  // recuperer tous les professeurs ainsi que les cours qui leurs sont deja assignés
  getTeachersWithCourses() {
    return this.http.get(this.baseUrl + 'TeachersWithCourses');
  }

  getAllTeacherCoursesById(id: number) {
    return this.http.get(this.baseUrl + id + '/GetAllTeacherCoursesById');
  }

  // recuperation de tous les cours
  getCourses(): Observable<Course[]> {
    return this.http.get<Course[]>(this.baseUrl + 'GetCourses');
  }

  saveSchedules(schedules: ScheduleData[]) {
    return this.http.put(this.baseUrl + 'saveSchedules', schedules);
  }

  saveTeacherAffectation(id: number, courseId: number, levelId: number, classIds: number[]) {
    return this.http.post(this.baseUrl + id + '/' + courseId + '/' + levelId + ' /SaveTeacherAffectation', classIds);
  }

  updateTeacher(id: number, user: any) {
    return this.http.post(this.baseUrl + id + '/UpdateTeacher', user);
  }

  ////////////////////////////////////////
  // tous les cours et leurs differents professeurs
  getCoursesTeachers() {
    return this.http.get(this.baseUrl + 'GetCoursesTeachers');
  }

  getAllCoursesDetails() {
    return this.http.get(this.baseUrl + 'GetAllCoursesDetails');
  }

  getStudentAllDetailsById(id: number) {
    return this.http.get(this.baseUrl + id + '/GetStudentsAllDetailsById');
  }

  getParentAllDetailsById(id: number) {
    return this.http.get(this.baseUrl + id + '/GetParentAllDetailsById');
  }

  getLevels() {
    return this.http.get(this.baseUrl + 'ClassLevels');
  }

  getLevelsWithClasses() {
    return this.http.get(this.baseUrl + 'LevelsWithClasses');
  }

  updatCourse(courseId: number, course: Course) {
    return this.http.post(this.baseUrl + 'UpdateCourse/' + courseId, course);
  }

  getCourse(courseId: number): Observable<Course> {
    return this.http.get<Course>(this.baseUrl + 'course/' + courseId);

  }

  saveClassModification(classId: number, data: any) {
    return this.http.post(this.baseUrl + classId + '/UpdateClass', data);
  }

  updateCourse(courseId: number, courseName: string) {
    return this.http.post(this.baseUrl + courseId + '/UpdateCourse/' + courseName, {});
  }

  addCourse(course: any) {
    return this.http.post(this.baseUrl + 'AddCourse', course);
  }

  courseExist(courseName) {
    return this.http.get(this.baseUrl + 'CourseExist/' + courseName);
  }

  getClassesByLevelId(id: number) {
    return this.http.get(this.baseUrl + id + '/ClassesByLevelId');
  }

  getClassLevelsWithClasses(ids: number[]) {
    return this.http.post(this.baseUrl + 'ClassLevelsWithClasses', ids);
  }

  saveClasses(classes: any) {
    return this.http.post(this.baseUrl + 'SaveClasses', classes);
  }

  deleteClass(classId: number) {
    return this.http.post(this.baseUrl + classId + '/DeleteClass', {});
  }

  teacherClassCoursByLevel(teacherId: number, levelid: number, courseId: number) {
    return this.http.get(this.baseUrl + 'TeacherClassCoursByLevel/  ' + teacherId + '/' + levelid + '/' + courseId);
  }

  getTeacher(id) {
    return this.http.get(id);
  }

  getClassTypes() {
    return this.http.get(this.baseUrl + 'ClassTypes');
  }

  getCLClassTypes() {
    return this.http.get(this.baseUrl + 'CLClassTypes');
  }

  getLevelWithClassTypes() {
    return this.http.get(this.baseUrl + 'ClassTypesByLevel');
  }

  createCourseCoefficient(courseCoefficient) {
    return this.http.post(this.baseUrl + 'CreateCourseCoefficient', courseCoefficient);
  }
  getClasslevelsCoefficients(levelId: number) {
    return this.http.get(this.baseUrl + 'ClassLevelCoefficients/' + levelId);
  }

  getCourseCoefficient(id: number) {
    return this.http.get(this.baseUrl + 'CourseCoefficient/' + id);
  }

  updateCourseCoefficient(id: number, coeffficient: number) {
    return this.http.post(this.baseUrl + 'EditCoefficient/' + id + '/' + coeffficient, {});
  }

  saveNewTheme(theme) {
    return this.http.post(this.baseUrl + 'SaveNewTheme', theme);
  }
  getTeacherCourseProgram(courseId: number, teacherId: number) {
    return this.http.get(this.baseUrl + 'courses/' + courseId + '/teacher/' + teacherId + '/Program');
  }

  searchThemes(classLevelId: number, courseId: number) {
    return this.http.get(this.baseUrl + 'ClassLevelCourseThemes/' + classLevelId + '/' + courseId);
  }

  getPeriods(): Observable<Period[]> {
    return this.http.get<Period[]>(this.baseUrl + 'Periods');
  }

  getClassEvents() {
    return this.http.get(this.baseUrl + 'ClassEvents');
  }

  getEvents(classId) {
    return this.http.get(this.baseUrl + classId + '/events');
  }

  classStudentsAssignment(classId: number, studentsList) {
    return this.http.post(this.baseUrl + 'classStudentsAssignment/' + classId , studentsList);
  }

  addCourseShowing(courseShowing: FormData) {
    return this.http.post(this.baseUrl + 'courseShowing', courseShowing);
  }

  getActiveClasslevels() {
    return this.http.get(this.baseUrl + 'ActiveClasslevels');
  }

  getClassLevelCourses() {
    return this.http.get(this.baseUrl + 'ClassLevelCourses');
  }

  saveLevelCourses(levelCourses) {
    return this.http.post(this.baseUrl + 'saveCLCourses', levelCourses);
  }

  getCoursesByLevelId(levelid) {
    return this.http.get(this.baseUrl + 'CoursesByLevelId/' + levelid);
  }

  getCurrentWeekAbsences() {
    return this.http.get(this.baseUrl + 'CurrentWeekAbsences');
  }

  getWeekAbsences(data: any) {
    return this.http.post(this.baseUrl + 'WeekAbsences', data);
  }

  getDayAbsences(date: Date) {
    return this.http.post(this.baseUrl + 'DayAbsences', date);
  }

  getUserClassLife(childId) {
    return this.http.get(this.baseUrl + 'UserClassLife/' + childId);
  }

  getScheduleCoursesByDay(classId) {
    return this.http.get(this.baseUrl + classId + '/ScheduleCoursesByDay');
  }

}
