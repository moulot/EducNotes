import {Injectable} from '@angular/core';
import { Resolve, Router, ActivatedRouteSnapshot } from '@angular/router';
import { AlertifyService } from '../_services/alertify.service';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { CommService } from '../_services/comm.service';
import { EmailTemplate } from '../_models/emailTemplate';

@Injectable()
export class EditEmailTemplateResolver implements Resolve<EmailTemplate> {
    constructor(private commService: CommService, private router: Router, private alertify: AlertifyService) {}

    resolve(route: ActivatedRouteSnapshot): Observable<EmailTemplate> {
      console.log(route.params['id']);
        return this.commService.getEmailTemplateById(route.params['id']).pipe(
            catchError(error => {
                this.alertify.error('problème de récupération de données');
                this.router.navigate(['/home']);
                return of(null);
            })
        );
    }
}
