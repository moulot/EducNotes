import { Component, OnInit, ViewChild } from '@angular/core';
import { Period } from 'src/app/_models/period';
import { UserService } from 'src/app/_services/user.service';
import { EvaluationService } from 'src/app/_services/evaluation.service';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { AuthService } from 'src/app/_services/auth.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { SkillsModalComponent } from '../skills-modal/skills-modal.component';
import { EvalType } from 'src/app/_models/evalType';
import { Evaluation } from 'src/app/_models/evaluation';
import { User } from 'src/app/_models/user';
import { Router } from '@angular/router';
import { Utils } from 'src/app/shared/utils';
import { MDBModalService, MDBModalRef, MDBDatePickerComponent } from 'ng-uikit-pro-standard';
import { ClassService } from 'src/app/_services/class.service';

@Component({
  selector: 'app-eval-addForm',
  templateUrl: './eval-addForm.component.html',
  styleUrls: ['./eval-addForm.component.scss']
})
export class EvalAddFormComponent implements OnInit {
  @ViewChild('datePicker', {static: false}) datePicker: MDBDatePickerComponent;
  myDatePickerOptions = Utils.myDatePickerOptions;
  teacher: User;
  currentPeriod: Period;
  newEvalForm: FormGroup;
  teacherCourses: any;
  periods: Period[];
  evalTypes: EvalType[];
  coursesSkills: any = [];
  selCourseSkills: any = [];
  skillsSelected: any = [];
  teacherClasses: any;
  newEval = <Evaluation>{};
  progEltIds: string;
  isGradedOn = false;
  isGradeCounted = true;
  optionsClass: any[] = [];
  optionsCourse: any[] = [];
  optionsPeriod: any[] = [];
  optionsEvalType: any[] = [];
  modalRef: MDBModalRef;
  wait = false;

  constructor(private userService: UserService, private evalService: EvaluationService,
    private fb: FormBuilder, private authService: AuthService, private router: Router, private classService: ClassService,
    public alertify: AlertifyService, private modalService: MDBModalService) { }

  ngOnInit() {
    this.createNewEvalForm();
    this.teacher = this.authService.currentUser;
    this.getTeacherClasses(this.teacher.id);
    this.getTeacherCourses(this.teacher.id);
    this.getPeriods();
    this.getEvalTypes();
    this.loadCoursesSkills();
  }

  createNewEvalForm() {
    this.newEvalForm = this.fb.group({
      newaclass: [null, Validators.required],
      newcourse: [null, Validators.required],
      newperiod: [null, Validators.required],
      evalName: ['', Validators.required],
      evalType: [null, Validators.required],
      evalDate: [null, Validators.required],
      grades: this.fb.group({
        evalGraded: [false],
        // gradeInLetter: [false],
        evalMaxGrade: [''], // {validator: this.maxGradeValidator}],
        evalCoeff: ['1'], // {validator: this.coeffValidator}],
        // notCounted: [false],
        // isNegative: [false]
      })
    }, {validator: this.gradeDataValidator});
  }

  gradeDataValidator(g: FormGroup) {
    const evalGraded = g.controls['grades'].get('evalGraded').value;
    const evalMaxGrade = g.controls['grades'].get('evalMaxGrade').value;
    const evalCoeff = g.controls['grades'].get('evalCoeff').value;
    if (evalGraded) {
      if (!Utils.isNumber(evalMaxGrade)) {
        return {'maxGradeNOK': true};
      }
      if (!Utils.isNumber(evalCoeff)) {
        return {'coeffNOK': true};
      }
    }
    return null;
  }

  maxGradeValidator(g: FormGroup) {
    const isGraded = g.get('evalGraded').value;
    const maxGrade = g.get('evalMaxGrade').value;
    if (isGraded === true) {
      if (!Utils.isNumber(maxGrade)) {
        return {'maxGradeNOK': true};
      } else {
        return null;
      }
    } else {
      return null;
    }
  }

  coeffValidator(g: FormGroup) {
    const isGraded = g.get('evalGraded').value;
    const evalCoeff = g.get('evalCoeff').value;
    if (isGraded === true) {
      if (!Utils.isNumber(evalCoeff)) {
        return {'coeffNOK': true};
      } else {
        return null;
      }
    } else {
      return null;
    }
  }

  createEvaluation(more: boolean) {
    this.wait = true;
    this.newEval.userId = this.teacher.id;
    this.newEval.name = this.newEvalForm.value.evalName;
    this.newEval.courseId = this.newEvalForm.value.newcourse;
    this.newEval.evalTypeId = this.newEvalForm.value.evalType;
    const dateData = this.newEvalForm.value.evalDate.split('/');
    this.newEval.evalDate = new Date(dateData[2], dateData[1] - 1, dateData[0]);
    this.newEval.graded = this.newEvalForm.controls['grades'].value.evalGraded;
    this.newEval.periodId = this.newEvalForm.value.newperiod;
    this.newEval.canBeNagative = this.newEvalForm.controls['grades'].value.isNegative;
    this.newEval.classId = this.newEvalForm.value.newaclass;
    this.newEval.coeff = this.newEvalForm.controls['grades'].value.evalCoeff;
    this.newEval.gradeInLetter = this.newEvalForm.controls['grades'].value.gradeInLetter;
    this.newEval.maxGrade = this.newEvalForm.controls['grades'].value.evalMaxGrade;
    this.newEval.significant = this.newEvalForm.controls['grades'].value.notCounted;
    this.newEval.progElts = [];
    this.progEltIds = '';
    for (let i = 0; i < this.skillsSelected.length; i++) {
      const id = this.skillsSelected[i].progEltId;
      if (this.progEltIds === '') {
        this.progEltIds = id;
      } else {
        this.progEltIds = this.progEltIds + ',' + id;
      }
    }

    this.evalService.saveEvaluation(this.newEval, this.progEltIds).subscribe(() => {
      this.alertify.success('ajout de l\'évaluation validé.');
      this.wait = false;
    }, error => {
      this.alertify.error(error);
      this.wait = false;
    }, () => {
      this.newEvalForm.reset();
      this.datePicker.clearDate();
      this.skillsSelected = [];
      if (this.isGradedOn) {
        this.isGradedOn = false;
      }
      this.wait = false;
      if (more === false) {
        this.router.navigate(['/teacher']);
      }
    });
  }

  newCancel() {
    this.newEvalForm.reset();
    this.skillsSelected = [];
    if (this.isGradedOn) {
      this.isGradedOn = false;
    }
}

  toggleGrade() {
    this.isGradedOn = !this.isGradedOn;
  }

  toggleCounted() {
    this.isGradeCounted = !this.isGradeCounted;
  }

  toggleNegative() {

  }

  getTeacherClasses(teacherId) {
    this.userService.getTeacherClasses(teacherId).subscribe((classes: any) => {
      this.teacherClasses = classes;
      for (let i = 0; i < classes.length; i++) {
        const elt = classes[i];
        const element = {value: elt.classId, label: 'classe ' + elt.className};
        this.optionsClass = [...this.optionsClass, element];
      }

    }, error => {
      this.alertify.error(error);
    });
  }

  getTeacherCourses(teacherId) {
    this.userService.getTeacherCourses(teacherId).subscribe((courses: any) => {
      this.teacherCourses = courses;
      for (let i = 0; i < courses.length; i++) {
        const elt = courses[i];
        const element = {value: elt.id, label: elt.name};
        this.optionsCourse = [...this.optionsCourse, element];
      }
    }, error => {
      this.alertify.error(error);
    });
  }

  getPeriods() {
    this.classService.getPeriods().subscribe((periods: Period[]) => {
      this.periods = periods;
      for (let i = 0; i < periods.length; i++) {
        const elt = periods[i];
        const element = {value: elt.id, label: elt.name};
        this.optionsPeriod = [...this.optionsPeriod, element];
      }
    }, error => {
      this.alertify.error(error);
    });
  }

  getEvalTypes() {
    this.evalService.getEvalTypes().subscribe((types: EvalType[]) => {
      this.evalTypes = types;
      for (let i = 0; i < types.length; i++) {
        const elt = types[i];
        const element = {value: elt.id, label: elt.name};
        this.optionsEvalType = [...this.optionsEvalType, element];
      }
    }, error => {
      this.alertify.error(error);
    });
  }

  loadCoursesSkills() {
    this.evalService.getCoursesSkills().subscribe(data => {
      this.coursesSkills = data;
    }, error => {
      this.alertify.error(error);
    });
  }

  courseChanged() {
    const courseId = this.newEvalForm.value.newcourse;
    this.selCourseSkills = this.coursesSkills.filter(el => el.courseId === courseId);
  }

  addSkills() {
    const modalOptions = {
      backdrop: true,
      keyboard: true,
      focus: true,
      show: false,
      ignoreBackdropClick: false,
      class: 'modal-lg',
      containerClass: 'overflow-auto',
      animated: true,
      data: {
        courseSkills : this.selCourseSkills[0]
      }
    };

    this.modalRef = this.modalService.show(SkillsModalComponent, modalOptions);

    this.modalRef.content.updateProgElt.subscribe( (data) => {
      this.skillsSelected = [...this.skillsSelected, data];
      this.skillsSelected = this.skillsSelected.filter(el => el.checked === true);
    });
  }

}
