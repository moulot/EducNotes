import {Injectable} from '@angular/core';
import { Resolve, Router, ActivatedRouteSnapshot } from '@angular/router';
import { User } from '../_models/user';
import { AlertifyService } from '../_services/alertify.service';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { AuthService } from '../_services/auth.service';
@Injectable()
export  class EmailConfirmResolver implements Resolve<User> {

    constructor(private router: Router, private authService: AuthService, private alertify: AlertifyService) {}
    resolve(route: ActivatedRouteSnapshot): Observable<User> {
        return this.authService.confirmEmail(route.params['code']).pipe(
            catchError(error => {
                this.alertify.error(error);
                this.router.navigate(['/Home']);
                return of(null);
            })
        );
    }
}
