import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ProductService {
  baseUrl = environment.apiUrl + 'products/';

  constructor(private http: HttpClient) { }

  getProducts() {
    return this.http.get(this.baseUrl);
  }

  getServices() {
    return this.http.get(this.baseUrl + 'Services');
  }

  getProduct(id) {
    return this.http.get(this.baseUrl + id);
  }

  getProductTypes() {
    return this.http.get(this.baseUrl + 'ProductTypes');
  }

  getLevelPrices(productId) {
    return this.http.get(this.baseUrl + 'LevelPrices/' + productId);
  }

  getZonePrices(productId) {
    return this.http.get(this.baseUrl + 'ZonePrices/' + productId);
  }

  getPeriodicities() {
    return this.http.get(this.baseUrl + 'Periodicities');
  }

  getPayableAts() {
    return this.http.get(this.baseUrl + 'PayableAts');
  }

  getProductData() {
    return this.http.get(this.baseUrl + 'ProductData');
  }

}
