import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { HttpClient, HttpParams } from '@angular/common/http';
import { User } from '../_models/user';
import { Setting } from '../_models/setting';
import { DataForEmail } from '../_models/dataForEmail';

@Injectable({
  providedIn: 'root'
})
export class AdminService {
  baseUrl = environment.apiUrl;
  sexe = [{ id: 0, name: ' FEMME' }, { id: 1, name: ' HOMME' }];

  constructor(private http: HttpClient) { }

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

  emailExist(email: string) {
    return this.http.get(this.baseUrl + 'admin/' + email + '/VerifyEmail');
  }

  userNameExist(userName: string) {
    return this.http.get(this.baseUrl + 'admin/' + userName + '/VerifyUserName');
  }

  searchUsers(data: any) {
    return this.http.post(this.baseUrl + 'admin/SearchUsers', data);
  }

  // recherche des incription en fonction du niveau, du nom ou du prenom
  searchIncription(searchParams) {
    let params = new HttpParams();
    params = params.append('levelId', searchParams.levelId);
    params = params.append('lastName', searchParams.lastName);
    params = params.append('firstName', searchParams.firstName);

    return this.http.get(this.baseUrl + 'admin/SearchInscription', { params });
  }

  studentAffectation(classid, ids) {
    return this.http.post(this.baseUrl + 'admin/' + classid + '/StudentAffectation', ids);
  }

  // enregistrement des professeurs extraits du fichier excel
  importTeachersFile(teachers: User[]) {
    return this.http.post(this.baseUrl + 'admin/ImportTeachers', teachers);
  }

  saveImportedUsers(importedUsers, insertUserId) {
    return this.http.post(this.baseUrl + 'admin/ImportedUsers/' + insertUserId, importedUsers);
  }

  getSettings() {
    return this.http.get(this.baseUrl + 'admin/Settings');
  }

  updateSettings(settings: Setting[]) {
    return this.http.post(this.baseUrl + 'admin/UpdateSettings', settings);
  }

}
