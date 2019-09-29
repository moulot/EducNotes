import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Period } from '../_models/period';
import { environment } from 'src/environments/environment';
import { EvalType } from '../_models/evalType';
import { Evaluation } from '../_models/evaluation';
import { UserEvaluation } from '../_models/userEvaluation';

@Injectable({
  providedIn: 'root'
})
export class EvaluationService {
  baseUrl = environment.apiUrl;
  currentEval: any;
  userGrades: any = [];
  gradeIndex: number;

  constructor(private http: HttpClient) { }

  getPeriods(): Observable<Period[]> {
    return this.http.get<Period[]>(this.baseUrl + 'evaluation/Periods');
  }

  getEvalTypes(): Observable<EvalType[]> {
    return this.http.get<EvalType[]>(this.baseUrl + 'evaluation/EvalTypes');
  }

  getCoursesSkills() {
    return this.http.get(this.baseUrl + 'evaluation/CoursesSkills');
  }

  saveEvaluation(evaluation: Evaluation, evalProgEltIds: string) {

    let params = new HttpParams();
    params = params.append('progEltIds', evalProgEltIds);

    return this.http.put(this.baseUrl + 'evaluation/saveEvaluation', evaluation, {params});
  }

  saveUserGrades(userGrades: UserEvaluation[]) {
    return this.http.put(this.baseUrl + 'evaluation/saveUserGrades', userGrades);
  }

  getUserEvals(classId, courseId, periodId) {
    return this.http.get(this.baseUrl + 'evaluation/class/' + classId + '/course/' + courseId + '/period/' + periodId);
  }

  getClassEvalsToCome(classId) {
    return this.http.get(this.baseUrl + 'evaluation/class/' + classId + '/evalsToCome');
  }

  getClassEvals(classId) {
    return this.http.get(this.baseUrl + 'evaluation/class/' + classId + '/AllEvaluations');
  }

  setCurrentCurrentEval(evaluation, userGrades) {
    this.currentEval = evaluation;
    this.userGrades = userGrades;
  }

  setColIndex(index: number) {
    this.gradeIndex = index;
  }
}
