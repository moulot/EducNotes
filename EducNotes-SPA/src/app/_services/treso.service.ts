import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { Product } from '../_models/product';
import { ClassLevelProduct } from '../_models/classlevelproduct';
import { Observable } from 'rxjs';
import { DeadLine } from '../_models/deadline';
import { Periodicity } from '../_models/periodicity';
import { PayableAt } from '../_models/payable-at';

@Injectable({
  providedIn: 'root'
})
export class TresoService {
  baseUrl = environment.apiUrl;
  recouvrementTypes  = [
    {value : 1 , label : 'par échéance'},
    {value : 2 , label : 'périodique'}
  ];

  billingTypes  = [
    {value : 1 , label : 'montant unique'},
    {value : 2 , label : 'montant par niveau'}
  ];

  constructor(private http: HttpClient) { }

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

  getPeriodicities(): Observable<Periodicity[]> {
    return this.http.get<Periodicity[]>(this.baseUrl + 'treso/GetPeriodicities');
  }

  getPayableAts(): Observable<PayableAt[]> {
    return this.http.get<PayableAt[]>(this.baseUrl + 'treso/GetPayableAts');
  }

  getPeriodicity(periodicityId: number): Observable<Periodicity> {
    return this.http.get<Periodicity>(this.baseUrl + 'treso/GetPeriodicity/' + periodicityId);
  }

  getPayable(payableId: number): Observable<PayableAt> {
    return this.http.get<PayableAt>(this.baseUrl + 'treso/GetPayableAt/' + payableId);
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
    return this.http.post(this.baseUrl + 'treso/EditDeadLine/' + deadLineId, deadLine);
  }

  editProduct(productId: number, product) {
    return this.http.post(this.baseUrl + 'treso/EditProduct/' + productId, product);
  }

  getClassLevelServices(levelId: number) {
    return this.http.get(this.baseUrl + 'treso/GetLvlServices/' + levelId);
  }

  createPeriodicity(periodicityName: string) {
    return this.http.post(this.baseUrl + 'treso/CreatePeriodicity/' + periodicityName , {});
  }

  createPayableAt(payableAt: PayableAt) {
    return this.http.post(this.baseUrl + 'treso/CreatePayableAt' , payableAt);
  }

  EditPeriodicity(id: number, periodicityName: string) {
    return this.http.post(this.baseUrl + 'treso/EditPeriodicity/' + id + '/' + periodicityName , {});
  }

  EditPayableAt(id: number, payableAt: any) {
    return this.http.post(this.baseUrl + 'treso/EditPayableAt/' + id  , payableAt);
  }


}
