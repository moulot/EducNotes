import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { HttpClient } from '@angular/common/http';
import { ConfirmEmailPhone } from '../_models/confirmEmailPhone';

@Injectable({
  providedIn: 'root'
})
export class AccountService {
  baseUrl = environment.apiUrl + 'accounts/';

  constructor(private http: HttpClient) { }

  confirmEmail(confirmEmail: ConfirmEmailPhone) {
    return this.http.post(this.baseUrl + 'ConfirmEmail', confirmEmail);
  }

  sendPhoneNumberToValidate(userId, num) {
    return this.http.get(this.baseUrl + 'PhoneCode/' + userId + '/' + num);
  }

  validtePhone(confirmEmail: ConfirmEmailPhone) {
    return this.http.post(this.baseUrl + 'ConfirmPhoneNumber', confirmEmail);
  }

}
