import {Injectable} from '@angular/core';
import { Resolve, Router, ActivatedRouteSnapshot } from '@angular/router';
import { AlertifyService } from '../_services/alertify.service';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { Schedule } from '../_models/schedule';
import { ClassService } from '../_services/class.service';

@Injectable()
export class CallSheetResolver implements Resolve<any> {
    constructor(private classService: ClassService, private router: Router, private alertify: AlertifyService) {}

    resolve(route: ActivatedRouteSnapshot): any {
        return this.classService.getSessionData(route.params['id']).pipe(
            catchError(() => {
                this.alertify.error('problème de récupération de données');
                this.router.navigate(['/home']);
                return of(null);
            })
        );
    }
}
