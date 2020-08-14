import {Injectable} from '@angular/core';
import { Resolve, Router, ActivatedRouteSnapshot } from '@angular/router';
import { AlertifyService } from '../_services/alertify.service';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { Setting } from '../_models/setting';
import { AdminService } from '../_services/admin.service';

@Injectable()
export class SchoolSettingsResolver implements Resolve<Setting[]> {
  constructor(private adminService: AdminService, private router: Router, private alertify: AlertifyService) {}

  resolve(route: ActivatedRouteSnapshot): Observable<Setting[]> {
    return this.adminService.getSettings().pipe(
      catchError(error => {
        this.alertify.error('problème de récupération de données');
        this.router.navigate(['/home']);
        return of(null);
      })
    );
  }
}
