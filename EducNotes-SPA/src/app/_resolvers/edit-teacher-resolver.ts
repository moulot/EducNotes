import { Injectable } from '@angular/core';
import { Resolve, Router, ActivatedRouteSnapshot } from '@angular/router';
import { AlertifyService } from '../_services/alertify.service';
import { of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { UserService } from '../_services/user.service';

@Injectable()
export class EditTeacherResolver implements Resolve<any> {
    constructor(private router: Router, private userService: UserService, private alertify: AlertifyService) { }
    resolve(route: ActivatedRouteSnapshot): any {
        return this.userService.getTeacherWithCourses(route.params['id']).pipe(
            catchError(error => {
                this.alertify.error(error);
                this.router.navigate(['/Home']);
                return of(null);
            })
        );
    }
}
