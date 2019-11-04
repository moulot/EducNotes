import { Injectable } from '@angular/core';
import { Resolve, Router, ActivatedRouteSnapshot } from '@angular/router';
import { AlertifyService } from '../_services/alertify.service';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { TresoService } from '../_services/treso.service';
import { Periodicity } from '../_models/periodicity';

@Injectable()
export class PeriodicitiesListResolver implements Resolve<Periodicity[]> {

    constructor(private tresoService: TresoService, private router: Router,
        private alertify: AlertifyService) { }

    resolve(route: ActivatedRouteSnapshot): Observable<Periodicity[]> {
        return this.tresoService.getPeriodicities().pipe(
            catchError(() => {
                this.alertify.error('problème de récupération de données');
                this.router.navigate(['/home']);
                return of(null);
            })
        );
    }
}
