import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { HttpClient } from '@angular/common/http';
import { ConfirmEmail } from '../_models/confirmEmail';

@Injectable({
  providedIn: 'root'
})
export class AccountService {
  baseUrl = environment.apiUrl + 'accounts/';

  constructor(private http: HttpClient) { }

  confirmEmail(confirmEmail: ConfirmEmail) {
    return this.http.post(this.baseUrl + 'ConfirmEmail', confirmEmail);
  }

}
