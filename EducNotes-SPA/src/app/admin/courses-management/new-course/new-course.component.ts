import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators, FormControl } from '@angular/forms';
import { ClassService } from 'src/app/_services/class.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { SharedAnimations } from 'src/app/shared/animations/shared-animations';
import { Course } from 'src/app/_models/course';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-new-course',
  templateUrl: './new-course.component.html',
  styleUrls: ['./new-course.component.scss'],
  animations :  [SharedAnimations]
})


export class NewCourseComponent implements OnInit {
  // levels;
  courseModel: Course;
  courseForm: FormGroup;
  submitText = 'enregistrer';
  color = '';
  courseId;
  formMode = 'add';
  model;


  constructor(private fb: FormBuilder, private classService: ClassService,
     private router: Router, private alertify: AlertifyService, private route: ActivatedRoute) { }

  ngOnInit() {
    this.route.data.subscribe(data => {
      if (data.course) {
         this.model = data.course;
             this.courseId = data.course.id;
          this.formMode = 'edit';
     } else {
       this.initParams();
     }
   });

    this.createCourseForm();
    this.color = this.model.color;
   // this.createCourseForm();
  }

  initParams() {
    this.model = {
      name : '',
      abbreviation : '',
      color : ''
    };
  }

  createCourseForm() {
    this.courseForm = this.fb.group({
      name: [this.model.name, Validators.required],
      abbreviation: [this.model.abbreviation],
      color: [this.model.color]
    });
  }

  setColor() {
    this.courseForm.patchValue({color: this.color});
  }

  save() {

      if (this.formMode === 'add') {
        this.classService.addNewCourse(this.courseForm.value).subscribe(() => {
          this.alertify.success('cours enregistrée...');
          this.router.navigate(['/courses']);
        }, error => {
          console.log(error);
        });
      } else {
        this.classService.updatCourse(this.courseId, this.courseForm.value).subscribe(() => {
          this.alertify.success('cours enregistrée...');
          this.router.navigate(['/courses']);

        }, error => {
          console.log(error);
        });
      }

    }


}
