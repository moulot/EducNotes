import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { HttpClient } from '@angular/common/http';
import { Sms } from '../_models/sms';
import { clickatellParams } from '../_models/clickatellParams';
import { SmsTemplate } from '../_models/smsTemplate';
import { Observable } from 'rxjs';
import { EmailTemplate } from '../_models/emailTemplate';

@Injectable({
  providedIn: 'root'
})
export class CommService {
  baseUrl = environment.apiUrl + 'comm/';

  constructor(private http: HttpClient) { }

  // sendSms(data: Sms) {
  //   return this.http.post(this.baseUrl + 'sendSms', data);
  // }
  sendSms(data: clickatellParams) {
    return this.http.post(this.baseUrl + 'sendSms', data);
  }

  getEmailCategories() {
    return this.http.get(this.baseUrl + 'EmailCategories');
  }

  getSmsCategories() {
    return this.http.get(this.baseUrl + 'SmsCategories');
  }

  getEmailTemplates() {
    return this.http.get(this.baseUrl + 'EmailTemplates');
  }

  getEmailTemplateById(id) {
    return this.http.get(this.baseUrl + 'EmailTemplates/' + id);
  }

  saveEmailTemplate(emailTemplate: EmailTemplate) {
    return this.http.put(this.baseUrl + 'SaveEmailTemplate', emailTemplate);
  }

  getSmsTemplates() {
    return this.http.get(this.baseUrl + 'SmsTemplates');
  }

  getSmsTemplateById(id) {
    return this.http.get(this.baseUrl + 'SmsTemplates/' + id);
  }

  saveSmsTemplate(smsTemplate: SmsTemplate) {
    return this.http.put(this.baseUrl + 'SaveSmsTemplate', smsTemplate);
  }

  getSmsByCategory() {
    return this.http.get(this.baseUrl + 'SmsByCategory');
  }

  getTokens() {
    return this.http.get(this.baseUrl + 'Tokens');
  }

}
