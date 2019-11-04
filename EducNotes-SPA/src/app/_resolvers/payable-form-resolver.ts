import { Injectable } from '@angular/core';
import { Resolve, Router, ActivatedRouteSnapshot } from '@angular/router';
import { AlertifyService } from '../_services/alertify.service';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { TresoService } from '../_services/treso.service';
import { PayableAt } from '../_models/payable-at';

@Injectable()
export class PayableFormResolver implements Resolve<PayableAt> {

    constructor(private tresoService: TresoService, private router: Router,
        private alertify: AlertifyService) { }

    resolve(route: ActivatedRouteSnapshot): Observable<PayableAt> {
        return this.tresoService.getPayable(route.params['id']).pipe(
            catchError(() => {
                this.alertify.error('problème de récupération de données');
                this.router.navigate(['/home']);
                return of(null);
            })
        );
    }
}
