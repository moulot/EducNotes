import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { Product } from '../_models/product';
import { ClassLevelProduct } from '../_models/classlevelproduct';
import { Observable } from 'rxjs';
import { DeadLine } from '../_models/deadline';

@Injectable({
  providedIn: 'root'
})
export class TresoService {
  baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  getProductTypes() {
    return this.http.get(this.baseUrl + 'treso/GetProductTypes');
  }

  getDeadlines(): Observable<DeadLine[]> {
    return this.http.get<DeadLine[]>(this.baseUrl + 'treso/GetDeadLines');
  }

  getProducts(): Observable<Product[]> {
    return this.http.get<Product[]>(this.baseUrl + 'treso/GetProducts');
  }

  getDeadline(deadlineId: number): Observable<DeadLine> {
    return this.http.get<DeadLine>(this.baseUrl + 'treso/GetDeadLine/' + deadlineId);
  }

  getProduct(productId: number): Observable<Product> {
    return this.http.get<Product>(this.baseUrl + 'treso/GetProduct/' + productId);
  }

  createProduct(product: Product) {
    return this.http.post(this.baseUrl + 'treso/CreateProduct', product);
  }

  createClasslevelProduct(lvlprod: ClassLevelProduct) {
    return this.http.post(this.baseUrl + 'treso/CreateClassLevelProduct', lvlprod);
  }

  getProductByType(productTypeId: number) {
    return this.http.get(this.baseUrl + 'treso/getProducts/' + productTypeId);
  }

  createlvlProduct(lvlProds) {
    return this.http.post(this.baseUrl + 'treso/CreateClassLevelProduct', lvlProds);
  }

  createDeadLine(deadLine) {
    return this.http.post(this.baseUrl + 'treso/CreateDeadLine', deadLine);
  }

  editDeadLine(deadLineId: number, deadLine) {
   return  this.http.post(this.baseUrl + 'treso/EditDeadLine/' + deadLineId, deadLine);
  }

  editProduct(productId: number, product) {
    return  this.http.post(this.baseUrl + 'treso/EditProduct/' + productId, product);
   }

   getClassLevelServices(levelId: number) {
    return this.http.get(this.baseUrl + 'treso/GetLvlServices/' + levelId);
   }


}
