import { Injectable } from '@angular/core';
import { Resolve, Router, ActivatedRouteSnapshot } from '@angular/router';
import { AlertifyService } from '../_services/alertify.service';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { DeadLine } from '../_models/deadline';
import { TresoService } from '../_services/treso.service';

@Injectable()
export class DeadLineListResolver implements Resolve<DeadLine[]> {

    constructor(private tresoService: TresoService, private router: Router,
        private alertify: AlertifyService) { }

    resolve(route: ActivatedRouteSnapshot): Observable<DeadLine[]> {
        return this.tresoService.getDeadlines().pipe(
            catchError(() => {
                this.alertify.error('problème de récupération de données');
                this.router.navigate(['/home']);
                return of(null);
            })
        );
    }
}