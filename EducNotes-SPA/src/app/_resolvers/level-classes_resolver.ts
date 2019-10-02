import {Injectable} from '@angular/core';
import { Resolve, Router, ActivatedRouteSnapshot } from '@angular/router';
import { AlertifyService } from '../_services/alertify.service';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { ClassService } from '../_services/class.service';

@Injectable()
export class LevelClassesResolver implements Resolve<any> {

    constructor(private router: Router, private classService: ClassService, private alertify: AlertifyService) {}
    resolve(route: ActivatedRouteSnapshot): any {
        return this.classService.getClassesByLevelId(route.params['levelId']).pipe(
            catchError(error => {
                this.alertify.error(error);
                this.router.navigate(['/Home']);
                return of(null);
            })
        );
    }
}
