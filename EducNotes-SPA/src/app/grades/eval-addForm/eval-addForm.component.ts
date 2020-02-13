import { Component, OnInit } from '@angular/core';
import { Period } from 'src/app/_models/period';
import { UserService } from 'src/app/_services/user.service';
import { EvaluationService } from 'src/app/_services/evaluation.service';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { AuthService } from 'src/app/_services/auth.service';
import { NzModalService, NzModalRef } from 'ng-zorro-antd';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { SkillsModalComponent } from '../skills-modal/skills-modal.component';
import { EvalType } from 'src/app/_models/evalType';
import { Evaluation } from 'src/app/_models/evaluation';
import { User } from 'src/app/_models/user';
import { Router } from '@angular/router';
import { Utils } from 'src/app/shared/utils';
import { MDBModalService, MDBModalRef } from 'ng-uikit-pro-standard';

@Component({
  selector: 'app-eval-addForm',
  templateUrl: './eval-addForm.component.html',
  styleUrls: ['./eval-addForm.component.scss']
})
export class EvalAddFormComponent implements OnInit {
  myDatePickerOptions = Utils.myDatePickerOptions;
  teacher: User;
  currentPeriod: Period;
  newEvalForm: FormGroup;
  teacherCourses: any;
  periods: Period[];
  evalTypes: EvalType[];
  nzModalRef: NzModalRef;
  coursesSkills: any = [];
  selCourseSkills: any = [];
  skillsSelected: any = [];
  teacherClasses: any;
  newEvaluation = <Evaluation>{};
  progEltIds: string;
  isGradedOn = false;
  isGradeCounted = true;
  optionsClass: any[] = [];
  optionsCourse: any[] = [];
  optionsPeriod: any[] = [];
  optionsEvalType: any[] = [];
  modalRef: MDBModalRef;

  constructor(private userService: UserService, private evalService: EvaluationService,
    private fb: FormBuilder, private authService: AuthService, private router: Router,
    public alertify: AlertifyService, private nzModalService: NzModalService, private modalService: MDBModalService) { }

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
        evalMaxGrade: [''],
        evalCoeff: ['0'],
        // notCounted: [false],
        // isNegative: [false]
      })
    }); // , { validator: Validators.compose([CustomValidators.maxGradeValidator])}
  }

  createEvaluation(more: boolean) {
    this.newEvaluation.userId = this.teacher.id;
    this.newEvaluation.name = this.newEvalForm.value.evalName;
    this.newEvaluation.courseId = this.newEvalForm.value.newcourse;
    this.newEvaluation.evalTypeId = this.newEvalForm.value.evalType;
    const dateData = this.newEvalForm.value.evalDate.split('/');
    this.newEvaluation.evalDate = new Date(dateData[2], dateData[1] - 1, dateData[0]);
    this.newEvaluation.graded = this.newEvalForm.controls['grades'].value.evalGraded;
    this.newEvaluation.periodId = this.newEvalForm.value.newperiod;
    this.newEvaluation.canBeNagative = this.newEvalForm.controls['grades'].value.isNegative;
    this.newEvaluation.classId = this.newEvalForm.value.newaclass;
    this.newEvaluation.coeff = this.newEvalForm.controls['grades'].value.evalCoeff;
    this.newEvaluation.gradeInLetter = this.newEvalForm.controls['grades'].value.gradeInLetter;
    this.newEvaluation.maxGrade = this.newEvalForm.controls['grades'].value.evalMaxGrade;
    this.newEvaluation.significant = this.newEvalForm.controls['grades'].value.notCounted;
    this.newEvaluation.progElts = [];
    this.progEltIds = '';
    for (let i = 0; i < this.skillsSelected.length; i++) {
      const id = this.skillsSelected[i].progEltId;
      if (this.progEltIds === '') {
        this.progEltIds = id;
      } else {
        this.progEltIds = this.progEltIds + ',' + id;
      }
    }

    this.evalService.saveEvaluation(this.newEvaluation, this.progEltIds).subscribe(() => {
      this.alertify.success('ajout de l\'évaluation validé.');
    }, error => {
      this.alertify.error(error);
    }, () => {
      this.newEvalForm.reset();
      this.skillsSelected = [];
      if (more === false) {
        this.router.navigate(['/teacher']);
      }
    });
  }

  newCancel() {
    this.newEvalForm.reset();
    this.skillsSelected = [];
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
    this.evalService.getPeriods().subscribe((periods: Period[]) => {
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
