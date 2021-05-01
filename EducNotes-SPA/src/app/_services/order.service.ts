import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { HttpClient } from '@angular/common/http';
import { AlertifyService } from './alertify.service';
import { Router } from '@angular/router';
import { TuitionData } from '../_models/tuitionData';

@Injectable({
  providedIn: 'root'
})
export class OrderService {
  baseUrl = environment.apiUrl + 'orders/';

  constructor(private http: HttpClient, private alertify: AlertifyService,
    private router: Router) { }

  getOrder(id: number) {
    return this.http.get(this.baseUrl + id);
  }

  getBalanceData() {
    return this.http.get(this.baseUrl + 'BalanceData');
  }

  getRecoveryData() {
    return this.http.get(this.baseUrl + 'RecoveryData');
  }

  getTuitionData() {
    return this.http.get(this.baseUrl + 'tuitionOrderData');
  }

  addNewTuition(tuition: TuitionData) {
    return this.http.post(this.baseUrl + 'NewTuition', tuition);
  }

  getTuitionFromChild(childid) {
    return this.http.get(this.baseUrl + 'tuitionFromChild/' + childid);
  }

  getTuitionFigures() {
    return this.http.get(this.baseUrl + 'TuitionFigures');
  }

  getTuitionList() {
    return this.http.get(this.baseUrl + 'TuitionList');
  }

  getOrderAmountByDeadline() {
    return this.http.get(this.baseUrl + 'AmountByDeadline');
  }

  getLevelLatePayments() {
    return this.http.get(this.baseUrl + 'LevelLatePayments');
  }

  getChildLatePayments() {
    return this.http.get(this.baseUrl + 'ChildLatePayments');
  }

  getChildLatePaymentByLevel(levelid) {
    return this.http.get(this.baseUrl + 'ChildLatePaymentByLevel/' + levelid);
  }

  getLastTuitions() {
    return this.http.get(this.baseUrl + 'LastTuitions');
  }

  // getLastTuitionsValidated() {
  //   return this.http.get(this.baseUrl + 'LastTuitionsValidated');
  // }

}
