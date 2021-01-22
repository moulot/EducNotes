import { Component, OnInit, EventEmitter, Output } from '@angular/core';
import { Utils } from 'src/app/shared/utils';
import { UserEvaluation } from 'src/app/_models/userEvaluation';
import { debounceTime } from 'rxjs/operators';
import { EvaluationService } from 'src/app/_services/evaluation.service';
import { FormBuilder, FormGroup, FormControl } from '@angular/forms';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { ActivatedRoute, Router } from '@angular/router';
import { SharedAnimations } from 'src/app/shared/animations/shared-animations';

@Component({
  selector: 'app-add-user-grades',
  templateUrl: './add-user-grades.component.html',
  styleUrls: ['./add-user-grades.component.scss'],
  animations: [SharedAnimations]
})
export class AddUserGradesComponent implements OnInit {
  eval: any;
  userGrades: any = [];
  newUserGrades: any = [];
  evalForm: FormGroup;
  userEvals: UserEvaluation[] = [];
  searchControl: FormControl = new FormControl();
  filteredUserGrades: any = [];
  viewMode: 'list' | 'grid' = 'list';
  page = 1;
  pageSize = 15;
  closed: boolean;
  gradeErrors = [];
  formNOK = false;
  wait = false;

  constructor(private evalService: EvaluationService, private fb: FormBuilder,
    private alertify: AlertifyService, private route: ActivatedRoute, private router: Router) { }

  ngOnInit() {
    this.route.data.subscribe((data: any) => {
      const classEval = data['data'];
      this.eval = classEval.eval;
      this.userGrades = classEval.usersEval;
      this.filteredUserGrades = this.userGrades;
      this.newUserGrades = this.userGrades;
      this.closed = this.eval.closed;
      for (let i = 0; i < this.userGrades.length; i++) {
        const elt = false;
        this.gradeErrors = [...this.gradeErrors, elt];
      }
    });

    this.searchControl.valueChanges.pipe(debounceTime(200)).subscribe(value => {
      this.filerData(value);
    });
  }

  createEvalForm() {

  }

  filerData(val) {
    if (val) {
      val = val.toLowerCase();
    } else {
      return this.filteredUserGrades = [...this.userGrades];
    }

    const columns = Object.keys(this.userGrades[0]);
    if (!columns.length) {
      return;
    }

    const rows = this.userGrades.filter(function(d) {
      for (let i = 0; i <= columns.length; i++) {
        const column = columns[i];
        if (d[column] && d[column].toString().toLowerCase().indexOf(val) > -1) {
          return true;
        }
      }
    });
    this.filteredUserGrades = rows;
  }

  saveNotes() {
    this.wait = true;
    let formOK = true;
    let evalClosed = false;
    if (this.closed === true) {
      evalClosed = true;
    }

    for (let i = 0; i < this.userGrades.length; i++) {
      const elt = this.userGrades[i];
      if (Number(elt.grade) > Number(this.eval.maxGrade)) {
        formOK = false;
      }
      const ue = <UserEvaluation>{};
      ue.id = elt.id;
      ue.evaluationId = this.eval.id;
      ue.userId = elt.userId;
      ue.grade = elt.grade;
      ue.comment = elt.comment;
      this.userEvals = [...this.userEvals, ue];
    }

    if (formOK) {
      this.evalService.saveUserGrades(this.userEvals, evalClosed).subscribe(() => {
        this.alertify.success('ajout des notes OK');
      }, error => {
        this.alertify.error(error);
        this.wait = false;
      }, () => {
        this.router.navigate(['/grades']);
        this.wait = false;
      });
    } else {
      this.alertify.error('des notes sont incorrectes. recommencez svp');
    }
  }

  checkGrade(index, maxGrade) {
    const grade = this.userGrades[index].grade;
    if (Number(grade) > Number(maxGrade)) {
      this.gradeErrors[index] = true;
      this.formNOK = true;
    } else {
      this.gradeErrors[index] = false;
      this.formNOK = false;
    }
  }

  cancelForm() {
    this.router.navigate(['/grades']);
  }

}
