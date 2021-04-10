import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { HttpClient } from '@angular/common/http';
import { Sms } from '../_models/sms';
import { clickatellParams } from '../_models/clickatellParams';
import { SmsTemplate } from '../_models/smsTemplate';
import { Observable } from 'rxjs';
import { EmailTemplate } from '../_models/emailTemplate';
import { DataForBroadcast } from '../_models/dataForBroadcast';

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

  getEmailBroadCastData() {
    return this.http.get(this.baseUrl + 'BroadCastData');
  }

  getEmailBroadcastTemplates() {
    return this.http.get(this.baseUrl + 'EmailBroadcastTemplates');
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

  getSmsBroadcastTemplates() {
    return this.http.get(this.baseUrl + 'SmsBroadcastTemplates');
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

  getBroadcastTokens() {
    return this.http.get(this.baseUrl + 'BroadcastTokens');
  }

  getBroadcastRecap(dataForBroadcast) {
    return this.http.post(this.baseUrl + 'BroadcastRecap', dataForBroadcast);
  }

  sendBroadcastMessages(users) {
    return this.http.post(this.baseUrl + 'sendBroadcastMessages', users);
  }

  UsersBroadcastMessaging(dataForBroadcast) {
    return this.http.post(this.baseUrl + 'UsersBroadcastMessaging', dataForBroadcast);
  }

  ClassesBroadcastMessaging(dataForBroadcast) {
    return this.http.post(this.baseUrl + 'ClassesBroadcastMessaging', dataForBroadcast);
  }

}
