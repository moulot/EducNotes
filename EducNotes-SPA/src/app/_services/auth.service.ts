import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject } from 'rxjs';
import {map} from 'rxjs/operators';
import {JwtHelperService} from '@auth0/angular-jwt';
import { environment } from 'src/environments/environment';
import { User } from '../_models/user';
import { Period } from '../_models/period';
import { Router } from '@angular/router';
import { AlertifyService } from './alertify.service';


@Injectable({
  providedIn: 'root'
})
export class AuthService {
  baseUrl = environment.apiUrl + 'auth/';
  parentTypeId = environment.parentTypeId;
  studentTypeId = environment.studentTypeId;
  teacherTypeId = environment.teacherTypeId;
  adminTypeId = environment.adminTypeId;
  jwtHelper = new JwtHelperService();
  decodedToken: any;
  currentUser: User;
  newUser: User;
  currentPeriod: Period;

  photoUrl = new BehaviorSubject<string>('../../assets/user.png');
  currentPhotoUrl = this.photoUrl.asObservable();

  constructor(private http: HttpClient, private alertify: AlertifyService,
    private router: Router) { }

  changeMemberPhoto(photoUrl: string) {
    this.photoUrl.next(photoUrl);
  }

  login(model: any) {
    return this.http.post(this.baseUrl + 'login', model)
      .pipe(
        map((response: any) => {
          const user = response;
          if (user) {
            localStorage.setItem('token', user.token);
            localStorage.setItem('user', JSON.stringify(user.user));
            localStorage.setItem('currentPeriod', JSON.stringify(user.currentPeriod));
            this.decodedToken = this.jwtHelper.decodeToken(user.token);
            this.currentUser = user.user;
            this.currentPeriod = user.currentPeriod;
            this.changeMemberPhoto(this.currentUser.photoUrl);
          }
        })
      );
  }

  logout() {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    localStorage.removeItem('currentPeriod');
    this.decodedToken = null;
    this.currentUser = null;
    this.currentPeriod = null;
    this.alertify.infoBar('vous êtes déconnecté');
    this.router.navigate(['/']);
  }

  register(user: User) {
    return this.http.post(this.baseUrl + 'register', user);
  }

  setUserPassword(id: number, password: string) {
     return this.http.post(this.baseUrl + id + '/setPassword/' + password, {})
    .pipe(
      map((response: any) => {
        const user = response;
        if (user) {
          localStorage.setItem('token', user.token);
          localStorage.setItem('user', JSON.stringify(user.user));
          this.decodedToken = this.jwtHelper.decodeToken(user.token);
          this.currentUser = user.user;
          this.changeMemberPhoto(this.currentUser.photoUrl);
        }
      })
    );
  }



  setUserLoginPassword(id: number, loginModel: any) {
    return this.http.post(this.baseUrl  + id +  '/setLoginPassword', loginModel)
   .pipe(
     map((response: any) => {
       const user = response;
       if (user) {
         localStorage.setItem('token', user.token);
         localStorage.setItem('user', JSON.stringify(user.user));
         this.decodedToken = this.jwtHelper.decodeToken(user.token);
         this.currentUser = user.user;
         this.changeMemberPhoto(this.currentUser.photoUrl);
       }
     })
   );
 }

  forgotPassord(email: string) {
    return this.http.get(this.baseUrl + email + '/ForgotPassword');
  }

  codeValidation(model: any) {
    return this.http.post(this.baseUrl + 'codeValidation', model);
  }
  loggedIn() {
    const token = localStorage.getItem('token');
    return !this.jwtHelper.isTokenExpired(token);
  }

  parentLoggedIn() {
    return this.currentUser.userTypeId === this.parentTypeId;
  }

  studentLoggedIn() {
    return this.currentUser.userTypeId === this.studentTypeId;
  }

  teacherLoggedIn() {
    return this.currentUser.userTypeId === this.teacherTypeId;
  }

  adminLoggedIn() {
    return this.currentUser.userTypeId === this.adminTypeId;
  }

  roleMatch(allowedRoles): boolean {
    let isMatch = false;
    const userRoles = this.decodedToken.role as Array<string>;
    allowedRoles.forEach(element => {
      if (userRoles.includes(element)) {
        isMatch = true;
        return;
      }
    });
    return isMatch;
  }

  confirmemail(token: string) {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    this.decodedToken = null;
    this.currentUser = null;
    return this.http.get(this.baseUrl  + 'emailValidation/' + token);

  }

  resetPassword(token: string) {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    this.decodedToken = null;
    this.currentUser = null;
    return this.http.get(this.baseUrl  + 'ResetPassword/' + token);

  }

  finaliseRegistration(model: User) {
    return this.http.post(this.baseUrl + 'LastValidation', model)
      .pipe(
        map((response: any) => {
          const user = response;
          if (user) {
            localStorage.setItem('token', user.token);
            localStorage.setItem('user', JSON.stringify(user.user));
            this.decodedToken = this.jwtHelper.decodeToken(user.token);
            this.currentUser = user.user;
            this.changeMemberPhoto(this.currentUser.photoUrl);
          }
        })
      );
  }

  getEmails() {
    return this.http.get(this.baseUrl + 'GetEmails');
  }
  getUserNames() {
   return this.http.get(this.baseUrl + 'GetUserNames');
  }

  getAllCities() {
    return this.http.get(this.baseUrl + 'GetAllCities');
  }

  getLevels() {
    return this.http.get(this.baseUrl + 'GetLevels');
  }

  getDistrictsByCityId(id: number) {
    return this.http.get(this.baseUrl + 'GetDistrictsByCityId/' + id);
  }

   // le post pour enrgister la preinscription : model {father:any,mother:any; children : any[]}
  parentSelfPreinscription(userId: number, data: any) {
    return this.http.post(this.baseUrl + userId + '/ParentSelfPreinscription', data)
    .pipe(
      map((response: any) => {
        const user = response;
        if (user) {
          localStorage.setItem('token', user.token);
          localStorage.setItem('user', JSON.stringify(user.user));
          localStorage.setItem('currentPeriod', JSON.stringify(user.currentPeriod));
          this.decodedToken = this.jwtHelper.decodeToken(user.token);
          this.currentUser = user.user;
          this.currentPeriod = user.currentPeriod;
          this.changeMemberPhoto(this.currentUser.photoUrl);
        }
      })
    );
  }

  teacherSelfPreinscription(userId: number, data: any) {
    return this.http.post(this.baseUrl + userId + '/TeacherSelfPreinscription', data)
    .pipe(
      map((response: any) => {
        const user = response;
        if (user) {
          localStorage.setItem('token', user.token);
          localStorage.setItem('user', JSON.stringify(user.user));
          localStorage.setItem('currentPeriod', JSON.stringify(user.currentPeriod));
          this.decodedToken = this.jwtHelper.decodeToken(user.token);
          this.currentUser = user.user;
          this.currentPeriod = user.currentPeriod;
          this.changeMemberPhoto(this.currentUser.photoUrl);
        }
      })
    );
  }

  emailExist(email: string) {
    return this.http.get(this.baseUrl +  email + '/VerifyEmail');
  }

  userNameExist(userName: string) {
    return this.http.get(this.baseUrl +  userName + '/VerifyUserName');
  }

}
