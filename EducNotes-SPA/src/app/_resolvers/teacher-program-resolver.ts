import {Injectable} from '@angular/core';
import { Resolve, Router, ActivatedRouteSnapshot } from '@angular/router';
import { AlertifyService } from '../_services/alertify.service';
import { of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { AuthService } from '../_services/auth.service';
import { EvaluationService } from '../_services/evaluation.service';
import { ClassService } from '../_services/class.service';

@Injectable()
export class TeacherProgramResolver implements Resolve<any> {
    constructor(private classService: ClassService, private router: Router,
      private alertify: AlertifyService, private authService: AuthService) {}

    resolve(route: ActivatedRouteSnapshot): any {
      const teacherId = this.authService.currentUser.id;
      return this.classService.getTeacherCourseProgram(route.params['courseId'], teacherId).pipe(
        catchError(error => {
          this.alertify.error('problème de récupération de données');
          this.router.navigate(['/home']);
          return of(null);
        })
      );
    }
}
