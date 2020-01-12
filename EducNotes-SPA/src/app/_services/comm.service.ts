import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { HttpClient } from '@angular/common/http';
import { Sms } from '../_models/sms';
import { clickatellParams } from '../_models/clickatellParams';
import { SmsTemplate } from '../_models/smsTemplate';

@Injectable({
  providedIn: 'root'
})
export class CommService {
  baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) { }

  // sendSms(data: Sms) {
  //   return this.http.post(this.baseUrl + 'comm/sendSms', data);
  // }
  sendSms(data: clickatellParams) {
    return this.http.post(this.baseUrl + 'comm/sendSms', data);
  }

  getSmsCategories() {
    return this.http.get(this.baseUrl + 'comm/SmsCategories');
  }

  getSmsTemplates() {
    return this.http.get(this.baseUrl + 'comm/SmsTemplates');
  }

  saveSmsTemplate(smsTemplate: SmsTemplate) {
    return this.http.put(this.baseUrl + 'comm/SaveSmsTemplate', smsTemplate);
  }

}
