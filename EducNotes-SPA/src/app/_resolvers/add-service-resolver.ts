import {Injectable} from '@angular/core';
import { Resolve, Router, ActivatedRouteSnapshot } from '@angular/router';
import { AlertifyService } from '../_services/alertify.service';
import { of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { ProductService } from '../_services/product.service';

@Injectable()
export class AddServiceResolver implements Resolve<any> {
    constructor(private productService: ProductService, private router: Router, private alertify: AlertifyService) {}

    resolve(route: ActivatedRouteSnapshot): any {
        return this.productService.getProduct(route.params['id']).pipe(
            catchError(() => {
                this.alertify.error('problème de récupération de données initiales');
                this.router.navigate(['/home']);
                return of(null);
            })
        );
    }
}
