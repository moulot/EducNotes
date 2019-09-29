import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { EvaluationService } from 'src/app/_services/evaluation.service';
import { FormGroup, FormBuilder, Validators, FormControl } from '@angular/forms';
import { UserEvaluation } from 'src/app/_models/userEvaluation';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { debounceTime } from 'rxjs/operators';
import { SharedAnimations } from 'src/app/shared/animations/shared-animations';

@Component({
  selector: 'app-grade-addForm',
  templateUrl: './grade-addForm.component.html',
  styleUrls: ['./grade-addForm.component.css'],
  animations: [SharedAnimations]
})
export class GradeAddFormComponent implements OnInit {
  @Output() toggleView = new EventEmitter<boolean>();
  @Output() cancelNotesFrom = new EventEmitter<any>();
  selectedEval: any;
  userGrades: any = [];
  newUserGrades: any = [];
  notesForm: FormGroup;
  model: any = {};
  gradeIndex: number;
  userEvals: UserEvaluation[] = [];
  searchControl: FormControl = new FormControl();
  filteredUserGrades: any = [];
  viewMode: 'list' | 'grid' = 'list';
  page = 1;
  pageSize = 15;

  constructor(private evalService: EvaluationService, private fb: FormBuilder, private alertify: AlertifyService) { }

  ngOnInit() {
    this.selectedEval = this.evalService.currentEval;
    this.userGrades = this.evalService.userGrades;
    this.filteredUserGrades = this.userGrades;
    this.newUserGrades = this.userGrades;
    this.gradeIndex = this.evalService.gradeIndex;

    this.searchControl.valueChanges.pipe(debounceTime(200)).subscribe(value => {
      this.filerData(value);
    });
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
    for (let i = 0; i < this.userGrades.length; i++) {
      const elt = this.userGrades[i];
      const grade = elt.grades[this.gradeIndex];
      const comment = elt.comments[this.gradeIndex];
      const ue = <UserEvaluation>{};
      ue.evaluationId = this.selectedEval.id;
      ue.userId = elt.userId;
      ue.grade = grade;
      ue.comment = comment;
      this.userEvals = [...this.userEvals, ue];
    }

    this.evalService.saveUserGrades(this.userEvals).subscribe(() => {
      this.alertify.success('ajout des notes OK');
    }, error => {
      this.alertify.error(error);
    }, () => {
      this.evalService.setCurrentCurrentEval(this.selectedEval, this.newUserGrades);
      this.toggleView.emit();
      this.scrollToTop();
    });
  }

  cancelForm() {
    this.toggleView.emit();
    this.scrollToTop();
  }

  scrollToTop() {
    const scrollToTop = window.setInterval(() => {
      const pos = window.pageYOffset;
      if (pos > 0) {
          window.scrollTo(0, pos - 10); // how far to scroll on each step
      } else {
          window.clearInterval(scrollToTop);
      }
    }, 10);
  }

}
