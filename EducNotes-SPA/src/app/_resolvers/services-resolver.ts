import {Injectable} from '@angular/core';
import { Resolve, Router, ActivatedRouteSnapshot } from '@angular/router';
import { AlertifyService } from '../_services/alertify.service';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { ProductService } from '../_services/product.service';


@Injectable()
export class ServicesResolver implements Resolve<any> {
    constructor(private productService: ProductService, private router: Router,
      private alertify: AlertifyService) { }

    resolve(): Observable<any> {
      return this.productService.getServices().pipe(
        catchError(() => {
          this.alertify.error('problème pour récupérer les données');
          this.router.navigate(['/home']);
          return of(null);
        })
      );
    }
}
