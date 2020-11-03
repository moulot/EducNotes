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
  baseUrl = environment.apiUrl + 'evaluation/';
  currentEval: any;
  userGrades: any = [];
  gradeIndex: number;

  constructor(private http: HttpClient) { }

  getEvalTypes(): Observable<EvalType[]> {
    return this.http.get<EvalType[]>(this.baseUrl + 'EvalTypes');
  }

  getFormData() {
    return this.http.get(this.baseUrl + 'FormData');
  }

  getCoursesSkills() {
    return this.http.get(this.baseUrl + 'CoursesSkills');
  }

  saveEvaluation(evaluation: Evaluation, evalProgEltIds: string) {
    let params = new HttpParams();
    params = params.append('progEltIds', evalProgEltIds);
    return this.http.put(this.baseUrl + 'saveEvaluation', evaluation, {params});
  }

  saveUserGrades(userGrades: UserEvaluation[], evalClosed: boolean) {
    return this.http.put(this.baseUrl + evalClosed + '/saveUserGrades', userGrades);
  }

  getUserEvals(classId, courseId, periodId) {
    return this.http.get(this.baseUrl + 'class/' + classId + '/course/' + courseId + '/period/' + periodId);
  }

  getClassEval(id) {
    return this.http.get(this.baseUrl + 'ClassEval/' + id);
  }

  getUserCoursesWithEvals(classId, userId) {
    return this.http.get(this.baseUrl + 'class/' + classId + '/CoursesWithEvals/' + userId);
  }

  getUserGrades(userId) {
    return this.http.get(this.baseUrl + 'UserGrades/' + userId);
  }

  getTeacherEvalsToCome(teacherId) {
    return this.http.get(this.baseUrl + 'teacher/' + teacherId + '/evalsToCome');
  }

  getClassEvalsToCome(classId) {
    return this.http.get(this.baseUrl + 'class/' + classId + '/evalsToCome');
  }

  getClassEvals(classId) {
    return this.http.get(this.baseUrl + 'class/' + classId + '/AllEvaluations');
  }

  setCurrentCurrentEval(evaluation, userGrades) {
    this.currentEval = evaluation;
    this.userGrades = userGrades;
  }

  setEvalClosed(evalId: number) {
    return this.http.get(this.baseUrl + '' + evalId + '/closeEval');
  }

  setColIndex(index: number) {
    this.gradeIndex = index;
  }
}
