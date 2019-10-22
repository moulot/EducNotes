import {Injectable} from '@angular/core';
import { Resolve, Router, ActivatedRouteSnapshot } from '@angular/router';
import { AlertifyService } from '../_services/alertify.service';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
 import { Course } from '../_models/course';
import { ClassService } from '../_services/class.service';

@Injectable()
export class CourseFormResolver implements Resolve<Course> {

    constructor( private router: Router, private classService: ClassService,
         private alertify: AlertifyService) {}

    resolve(route: ActivatedRouteSnapshot): Observable<Course> {
        return this.classService.getCourse(route.params['id']).pipe(
            catchError(() => {
                this.alertify.error('problème de récupération de données');
                this.router.navigate(['/home']);
                return of(null);
            })
        );
    }
}