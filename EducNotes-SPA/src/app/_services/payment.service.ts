import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class PaymentService {
  baseUrl = environment.apiUrl + 'payments/';

  constructor(private http: HttpClient) { }

  getPaymentTypes() {
    return this.http.get(this.baseUrl + 'GetPaymentTypes');
  }

  getPaymentData() {
    return this.http.get(this.baseUrl + 'PaymentData');
  }

}
