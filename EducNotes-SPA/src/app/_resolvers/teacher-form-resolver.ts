import {Injectable} from '@angular/core';
import { Resolve, Router, ActivatedRouteSnapshot } from '@angular/router';
import { AlertifyService } from '../_services/alertify.service';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { UserService } from '../_services/user.service';

@Injectable()
export class TeacherFormResolver implements Resolve<any> {

  constructor( private router: Router, private userService: UserService, private alertify: AlertifyService) {}

  resolve(route: ActivatedRouteSnapshot): Observable<any> {
    return this.userService.getAssignedClasses(route.params['id']).pipe(
      catchError(() => {
        this.alertify.error('problème de récupération de données');
        this.router.navigate(['/home']);
        return of(null);
      })
    );
  }
}
