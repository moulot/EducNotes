import { Component, OnInit, EventEmitter, Output } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { ClassService } from 'src/app/_services/class.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { SharedAnimations } from 'src/app/shared/animations/shared-animations';

@Component({
  selector: 'app-new-course',
  templateUrl: './new-course.component.html',
  styleUrls: ['./new-course.component.scss'],
  animations :  [SharedAnimations]
})
export class NewCourseComponent implements OnInit {
  @Output() addCourseResult = new EventEmitter();
  // levels;
  courseForm: FormGroup;
  submitText = 'enregistrer';

  constructor(private fb: FormBuilder, private classService: ClassService,
    private alertify: AlertifyService) { }

  ngOnInit() {
    this.createCourseForm();
    // this.getLevels();
  }
  createCourseForm() {
    this.courseForm = this.fb.group({
      name: ['', Validators.required],
      // classLevelIds: [null, Validators.required],
      abbreviation: ['', Validators.nullValidator]
    });
  }


  // getLevels() {
  //   this.classService.getLevels().subscribe((res) => {
  //     this.levels = res;
  //   }, error => {
  //     console.log(error);
  //   });
  // }


  // isNotSelected(value: any): boolean {
  //   return this.levels.indexOf(value) === -1;
  // }

  save() {
    const course =  Object.assign({}, this.courseForm.value);
    this.classService.addNewCourse(course).subscribe(() => {
      this.alertify.success('cours enregistrée...');
      this.addCourseResult.emit(true);
     }, error => {
       console.log(error);
     });
    }


  cancel() {
    this.addCourseResult.emit(false);
  }

}
