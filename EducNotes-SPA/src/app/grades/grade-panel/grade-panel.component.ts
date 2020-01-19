import { Component, OnInit, ViewChild, ElementRef } from '@angular/core';
import { User } from 'src/app/_models/user';
import { UserService } from 'src/app/_services/user.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { CourseUser } from 'src/app/_models/courseUser';
import { FormBuilder, FormGroup, Validators, FormControl, NgForm } from '@angular/forms';
import { AuthService } from 'src/app/_services/auth.service';
import { EvaluationService } from 'src/app/_services/evaluation.service';
import { Period } from 'src/app/_models/period';
import { EvalType } from 'src/app/_models/evalType';
import { debounceTime } from 'rxjs/operators';
import { SharedAnimations } from 'src/app/shared/animations/shared-animations';
import { Router } from '@angular/router';


@Component({
  selector: 'app-grade-panel',
  templateUrl: './grade-panel.component.html',
  styleUrls: ['./grade-panel.component.css'],
  animations: [SharedAnimations]
})
export class GradePanelComponent implements OnInit {
  @ViewChild('classSelect', {static: false}) classSelect: ElementRef;
  // @ViewChild('notesForm', {static: false}) form: NgForm;
  teacherClasses: any;
  teacher: User;
  notesForm: FormGroup;
  teacherCourses: any;
  periods: Period[];
  evalTypes: EvalType[];
  toggleListAdd = true;
  userGrades: any = [];
  evals: any = [];
  showTable = false;
  classesWithEvals: any;
  currentPeriod: Period;
  showSearch = true;
  viewMode: 'list' | 'grid' = 'list';
  page = 1;
  pageSize = 20;

  searchControl: FormControl = new FormControl();
  filteredUserGrades: any = [];
  evalsToBeGraded: any;
  evalsToCome: any;
  optionsClass = [];
  optionsCourse = [];
  optionsPeriod = [];

  constructor(private userService: UserService, private evalService: EvaluationService, private fb: FormBuilder,
    private authService: AuthService, public alertify: AlertifyService, private route: Router) { }

  ngOnInit() {
    this.currentPeriod = this.authService.currentPeriod;
    this.teacher = this.authService.currentUser;
    this.createNotesForm();
    this.getGradesData(this.teacher.id);
    this.getEvals(this.teacher.id);
    this.getFormData();

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

  getEvals(teacherId) {
    this.evalService.getTeacherEvalsToCome(teacherId).subscribe((evals: any) => {
      this.evalsToCome = evals.evalsToCome;
      this.evalsToBeGraded = evals.evalsToBeGraded;
    }, error => {
      this.alertify.error(error);
    });
  }

  enterGrades(evaluation, userGrades, index) {
    this.evalService.setCurrentCurrentEval(evaluation, userGrades);
    this.evalService.setColIndex(index);
    this.toggleView();
  }

  addUsersGrades(evalId) {
    // this.evalService.setCurrentCurrentEval(evaluation, userGrades);
    this.route.navigate(['/AddUserGrades', evalId]);
  }

  createNotesForm() {
    this.notesForm = this.fb.group({
      aclass: [null, Validators.required],
      course: [null, Validators.required],
      period: [null, Validators.required]
    });
  }

  onClick() {
    this.showSearch = !this.showSearch;
    this.notesForm.reset();
    this.showTable = false;
  }

  toggleView() {
    this.toggleListAdd = !this.toggleListAdd;
    if (this.toggleListAdd === true) {
      this.showNotes();
    }
  }

  cancelNotesFrom() {
    this.notesForm.reset();
    this.showTable = false;
  }

  getGradesData(teacherId) {
    this.userService.getGradesData(teacherId, this.currentPeriod.id).subscribe((data: any) => {
      this.teacherClasses = data.teacherClasses;
      this.teacherCourses = data.teacherCourses;
      this.classesWithEvals = data.classesWithEvals;
      for (let i = 0; i < this.teacherCourses.length; i++) {
        const elt = this.teacherCourses[i];
        const element = {value: elt.id, label: elt.name};
        this.optionsCourse = [...this.optionsCourse, element];
      }
      for (let i = 0; i < this.teacherClasses.length; i++) {
        const elt = this.teacherClasses[i];
        const element = {value: elt.classId, label: 'classe ' + elt.className};
        this.optionsClass = [...this.optionsClass, element];
      }
    });
  }

  getFormData() {
    this.evalService.getFormData().subscribe((data: any) => {
      this.periods = data.periods;
      this.evalTypes = data.types;
      for (let i = 0; i < this.periods.length; i++) {
        const elt = this.periods[i];
        const element = {value: elt.id, label: elt.name};
        this.optionsPeriod = [...this.optionsPeriod, element];
      }
  });
  }

  showNotes() {
    const aclass = this.notesForm.value.aclass;
    const course = this.notesForm.value.course;
    const period = this.notesForm.value.period;

    this.evalService.getUserEvals(aclass, course, period).subscribe((data: any) => {
      this.evals = data.evals;
      this.userGrades = data.userGrades;
      this.filteredUserGrades = data.userGrades;
      // console.log(this.userGrades);
    }, error => {
      this.alertify.error(error);
    }, () => {
      this.showTable = true;
    });
  }

}
