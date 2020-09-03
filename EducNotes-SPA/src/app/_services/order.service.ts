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
    return this.http.get(this.baseUrl + 'balanceData');
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
}
