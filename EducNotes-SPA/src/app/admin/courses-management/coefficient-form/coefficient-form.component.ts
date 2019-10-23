import { Component, OnInit } from '@angular/core';
import { ClassService } from 'src/app/_services/class.service';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { SharedAnimations } from 'src/app/shared/animations/shared-animations';
import { Router } from '@angular/router';

@Component({
  selector: 'app-coefficient-form',
  templateUrl: './coefficient-form.component.html',
  styleUrls: ['./coefficient-form.component.scss'],
  animations :  [SharedAnimations]
})
export class CoefficientFormComponent implements OnInit {
  courses: any[] = [];
  levels: any[] = [];
  classTypes: any[] = [];
  coefficientForm: FormGroup;


  constructor(private classService: ClassService, private alertify: AlertifyService,
    private fb: FormBuilder, private router: Router) { }

  ngOnInit() {
    this.getClassTypes();
    this.getCourses();
    this.getLevels();
    this.createCoefficientForm();
  }

  createCoefficientForm() {
    this.coefficientForm = this.fb.group({
      classLevelId: [null, Validators.required],
      courseId: [null, Validators.required],
      classTypeId: [null],
      coefficient: [null, Validators.required]
    });
  }


  getCourses() {
    this.classService.getAllCourses().subscribe((res: any[]) => {
      for (let i = 0; i < res.length; i++) {
        const element = {value: res[i].id , label: res[i].name};
         this.courses = [...this.courses, element];
      }
    });
  }

  getLevels() {
    this.classService.getLevels().subscribe((res: any[]) => {
      for (let i = 0; i < res.length; i++) {
        const element = {value: res[i].id , label: res[i].name};
        this.levels = [...this.levels, element];
      }
    });
  }

  getClassTypes() {
    this.classService.getClassTypes().subscribe((res: any[]) => {
      for (let i = 0; i < res.length; i++) {
        const element = {value: res[i].id , label: res[i].name};
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
