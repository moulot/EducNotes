import { Component, OnInit, EventEmitter, Output, Input } from '@angular/core';
import { FormGroup, FormBuilder, Validators, FormControl } from '@angular/forms';
import { ClassService } from 'src/app/_services/class.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { SharedAnimations } from 'src/app/shared/animations/shared-animations';
import { Course } from 'src/app/_models/course';

@Component({
  selector: 'app-new-course',
  templateUrl: './new-course.component.html',
  styleUrls: ['./new-course.component.scss'],
  animations :  [SharedAnimations]
})


export class NewCourseComponent implements OnInit {
  @Input() courseModel: Course;
  @Output() addCourseResult = new EventEmitter();
  // levels;
  courseForm: FormGroup;
  submitText = 'enregistrer';
  color = '';
  courseId;
  formMode = 'add';
  model;


  constructor(private fb: FormBuilder, private classService: ClassService,
    private alertify: AlertifyService) { }

  ngOnInit() {
    if (this.courseModel) {
      if (!this.courseModel.color) {
        this.courseModel.color = '';
        }
      this.model = this.courseModel;
      this.courseId = this.courseModel.id;
      this.formMode = 'edit';
    } else {
      this.initParams();
    }
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
  //  alert(this.courseForm.value.color);
  }

  save() {

      if (this.formMode === 'add') {
        // new insert
        this.classService.addNewCourse(this.courseForm.value).subscribe(() => {
          this.alertify.success('cours enregistrée...');
          this.addCourseResult.emit(true);
        }, error => {
          console.log(error);
        });
      } else {
        // updating Course
        this.classService.updatCourse(this.courseId, this.courseForm.value).subscribe(() => {
          this.alertify.success('cours enregistrée...');
          this.addCourseResult.emit(true);
        }, error => {
          console.log(error);
        });
      }

    }


  cancel() {
    this.addCourseResult.emit(false);
  }

}
