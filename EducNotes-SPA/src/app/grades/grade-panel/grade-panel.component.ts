import { Component, OnInit, ViewChild, ElementRef } from '@angular/core';
import { User } from 'src/app/_models/user';
import { UserService } from 'src/app/_services/user.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { CourseUser } from 'src/app/_models/courseUser';
import { FormBuilder, FormGroup, Validators, FormControl } from '@angular/forms';
import { AuthService } from 'src/app/_services/auth.service';
import { EvaluationService } from 'src/app/_services/evaluation.service';
import { Period } from 'src/app/_models/period';
import { EvalType } from 'src/app/_models/evalType';
import { debounceTime } from 'rxjs/operators';
import { SharedAnimations } from 'src/app/shared/animations/shared-animations';


@Component({
  selector: 'app-grade-panel',
  templateUrl: './grade-panel.component.html',
  styleUrls: ['./grade-panel.component.css'],
  animations: [SharedAnimations]
})
export class GradePanelComponent implements OnInit {
  @ViewChild('classSelect', {static: false}) classSelect: ElementRef;
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
  pageSize = 8;

  searchControl: FormControl = new FormControl();
  filteredUserGrades: any = [];

  constructor(private userService: UserService, private evalService: EvaluationService,
    private fb: FormBuilder, private authService: AuthService, public alertify: AlertifyService) { }

  ngOnInit() {
    this.currentPeriod = this.authService.currentPeriod;
    this.teacher = this.authService.currentUser;
    this.createNotesForm();
    this.getTeacherClasses(this.teacher.id);
    this.getTeacherCourses(this.teacher.id);
    this.getPeriods();
    this.getEvalTypes();
    this.getClassesWithEvalsCurrPeriod(this.teacher.id);

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

  enterGrades(evaluation, userGrades, index) {
    this.evalService.setCurrentCurrentEval(evaluation, userGrades);
    this.evalService.setColIndex(index);
    this.toggleView();
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
  }

  cancelNotesFrom() {
    this.notesForm.reset();
    this.showTable = false;
  }

  getTeacherClasses(teacherId) {
    this.userService.getTeacherClasses(teacherId).subscribe((courses: CourseUser[]) => {
      this.teacherClasses = courses;
    }, error => {
      this.alertify.error(error);
    });
  }

  getClassesWithEvalsCurrPeriod(teacherId) {
    this.userService.getTeacherClassesWithEvalsByPeriod(teacherId, this.currentPeriod.id).subscribe(data => {
      this.classesWithEvals = data;
    }, error => {
      this.alertify.error(error);
    });
  }

  getTeacherCourses(teacherId) {
    this.userService.getTeacherCourses(teacherId).subscribe(courses => {
      this.teacherCourses = courses;
    }, error => {
      this.alertify.error(error);
    });
  }

  getPeriods() {
    this.evalService.getPeriods().subscribe((periods: Period[]) => {
      this.periods = periods;
    }, error => {
      this.alertify.error(error);
    });
  }

  getEvalTypes() {
    this.evalService.getEvalTypes().subscribe((types: EvalType[]) => {
      this.evalTypes = types;
    }, error => {
      this.alertify.error(error);
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
    }, error => {
      this.alertify.error(error);
    }, () => {
      this.showTable = true;
    });
  }

}
