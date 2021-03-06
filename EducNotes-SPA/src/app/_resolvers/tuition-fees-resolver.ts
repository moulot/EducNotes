import {Injectable} from '@angular/core';
import { Resolve, Router, ActivatedRouteSnapshot } from '@angular/router';
import { AlertifyService } from '../_services/alertify.service';
import { of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { TresoService } from '../_services/treso.service';

@Injectable()
export class TuitionFeesResolver implements Resolve<any> {
  constructor(private tresoService: TresoService, private router: Router, private alertify: AlertifyService) {}

  resolve(route: ActivatedRouteSnapshot): any {
    return this.tresoService.getClasslevelProducts().pipe(
      catchError(error => {
        this.alertify.error('problème de récupération de données');
        this.router.navigate(['/home']);
        return of(null);
      })
    );
  }
}
