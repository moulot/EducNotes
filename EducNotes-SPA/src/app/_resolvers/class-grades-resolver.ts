import {Injectable} from '@angular/core';
import { Resolve, Router, ActivatedRouteSnapshot } from '@angular/router';
import { AlertifyService } from '../_services/alertify.service';
import { of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { AuthService } from '../_services/auth.service';
import { EvaluationService } from '../_services/evaluation.service';

@Injectable()
export class ClassGradesResolver implements Resolve<any> {
    constructor(private evalService: EvaluationService, private authService: AuthService,
        private router: Router, private alertify: AlertifyService) {}

    resolve(route: ActivatedRouteSnapshot): any {
        return this.evalService.getClassEval(route.params['evalId']).pipe(
            catchError(error => {
                this.alertify.error('problème de récupération de données');
                this.router.navigate(['/home']);
                return of(null);
            })
        );
    }
}
