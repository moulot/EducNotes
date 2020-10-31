import {Injectable} from '@angular/core';
import { Resolve, Router, ActivatedRouteSnapshot } from '@angular/router';
import { UserService } from '../_services/user.service';
import { AlertifyService } from '../_services/alertify.service';
import { of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { AuthService } from '../_services/auth.service';

@Injectable()
export class ParentFileResolver implements Resolve<any> {
  constructor(private userService: UserService, private authService: AuthService,
    private router: Router, private alertify: AlertifyService) {}

  resolve(route: ActivatedRouteSnapshot): any {
    return this.userService.getParentFile(route.params['id']).pipe(
      catchError(error => {
        this.alertify.error('problème de récupération de données');
        this.router.navigate(['/home']);
        return of(null);
      })
    );
  }
}
