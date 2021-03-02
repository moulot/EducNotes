import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { HttpClient } from '@angular/common/http';
import { ConfirmToken } from '../_models/confirmToken';

@Injectable({
  providedIn: 'root'
})
export class AccountService {
  baseUrl = environment.apiUrl + 'accounts/';

  constructor(private http: HttpClient) { }

  resetPassword(userData: ConfirmToken) {
    return this.http.post(this.baseUrl + 'ResetPassword', userData);
  }

  confirmEmail(confirmEmail: ConfirmToken) {
    return this.http.post(this.baseUrl + 'ConfirmEmail', confirmEmail);
  }

  sendPhoneNumberToValidate(userId, num) {
    return this.http.get(this.baseUrl + 'PhoneCode/' + userId + '/' + num);
  }

  sendCodeToEmail(confirmEmail: ConfirmToken) {
    return this.http.post(this.baseUrl + 'EmailCode', confirmEmail);
  }

  sendPwdCodeToSms(confirmEmail: ConfirmToken) {
    return this.http.post(this.baseUrl + 'PwdCode', confirmEmail);
  }

  validatePhone(confirmEmail: ConfirmToken) {
    return this.http.post(this.baseUrl + 'ConfirmPhoneNumber', confirmEmail);
  }

  editPhoneNumber(confirmToken: ConfirmToken) {
    return this.http.post(this.baseUrl + 'EditPhoneNumber', confirmToken);
  }

  editEmail(confirmToken: ConfirmToken) {
    return this.http.post(this.baseUrl + 'EditEmail', confirmToken);
  }

  editPwd(confirmToken: ConfirmToken) {
    return this.http.post(this.baseUrl + 'EditPwd', confirmToken);
  }

}
