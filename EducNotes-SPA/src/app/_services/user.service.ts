import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { User } from '../_models/user';
import { PaginatedResult } from '../_models/pagination';
import { map } from 'rxjs/operators';
import { Message } from '../_models/message';
import { Absence } from '../_models/absence';
import { UserSanction } from '../_models/userSanction';
import { UserReward } from '../_models/userReward';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  baseUrl = environment.apiUrl;
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
    return this.http.get(this.baseUrl + 'users');
  }

  getUsersWithRoles() {
    return this.http.get(this.baseUrl + 'users/usersWithRoles');
  }

  updateUserRoles(user: User, roles: {}) {
    return this.http.post(this.baseUrl + 'users/editRoles/' + user.userName, roles);
  }

  saveAbsence(absence: Absence) {
    return this.http.put(this.baseUrl + 'users/saveAbsence', absence);
  }

  getStudentLifeData(userId) {
    return this.http.get(this.baseUrl + 'users/' + userId + '/LifeData/');
  }

  saveSanction(userSanction: UserSanction) {
    return this.http.put(this.baseUrl + 'users/saveSanction', userSanction);
  }

  saveReward(userReward: UserReward) {
    return this.http.put(this.baseUrl + 'users/saveReward', userReward);
  }

  getUser(id): Observable<User> {
    return this.http.get<User>(this.baseUrl + 'users/' + id);
  }

  getChildren(parentId): Observable<User[]> {
    return this.http.get<User[]>(this.baseUrl + 'users/' + parentId + '/Children');
  }

  getTeacherScheduleToday(teacherId) {
    return this.http.get(this.baseUrl + 'users/' + teacherId + '/ScheduleToday');
  }

  getTeacherSchedule(teacherId) {
    return this.http.get(this.baseUrl + 'users/' + teacherId + '/Schedule');
  }

  getTeacherClasses(teacherId) {
    return this.http.get(this.baseUrl + 'users/' + teacherId + '/Classes');
  }

  getTeacherClassesWithEvalsByPeriod(teacherId, periodId) {
    return this.http.get(this.baseUrl + 'users/' + teacherId + '/period/' + periodId + '/ClassesWithEvalsByPeriod');
  }

  getTeacherCourses(teacherId) {
    return this.http.get(this.baseUrl + 'users/' + teacherId + '/Courses');
  }

  getTeacherNextCourses(teacherId) {
    return this.http.get(this.baseUrl + 'users/' + teacherId + '/NextCourses');
  }

  getTeacherSessions(teacherId, classId) {
    return this.http.get(this.baseUrl + 'users/' + teacherId + '/Sessions/' + classId);
  }

  updateUser(id: number, user: User) {
    return this.http.put(this.baseUrl + 'users/' + id, user);
  }

  SetMainPhoto(userId: number, id: number) {
    return this.http.post(this.baseUrl + 'users/' + userId + '/photos/' + id + '/setMain', {});
  }
  // le posr pour enrgister la preinscription : model {father:any,mother:any; children : any[]}
  savePreinscription(data: any) {
    return this.http.post(this.baseUrl + 'users/SavePreinscription', data);
  }

  deletePhoto(userId: number, id: number) {
    return this.http.delete(this.baseUrl + 'users/' + userId + '/photos/' + id);
  }

  sendLike(id: number, recipientId: number) {
    return this.http.post(this.baseUrl + 'users/' + id + '/like/' + recipientId, {});
  }

  getMessages(id: number, page?, itemsPerPage?, messageContainer?) {

    const paginatedResult: PaginatedResult<Message[]> = new PaginatedResult<Message[]>();

    let params = new HttpParams();

    params = params.append('MessageContainer', messageContainer);

    if (page != null && itemsPerPage != null) {
      params = params.append('pageNumber', page);
      params = params.append('pageSize', itemsPerPage);
    }

    return this.http.get<Message[]>(this.baseUrl + 'users/' + id + '/messages', {observe: 'response', params})
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
    return this.http.get<Message[]>(this.baseUrl + 'users/' + id + '/messages/thread/' + recipientId);
  }

  sendMessage(id: number, message: Message) {
    return this.http.post(this.baseUrl + 'users/' + id + '/messages', message);
  }

  deleteMessage(id: number, userId: number) {
    return this.http.post(this.baseUrl + 'users/' + userId + '/messages/' + id, {});
  }

  markAsRead(userId: number, messageId: number) {
    this.http.post(this.baseUrl + 'users/' + userId + '/messages/' + messageId + '/read', {})
      .subscribe();
  }

  // métode poour récuperer la liste des types User
  getUserTypes() {
    return this.http.get(this.baseUrl + 'users/GetUserTypes');
  }

  getAllClassesCourses(): Observable<any[]> {
    return this.http.get<any[]>(this.baseUrl + 'users/GetAllClassesCourses');
  }

  // recuperations de tous les professeurs
  getAllTeachers(): Observable<User[]> {
    return this.http.get<User[]>(this.baseUrl + 'users/GetAllTeachers');
  }
  // enregistrement d'un nouveau teacher
  saveTeacher(user: User) {
    return this.http.post(this.baseUrl + 'users/' + 'AddUser', user);
  }

  // enregistrement d'un nouveau teacher
  addUser(user: User) {
    return this.http.post(this.baseUrl + 'users/' + 'AddUser', user);
  }

  getUserByTypeId(id: number) {
    return this.http.get(this.baseUrl + 'users/' + 'GetUserByTypeId/' + id);
  }
  // supprimer un professeur
  deleteTeacher(id: number) {
    return this.http.post(this.baseUrl + 'users/' + id + '/DeleteTeacher', {});
  }

  emailExist(email: string) {
    return this.http.get(this.baseUrl + 'users/' + email + '/VerifyEmail');
  }

  // recuperation de tous les userTypes avec details
  getUserTypesDetails() {
    return this.http.get(this.baseUrl + 'users/GetUserTypesDetails');
  }

  // recupeation des types User pour le personnel
  getAdministrationUserTypes() {
    return this.http.get(this.baseUrl + 'users/GetAdminUserTypes');
  }

  // mise a jour du userTypes
  updateUserType(id: number, typeName: string) {
      return this.http.post(this.baseUrl + 'users/' + id + '/updateUserType/' + typeName , {} );
  }

  // add usertypes
  addUserType(userType: any) {
   return this.http.post(this.baseUrl + 'users/AddUserType', userType);
  }

  deleteUSerType(id: number) {
    return this.http.post(this.baseUrl + 'users/' + id + '/DeleteUserType',  {});
   }
   updatePerson(id: number, user: any) {
    return this.http.post(this.baseUrl + 'users/' + id + '/updatePerson', user);
  }

  searchUsers(data: any) {
    return this.http.post(this.baseUrl + 'users/SearchUsers', data);
  }

// recuperation de toutes les villes
  getAllCities() {
    return this.http.get(this.baseUrl + 'users/GetAllCities');
  }

  // recuperation des districts en fonction de  l'id de la ville
  getDistrictsByCityId(id: number) {
    return this.http.get(this.baseUrl + 'users/' + id + '/GetDistrictsByCityId');
  }

}
