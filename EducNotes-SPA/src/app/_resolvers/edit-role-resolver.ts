import { Injectable } from '@angular/core';
import { Resolve, Router, ActivatedRouteSnapshot } from '@angular/router';
import { AlertifyService } from '../_services/alertify.service';
import { of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { AdminService } from '../_services/admin.service';

@Injectable()
export class EditRoleResolver implements Resolve<any> {
  constructor(private router: Router, private adminService: AdminService, private alertify: AlertifyService) { }
  resolve(route: ActivatedRouteSnapshot): any {
    return this.adminService.getRoleWithUsers(route.params['id']).pipe(
      catchError(error => {
        console.log(error);
        this.router.navigate(['/Home']);
        return of(null);
      })
    );
  }
}
