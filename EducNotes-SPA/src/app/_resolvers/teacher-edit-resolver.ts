import {Injectable} from '@angular/core';
import { Resolve, Router, ActivatedRouteSnapshot } from '@angular/router';
import { AlertifyService } from '../_services/alertify.service';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { ClassService } from '../_services/class.service';

@Injectable()
export class MemberEditResolver implements Resolve<any> {
    constructor(private classService: ClassService,
        private router: Router, private alertify: AlertifyService) {}

    resolve(route: ActivatedRouteSnapshot): Observable<any> {
        return this.classService.getTeacher(route.params['id']).pipe(
            catchError(error => {
                this.alertify.error('problem retrieving data');
                this.router.navigate(['/members']);
                return of(null);
            })
        );
    }
}
