import { Component, OnInit } from '@angular/core';
import { ClassService } from 'src/app/_services/class.service';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { SharedAnimations } from 'src/app/shared/animations/shared-animations';
import { Router, ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-coefficient-form',
  templateUrl: './coefficient-form.component.html',
  styleUrls: ['./coefficient-form.component.scss'],
  animations: [SharedAnimations]
})
export class CoefficientFormComponent implements OnInit {
  courses: any[] = [];
  levels: any[] = [];
  classTypes: any[] = [];
  coefficientForm: FormGroup;
  formModel: any;
  editMode = 'add';


  constructor(private classService: ClassService, private alertify: AlertifyService,
    private fb: FormBuilder, private router: Router, private route: ActivatedRoute) { }

  ngOnInit() {

    this.route.data.subscribe(data => {
      if (data.coef) {
        this.formModel = data.coef;
        this.editMode = 'edit';
      } else {
        this.initParams();
      }
    });
    this.getClassTypes();
    this.getCourses();
    this.getLevels();
    this.createCoefficientForm();
  }

  initParams() {
    this.formModel = {
      classLevelId: null,
      courseId: null,
      classTypeId: null,
      coefficient: null,
    };
  }

  createCoefficientForm() {
    this.coefficientForm = this.fb.group({
      classLevelId: [this.formModel.classLevelId, Validators.required],
      courseId: [this.formModel.courseId, Validators.required],
      classTypeId: [this.formModel.classTypeId],
      coefficient: [this.formModel.coefficient, Validators.required]
    });
  }

  getCourses() {
    this.classService.getCourses().subscribe((res: any[]) => {
      for (let i = 0; i < res.length; i++) {
        const element = { value: res[i].id, label: res[i].name };
        this.courses = [...this.courses, element];
      }
    });
  }

  getLevels() {
    this.classService.getLevelsWithClasses().subscribe((res: any[]) => {
      for (let i = 0; i < res.length; i++) {
        const element = { value: res[i].id, label: res[i].name };
        this.levels = [...this.levels, element];
      }
    });
  }

  getClassTypes() {
    this.classService.getClassTypes().subscribe((res: any[]) => {
      for (let i = 0; i < res.length; i++) {
        const element = { value: res[i].id, label: res[i].name };
        this.classTypes = [...this.classTypes, element];
      }
    });
  }
  save() {
    this.classService.createCourseCoefficient(this.coefficientForm.value).subscribe(() => {
      this.alertify.success('enregistrement terminé...');
      this.router.navigate(['coefficients']);

    }, error => {
      this.alertify.error(error);
    });
  }

}
