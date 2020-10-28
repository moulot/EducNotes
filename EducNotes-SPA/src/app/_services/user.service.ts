import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { User } from '../_models/user';
import { PaginatedResult } from '../_models/pagination';
import { map } from 'rxjs/operators';
import { Message } from '../_models/message';
import { Absence } from '../_models/absence';
import { UserSanction } from '../_models/userSanction';
import { UserReward } from '../_models/userReward';
import { UserClassEvent } from '../_models/userClassEvent';
import { OrderToValidate } from '../_models/orderToValidate';
import { UserFileData } from '../_models/userFileData';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  baseUrl = environment.apiUrl + 'users/';
  sexe = [{id : 0, name : ' FEMME'}, {id : 1, name : ' HOMME'}];

constructor(private http: HttpClient) {}

  getUsers1(page?, itemsPerPage?, userParams?, likesParam?): Observable<PaginatedResult<User[]>> {

    const paginatedResult: PaginatedResult<User[]> = new PaginatedResult<User[]>();

    let params = new HttpParams();

    if (page != null && itemsPerPage != null) {
      params = params.append('pageNumber', page);
      params = params.append('pageSize', itemsPerPage);
    }

    if (userParams != null) {
      params = params.append('minAge', userParams.minAge);
      params = params.append('maxAge', userParams.maxAge);
      params = params.append('gender', userParams.gender);
      params = params.append('orderBy', userParams.orderBy);
    }

    if (likesParam === 'Likers') {
      params = params.append('likers', 'true');
    }

    if (likesParam === 'Likees') {
      params = params.append('likees', 'true');
    }

    return this.http.get<User[]>(this.baseUrl + 'users', {observe: 'response', params})
      .pipe(
        map(response => {
          paginatedResult.result = response.body;
          if (response.headers.get('Pagination') != null) {
            paginatedResult.pagination = JSON.parse(response.headers.get('Pagination'));
          }
          return paginatedResult;
        })
      );
  }

  getUsers() {
    return this.http.get(this.baseUrl);
  }

  getUsersWithRoles() {
    return this.http.get(this.baseUrl + 'usersWithRoles');
  }

  updateUserRoles(user: User, roles: {}) {
    return this.http.post(this.baseUrl + 'editRoles/' + user.userName, roles);
  }

  saveAbsence(absence: Absence) {
    return this.http.put(this.baseUrl + 'saveAbsence', absence);
  }

  getStudentClassLifeData(userId) {
    return this.http.get(this.baseUrl + userId + '/ClassLifeData/');
  }

  getStudentLifeData(userId) {
    return this.http.get(this.baseUrl + userId + '/LifeData/');
  }

  saveClassEvent(userClassEvent: UserClassEvent) {
    return this.http.put(this.baseUrl + 'saveClassEvent', userClassEvent);
  }

  saveSanction(userSanction: UserSanction) {
    return this.http.put(this.baseUrl + 'saveSanction', userSanction);
  }

  saveReward(userReward: UserReward) {
    return this.http.put(this.baseUrl + 'saveReward', userReward);
  }

  getUser(id): Observable<User> {
    return this.http.get<User>(this.baseUrl + id);
  }

  getTeacherWithCourses(teacherId) {
    return this.http.get(this.baseUrl + teacherId + '/teacherWithCourses');
  }

  getParentAccount(id): any {
    return this.http.get(this.baseUrl + 'Account/' + id);
  }

  getChildFile(id) {
    return this.http.get(this.baseUrl + 'ChildFile/' + id);
  }

  saveUserSms(parentId, sms: any) {
    return this.http.put(this.baseUrl + parentId + '/saveSms', sms);
  }

  getChildren(parentId): Observable<User[]> {
    return this.http.get<User[]>(this.baseUrl + parentId + '/Children');
  }

  getAccountChildren(parentId) {
    return this.http.get(this.baseUrl + parentId + '/AccountChildren');
  }

  getTeacherScheduleToday(teacherId) {
    return this.http.get(this.baseUrl + teacherId + '/ScheduleToday');
  }

  getTeacherSchedule(teacherId) {
    return this.http.get(this.baseUrl + teacherId + '/Schedule');
  }

  getTeacherScheduleByDay(teacherId) {
    return this.http.get(this.baseUrl + teacherId + '/ScheduleByDay');
  }

  getTeacherScheduleByClassByDay(teacherId) {
    return this.http.get(this.baseUrl + teacherId + '/ScheduleByClassByDay');
  }

  getGradesData(teacherId, periodId) {
    return this.http.get(this.baseUrl + teacherId + '/GradesData/' + periodId);
  }

  getTeacherClasses(teacherId) {
    return this.http.get(this.baseUrl + teacherId + '/Classes');
  }

  getTeacherClassesWithEvalsByPeriod(teacherId, periodId) {
    return this.http.get(this.baseUrl + teacherId + '/period/' + periodId + '/ClassesWithEvalsByPeriod');
  }

  getTeacherCourses(teacherId) {
    return this.http.get(this.baseUrl + teacherId + '/Courses');
  }

  getTeacherNextCourses(teacherId) {
    return this.http.get(this.baseUrl + teacherId + '/NextCourses');
  }

  getNextCoursesByClass(teacherId) {
    return this.http.get(this.baseUrl + teacherId + '/NextCoursesByClass');
  }

  getTeacherCurrWeekSessions(teacherId, classId) {
    return this.http.get(this.baseUrl + teacherId + '/CurrWeekSessions/' + classId);
  }

  getTeacherSessionsFromToday(teacherId, classId) {
    return this.http.get(this.baseUrl + teacherId + '/SessionsFromToday/' + classId);
  }

  getMovedWeekSessions(teacherId, classId, agendaParams) {

    let params = new HttpParams();
    params = params.append('dueDate', agendaParams.dueDate);
    params = params.append('moveWeek', agendaParams.moveWeek);

    return this.http.get(this.baseUrl + teacherId + '/MovedWeekSessions/' + classId, { params });
  }

  updateUser(id: number, user: User) {
    return this.http.put(this.baseUrl + id, user);
  }

  SetMainPhoto(userId: number, id: number) {
    return this.http.post(this.baseUrl + userId + '/photos/' + id + '/setMain', {});
  }
  // le posr pour enrgister la preinscription : model {father:any,mother:any; children : any[]}
  savePreinscription(data: any) {
    return this.http.post(this.baseUrl + 'SavePreinscription', data);
  }

  deletePhoto(userId: number, id: number) {
    return this.http.delete(this.baseUrl + userId + '/photos/' + id);
  }

  sendLike(id: number, recipientId: number) {
    return this.http.post(this.baseUrl + id + '/like/' + recipientId, {});
  }

  getMessages(id: number, page?, itemsPerPage?, messageContainer?) {

    const paginatedResult: PaginatedResult<Message[]> = new PaginatedResult<Message[]>();

    let params = new HttpParams();

    params = params.append('MessageContainer', messageContainer);

    if (page != null && itemsPerPage != null) {
      params = params.append('pageNumber', page);
      params = params.append('pageSize', itemsPerPage);
    }

    return this.http.get<Message[]>(this.baseUrl + id + '/messages', {observe: 'response', params})
      .pipe(
        map(response => {
          paginatedResult.result = response.body;
          if (response.headers.get('Pagination') != null) {
            paginatedResult.pagination = JSON.parse(response.headers.get('Pagination'));
          }

          return paginatedResult;
        })
      );
  }

  getMessageThread(id: number, recipientId: number) {
    return this.http.get<Message[]>(this.baseUrl + id + '/messages/thread/' + recipientId);
  }

  sendMessage(id: number, message: Message) {
    return this.http.post(this.baseUrl + id + '/messages', message);
  }

  deleteMessage(id: number, userId: number) {
    return this.http.post(this.baseUrl + userId + '/messages/' + id, {});
  }

  markAsRead(userId: number, messageId: number) {
    this.http.post(this.baseUrl + userId + '/messages/' + messageId + '/read', {})
      .subscribe();
  }

  // métode poour récuperer la liste des types User
  getUserTypes() {
    return this.http.get(this.baseUrl + 'GetUserTypes');
  }

  getAllClassesCourses(): Observable<any[]> {
    return this.http.get<any[]>(this.baseUrl + 'GetAllClassesCourses');
  }

  // recuperations de tous les professeurs
  getAllTeachers(): Observable<User[]> {
    return this.http.get<User[]>(this.baseUrl + 'GetAllTeachers');
  }

  addTeacher(user: FormData) {
    return this.http.post(this.baseUrl + 'AddTeacher', user);
  }

  getAssignedClasses(teacherId) {
    return this.http.get(this.baseUrl + teacherId + '/AssignedClasses');
  }

  assignClasses(teacherId, courses) {
    return this.http.post(this.baseUrl + teacherId + '/AssignClasses', courses);
  }

  getUserByTypeId(id: number) {
    return this.http.get(this.baseUrl + 'GetUserByTypeId/' + id);
  }
  // supprimer un professeur
  deleteTeacher(id: number) {
    return this.http.post(this.baseUrl + id + '/DeleteTeacher', {});
  }

  emailExist(email: string) {
    return this.http.get(this.baseUrl + email + '/VerifyEmail');
  }

  // recuperation de tous les userTypes avec details
  getUserTypesDetails() {
    return this.http.get(this.baseUrl + 'GetUserTypesDetails');
  }

  // recupeation des types User pour le personnel
  getAdministrationUserTypes() {
    return this.http.get(this.baseUrl + 'GetAdminUserTypes');
  }

  // mise a jour du userTypes
  updateUserType(id: number, typeName: string) {
      return this.http.post(this.baseUrl + id + '/updateUserType/' + typeName , {} );
  }

  // add usertypes
  addUserType(userType: any) {
   return this.http.post(this.baseUrl + 'AddUserType', userType);
  }

  deleteUSerType(id: number) {
    return this.http.post(this.baseUrl + id + '/DeleteUserType',  {});
   }

   updatePerson(id: number, user: any) {
    return this.http.post(this.baseUrl + id + '/updatePerson', user);
  }

  searchUsers(data: any) {
    return this.http.post(this.baseUrl + 'SearchUsers', data);
  }

// recuperation de toutes les villes
  getAllCities() {
    return this.http.get(this.baseUrl + 'GetAllCities');
  }

  // recuperation des districts en fonction de  l'id de la ville
  getDistrictsByCityId(id: number) {
    return this.http.get(this.baseUrl + id + '/GetDistrictsByCityId');
  }

  getEvents(userId) {
    return this.http.get(this.baseUrl + userId + '/events');
  }

  validateRegistration(order: OrderToValidate) {
    return this.http.post(this.baseUrl + 'ValidateRegistration', order);
  }

  loadStudentData() {
    return this.http.get(this.baseUrl + 'LoadStudents');
  }

  loadUserFile(userFileData: UserFileData) {
    return this.http.post(this.baseUrl + 'loadUserFile', userFileData);
  }

  searchUserFiles(searchData) {
    return this.http.get(this.baseUrl + 'UserFiles');
  }

  getUsersByClasslevel(levelId) {
    return this.http.get(this.baseUrl + 'UsersByLevel/' + levelId);
  }

  getUserInfos(userId, parentId) {
    return this.http.get(this.baseUrl + 'UserInfos/' + userId + '/' + parentId);
  }

  getTeacherScheduleNDays(teacherId) {
    return this.http.get(this.baseUrl + teacherId + '/ScheduleNDays');
  }
}
