import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { HttpClient, HttpParams } from '@angular/common/http';
import { User } from '../_models/user';
import { Observable } from 'rxjs';
import { Agenda } from '../_models/agenda';
import { Course } from '../_models/course';
import { Class } from '../_models/class';
import { Absence } from '../_models/absence';
import { Schedule } from '../_models/schedule';

@Injectable({
  providedIn: 'root'
})
export class ClassService {
  baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) { }

  getClass(classId): Observable<Class> {
    return this.http.get<Class>(this.baseUrl + 'classes/' + classId);
  }

  getClassTeachers(classId): Observable<User[]> {
    return this.http.get<User[]>(this.baseUrl + 'classes/' + classId + '/CourseWithTeacher');
  }

  getTodayToNDaysAgenda(classId, toNbDays) {
    return this.http.get(this.baseUrl + 'classes/' + classId + '/TodayToNDaysAgenda/' + toNbDays);
  }

  getClassCurrWeekAgenda(classId) {
    return this.http.get(this.baseUrl + 'classes/' + classId + '/CurrWeekAgenda');
  }

  getClassMovedWeekAgenda(classId, agendaParams) {

    let params = new HttpParams();
    params = params.append('dueDate', agendaParams.dueDate);
    params = params.append('moveWeek', agendaParams.moveWeek);

    return this.http.get(this.baseUrl + 'classes/' + classId + '/MovedWeekAgenda', { params });
  }

  getWeekDaysByDate(agendaParams): Observable<number[]> {

    let params = new HttpParams();
    params = params.append('dueDate', agendaParams.dueDate);

    return this.http.get<number[]>(this.baseUrl + 'classes/GetWeekDays', { params });
  }

  getSchedule(id) {
    return this.http.get(this.baseUrl + 'classes/Schedule/' + id);
  }

  getSession(scheduleId) {
    return this.http.get(this.baseUrl + 'classes/Session/' + scheduleId);
  }

  saveCallSheet(sessionId: number, absences: Absence[]) {
    return this.http.put(this.baseUrl + 'classes/saveCallSheet/' + sessionId, absences);
  }

  getSanctions() {
    return this.http.get(this.baseUrl + 'classes/GetSanctions');
  }

  getClassAbsences(classId) {
    return this.http.get(this.baseUrl + 'classes/' + classId + '/Absences');
  }

  getAbsencesBySession(sessionId) {
    return this.http.get(this.baseUrl + 'classes/absences/' + sessionId);
  }

  getClassSanctions(classId) {
    return this.http.get(this.baseUrl + 'classes/' + classId + '/ClassSanctions');
  }

  getClassSchedule(classId) {
    return this.http.get(this.baseUrl + 'classes/' + classId + '/Schedule');
  }

  getScheduleToday(classId: Number) {
    return this.http.get(this.baseUrl + 'classes/' + classId + '/schedule/today');
  }

  // pour recuperer les eleves d'une classe a partir de classId
  getClassStudents(classId): Observable<User[]> {
    return this.http.get<User[]>(this.baseUrl + 'classes/' + classId + '/Students');
  }

  getScheduleDay(classId, day) {
    return this.http.get(this.baseUrl + 'classes/' + classId + '/schedule/' + day);
  }

  getAgendaItemById(classId, agendaId) {
    return this.http.get(this.baseUrl + 'classes/' + classId + '/agendas/' + agendaId);
  }

  getAgendaItemByData(agendaParams): Observable<Agenda> {

    let params = new HttpParams();

    params = params.append('classId', agendaParams.classId);
    params = params.append('courseId', agendaParams.courseId);
    params = params.append('dueDate', agendaParams.dueDate);

    return this.http.get<Agenda>(this.baseUrl + 'classes/GetAgendaItem', { params });
  }

  getClassAgenda(classId): Observable<Agenda[]> {
    return this.http.get<Agenda[]>(this.baseUrl + 'classes/' + classId + '/GetClassAgenda');
  }

  // saveAgendaItem(id: number, agendaItem: Agenda) {
  //   return this.http.put(this.baseUrl + 'classes/' + id + '/SaveAgenda', agendaItem);
  // }

  saveAgendaItem(agendaItem: Agenda) {
    return this.http.put(this.baseUrl + 'classes/SaveAgenda', agendaItem);
  }

  getClassCourses(classId) {
    return this.http.get(this.baseUrl + 'classes/' + classId + '/ClassCourses');
  }

  getClassCoursesWithAgenda(classId, daysToNow, daysFromNow) {
    return this.http.get(this.baseUrl + 'classes/' + classId + '/CoursesWithAgenda/f/' + daysToNow + '/t/' + daysFromNow);
  }

  getClassAgendaByDate(classId, agendaParams) {

    let params = new HttpParams();
    params = params.append('currentDate', agendaParams.currentDate);
    params = params.append('nbDays', agendaParams.nbDays);
    params = params.append('isMovingPeriod', agendaParams.isMovingPeriod);

    return this.http.get(this.baseUrl + 'classes/' + classId + '/AgendaByDate', { params });
  }

  classAgendaSetDone(agendaId, isDone) {
    return this.http.get(this.baseUrl + 'classes/Agenda/' + agendaId + '/SetTask/' + isDone);
  }

  getAllClasses() {
    return this.http.get<any[]>(this.baseUrl + 'classes/GetAllClasses');
  }
  // recuperer tous les professeurs ainsi que les cours qui leurs sont deja assignés
  getAllTeachersCourses() {
    return this.http.get(this.baseUrl + 'classes/GetAllTeachersCourses');
  }

  getAllTeacherCoursesById(id: number) {
    return this.http.get(this.baseUrl + 'classes/' + id + '/GetAllTeacherCoursesById');
  }

  // recuperation de tous les cours
  getAllCourses(): Observable<Course[]> {
    return this.http.get<Course[]>(this.baseUrl + 'classes/GetAllCourses');
  }

  saveSchedules(schedules: Schedule[]) {
    return this.http.put(this.baseUrl + 'classes/saveSchedules', schedules);
  }

  saveTeacherAffectation(id: number, courseId: number, levelId: number, classIds: number[]) {
    return this.http.post(this.baseUrl + 'classes/' + id + '/' + courseId + '/' + levelId + ' /SaveTeacherAffectation', classIds);
  }
  updateTeacher(id: number, user: any) {
    return this.http.post(this.baseUrl + 'classes/' + id + '/UpdateTeacher', user);
  }

  ////////////////////////////////////////
  // tous les cours et leurs differents professeurs
  getCoursesTeachers() {
    return this.http.get(this.baseUrl + 'classes/GetCoursesTeachers');
  }

  getAllCoursesDetails() {
    return this.http.get(this.baseUrl + 'classes/GetAllCoursesDetails');
  }

  getStudentAllDetailsById(id: number) {
    return this.http.get(this.baseUrl + 'classes/' + id + '/GetStudentsAllDetailsById');
  }

  getParentAllDetailsById(id: number) {
    return this.http.get(this.baseUrl + 'classes/' + id + '/GetParentAllDetailsById');
  }

  getLevels() {
    return this.http.get(this.baseUrl + 'classes/GetLevels');
  }

  updatCourse(courseId: number, course: Course) {
    return this.http.post(this.baseUrl + 'classes/UpdateCourse/' + courseId, course);
  }

  getCourse(courseId: number): Observable<Course> {
    return this.http.get<Course>(this.baseUrl + 'classes/course/' + courseId);

  }

  saveClassModification(classId: number, data: any) {
    return this.http.post(this.baseUrl + 'classes/' + classId + '/UpdateClass', data);
  }

  updateCourse(courseId: number, courseName: string) {
    return this.http.post(this.baseUrl + 'classes/' + courseId + '/UpdateCourse/' + courseName, {});
  }

  addNewCourse(course: any) {
    return this.http.post(this.baseUrl + 'classes/AddCourse', course);
  }

  courseExist(courseName) {
    return this.http.get(this.baseUrl + 'classes/CourseExist/' + courseName);
  }

  getClassesByLevelId(id: number) {
    return this.http.get(this.baseUrl + 'classes/' + id + '/SearchClassesByLevel');
  }

  getClassLevelsWithClasses(ids: number[]) {
    return this.http.post(this.baseUrl + 'classes/ClassLevelsWithClasses', ids);
  }

  saveNewClasses(classes: any) {
    return this.http.post(this.baseUrl + 'classes/SaveNewClasses', classes);
  }

  deleteClass(classId: number) {
    return this.http.post(this.baseUrl + 'classes/' + classId + '/DeleteClass', {});
  }

  // saveClassModification(courseId: number, courseName: string) {
  //   return this.http.post(this.baseUrl + 'classes/' + courseId + '/UpdateCourse' + courseName, {}); updateCourse
  // }

  teacherClassCoursByLevel(teacherId: number, levelid: number, courseId: number) {
    return this.http.get(this.baseUrl + 'classes/TeacherClassCoursByLevel/  ' + teacherId + '/' + levelid + '/' + courseId);
  }

  getTeacher(id) {
    return this.http.get(id);
  }

  getClassTypes() {
    return this.http.get(this.baseUrl + 'classes/ClassTypes');
  }

  createCourseCoefficient(courseCoefficient) {
    return this.http.post(this.baseUrl + 'classes/CreateCourseCoefficient', courseCoefficient);
  }
  getClasslevelsCoefficients(levelId: number) {
    return this.http.get(this.baseUrl + 'classes/ClassLevelCoefficients/' + levelId);
  }

  getCourseCoefficient(id: number) {
    return this.http.get(this.baseUrl + 'classes/CourseCoefficient/' + id);

  }

}
