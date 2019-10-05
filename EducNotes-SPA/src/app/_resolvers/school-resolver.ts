import {Injectable} from '@angular/core';
import { Resolve, Router, ActivatedRouteSnapshot } from '@angular/router';
import { AlertifyService } from '../_services/alertify.service';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { Establishment } from '../_models/establishment';
import { AdminService } from '../_services/admin.service';

@Injectable()
export class SchoolResolver implements Resolve<Establishment> {

    constructor(private adminService: AdminService,
        private router: Router, private alertify: AlertifyService) {}

    resolve(route: ActivatedRouteSnapshot): Observable<Establishment> {
        return this.adminService.getSchool()
            .pipe(
            catchError(() => {
                this.alertify.error('problème de récupération de données');
                this.router.navigate(['/home']);
                return of(null);
            })
        );
    }
}
