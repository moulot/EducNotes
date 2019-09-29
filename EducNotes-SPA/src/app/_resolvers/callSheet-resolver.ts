import {Injectable} from '@angular/core';
import { Resolve, Router, ActivatedRouteSnapshot } from '@angular/router';
import { AlertifyService } from '../_services/alertify.service';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { User } from '../_models/user';
import { Schedule } from '../_models/schedule';
import { ClassService } from '../_services/class.service';

@Injectable()
export class CallSheetResolver implements Resolve<Schedule> {
    constructor(private classService: ClassService, private router: Router, private alertify: AlertifyService) {}

    resolve(route: ActivatedRouteSnapshot): Observable<Schedule> {
        return this.classService.getSchedule(route.params['id']).pipe(
            catchError(error => {
                this.alertify.error('problème de récupération de données');
                this.router.navigate(['/home']);
                return of(null);
            })
        );
    }
}
