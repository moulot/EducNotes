import {Injectable} from '@angular/core';
import { Resolve, Router, ActivatedRouteSnapshot } from '@angular/router';
import { AlertifyService } from '../_services/alertify.service';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { SmsTemplate } from '../_models/smsTemplate';
import { CommService } from '../_services/comm.service';

@Injectable()
export class EditSmsTemplateResolver implements Resolve<SmsTemplate> {
    constructor(private commService: CommService, private router: Router, private alertify: AlertifyService) {}

    resolve(route: ActivatedRouteSnapshot): Observable<SmsTemplate> {
        return this.commService.getSmsTemplateById(route.params['id']).pipe(
            catchError(error => {
                this.alertify.error('problème de récupération de données');
                this.router.navigate(['/home']);
                return of(null);
            })
        );
    }
}
