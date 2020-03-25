import {Injectable} from '@angular/core';
import { Resolve, Router, ActivatedRouteSnapshot } from '@angular/router';
import { AlertifyService } from '../_services/alertify.service';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { EmailTemplate } from '../_models/emailTemplate';
import { CommService } from '../_services/comm.service';

@Injectable()
export class EmailTemplateResolver implements Resolve<EmailTemplate[]> {
  constructor(private commService: CommService, private router: Router, private alertify: AlertifyService) {}

  resolve(route: ActivatedRouteSnapshot): Observable<EmailTemplate[]> {
    return this.commService.getEmailTemplates().pipe(
      catchError(error => {
        this.alertify.error('problème de récupération de données');
        this.router.navigate(['/home']);
        return of(null);
      })
    );
  }
}
