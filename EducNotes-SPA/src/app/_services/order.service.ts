import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { HttpClient } from '@angular/common/http';
import { AlertifyService } from './alertify.service';
import { Router } from '@angular/router';

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

  getTuitionData() {
    return this.http.get(this.baseUrl + 'tuitionData');
  }
}
