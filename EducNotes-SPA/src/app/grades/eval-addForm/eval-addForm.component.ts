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
import { CourseUser } from 'src/app/_models/courseUser';
import { Router } from '@angular/router';

@Component({
  selector: 'app-eval-addForm',
  templateUrl: './eval-addForm.component.html',
  styleUrls: ['./eval-addForm.component.scss']
})
export class EvalAddFormComponent implements OnInit {
  teacher: User;
  currentPeriod: Period;
  newEvalForm: FormGroup;
  teacherCourses: any;
  periods: Period[];
  evalTypes: EvalType[];
  nzModalRef: NzModalRef;
  courseSkills: any = [];
  skillsSelected: any = [];
  teacherClasses: any;
  newEvaluation = <Evaluation>{};
  progEltIds: string;
  isGradedOn = false;
  isGradeCounted = true;

  constructor(private userService: UserService, private evalService: EvaluationService,
    private fb: FormBuilder, private authService: AuthService, private router: Router,
    public alertify: AlertifyService, private nzModalService: NzModalService) { }

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
        isGraded: [false],
        gradeInLetter: [false],
        evalMaxGrade: [''],
        evalCoeff: ['0'],
        notCounted: [false],
        isNegative: [false]
      })
    } // , { validator: Validators.compose([CustomValidators.maxGradeValidator])}
    );
  }

  createEvaluation(more: boolean) {
    this.newEvaluation.userId = this.teacher.id;
    this.newEvaluation.name = this.newEvalForm.value.evalName;
    this.newEvaluation.courseId = this.newEvalForm.value.newcourse;
    this.newEvaluation.evalTypeId = this.newEvalForm.value.evalType;
    this.newEvaluation.evalDate = this.newEvalForm.value.evalDate;
    this.newEvaluation.graded = this.newEvalForm.controls['grades'].value.isGraded;
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
  }

  toggleGrade() {
    this.isGradedOn = !this.isGradedOn;
  }

  toggleCounted() {
    this.isGradeCounted = !this.isGradeCounted;
  }

  getTeacherClasses(teacherId) {
    this.userService.getTeacherClasses(teacherId).subscribe((courses: CourseUser[]) => {
      this.teacherClasses = courses;
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

  loadCoursesSkills() {
    this.evalService.getCoursesSkills().subscribe(data => {
      this.courseSkills = data;
    }, error => {
      this.alertify.error(error);
    });
  }

  addSkills() {

    this.nzModalRef = this.nzModalService.create<SkillsModalComponent>({
      nzTitle: 'compétences',
      nzWrapClassName: 'vertical-center-modal',
      nzWidth: '950px',
      nzContent: SkillsModalComponent,
      nzFooter: [
        {
          label: 'fermer',
          shape: 'primary',
          onClick: () => this.nzModalRef.destroy()
        }
      ],
      nzComponentParams: {
        title: 'title in component',
        subtitle: 'component sub title.',
        coursesSkills : this.courseSkills
      }
    });

    setTimeout(() => {
      const instance = this.nzModalRef.getContentComponent();
      instance.updateProgElt.subscribe((data) => {
        this.skillsSelected = [...this.skillsSelected, data];
        this.skillsSelected = this.skillsSelected.filter(el => el.checked === true);
      });
    }, 300);

  }

}
