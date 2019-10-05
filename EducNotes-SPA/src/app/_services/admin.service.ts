import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { HttpClient, HttpParams } from '@angular/common/http';
import { User } from '../_models/user';
import { Period } from '../_models/period';
import { Observable } from 'rxjs';
import { AlertifyService } from './alertify.service';
import { Establishment } from '../_models/establishment';

@Injectable({
  providedIn: 'root'
})
export class AdminService {
  baseUrl = environment.apiUrl;
  sexe = [{id : 0, name : ' FEMME'}, {id : 1, name : ' HOMME'}];

  constructor(private http: HttpClient, private alertify: AlertifyService) { }

  updateUserRoles(user: User, roles: {}) {
    return this.http.post(this.baseUrl + 'admin/editRoles/' + user.userName, roles);
  }

  getPhotosForApproval() {
    return this.http.get(this.baseUrl + 'admin/photosForModeration');
  }

  approvePhoto(photoId) {
    return this.http.post(this.baseUrl + 'admin/approvePhoto/' + photoId, {});
  }

  rejectPhoto(photoId) {
    return this.http.post(this.baseUrl + 'admin/rejectPhoto/' + photoId, {});
  }

  getCurrentPeriod(): Observable<Period> {
    return this.http.get<Period>(this.baseUrl + 'admin/CurrentPeriod');
  }

  emailExist(email: string) {
    return this.http.get(this.baseUrl + 'admin/' + email + '/VerifyEmail');
  }

  userNameExist(userName: string) {
    return this.http.get(this.baseUrl + 'admin/' + userName + '/VerifyUserName');
  }

  // enregistrement d'un nouveau teacher
  saveTeacher(user: User) {
    return this.http.post(this.baseUrl + 'admin/' + 'AddUser', user);
  }

  getClassLevelsDetails() {
    return this.http.get(this.baseUrl + 'admin/GetClassLevels');
  }

  // levelDetails(levelId: number) {
  //   return this.http.get(this.baseUrl + 'admin/LevelDetails/' + levelId)
  // }

  sendRegisterEmail(user: User) {
    return this.http.post(this.baseUrl + 'admin/' + 'SendRegisterEmail', user);
  }

  updatePerson(id: number, user: any) {
    return this.http.post(this.baseUrl + 'admin/' + id + '/UpdatePerson', user);
  }

   // supprimer un professeur
   deleteTeacher(id: number) {
    return this.http.post(this.baseUrl + 'admin/' + id + '/DeleteTeacher', {});
  }

  searchUsers(data: any) {
    return this.http.post(this.baseUrl + 'admin/SearchUsers', data);
  }

   // métode poour récuperer la liste des types User
   getUserTypes() {
    return this.http.get(this.baseUrl + 'admin/GetUserTypes');
  }

  // recherche des incription en fonction du niveau, du nom ou du prenom
  searchIncription(searchParams) {
    let params = new HttpParams();
    params = params.append('levelId', searchParams.levelId);
    params = params.append('lastName', searchParams.lastName);
    params = params.append('firstName', searchParams.firstName);

    return this.http.get(this.baseUrl + 'admin/SearchInscription', {params});
  }
  studentAffectation(classid, ids) {
   return this.http.post(this.baseUrl + 'admin/' + classid + '/StudentAffectation', ids);
  }

  getLastAdded() {
   return this.http.get(this.baseUrl + 'admin/LastUsersAdded');
  }

  getLastActivated() {
    return this.http.get(this.baseUrl + 'admin/LastUsersActivated');
  }

  getLevelsInscDetails() {
    return this.http.get(this.baseUrl + 'admin/ClassLevelDetails');
  }

  getUsersRecap() {
    return this.http.get(this.baseUrl + 'admin/UsersRecap');
  }

  getUserByTypeId(id: number) {
    return this.http.get(this.baseUrl + 'admin/' + 'GetUserByTypeId/' + id);
  }

  // recupeation des types User pour le personnel
  getAdministrationUserTypes() {
    return this.http.get(this.baseUrl + 'admin/GetAdminUserTypes');
  }

  // enregistrement d'un nouveau teacher
  addUser(user: User) {
    return this.http.post(this.baseUrl + 'admin/' + 'AddUser', user);
  }

  // le post pour enrgister la preinscription : model {father:any,mother:any; children : any[]}
  savePreinscription(userId: number, data: any) {
    return this.http.post(this.baseUrl + 'admin/' + userId + '/SavePreinscription', data);
  }

  getSchool() {
    return this.http.get(this.baseUrl + 'admin/school');
  }

  saveSchool(school: Establishment) {
    return this.http.put(this.baseUrl + 'admin/saveSchool', school);
  }
}
