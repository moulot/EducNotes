import {Injectable} from '@angular/core';
import { Resolve, Router, ActivatedRouteSnapshot } from '@angular/router';
import { AlertifyService } from '../_services/alertify.service';
import { of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { OrderService } from '../_services/order.service';

@Injectable()
export class PaymentLevelChildResolver implements Resolve<any> {
  constructor(private orderService: OrderService, private router: Router, private alertify: AlertifyService) {}

  resolve(route: ActivatedRouteSnapshot): any {
    return this.orderService.getChildLatePaymentByLevel(route.params['id']).pipe(
      catchError(error => {
        this.alertify.error('problème de récupération de données');
        this.router.navigate(['/home']);
        return of(null);
      })
    );
  }
}
