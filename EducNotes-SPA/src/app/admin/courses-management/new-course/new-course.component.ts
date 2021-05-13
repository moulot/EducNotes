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
  course: Course;
  courseForm: FormGroup;
  colorOptions = [];
  defaultColors: string[] = [
    '#ffffff',
    '#000105',
    '#3e6158',
    '#3f7a89',
    '#96c582',
    '#b7d5c4',
    '#bcd6e7',
    '#7c90c1',
    '#9d8594',
    '#dad0d8',
    '#4b4fce',
    '#4e0a77',
    '#a367b5',
    '#ee3e6d',
    '#d63d62',
    '#c6a670',
    '#f46600',
    '#cf0500',
    '#efabbd',
    '#8e0622',
    '#f0b89a',
    '#f0ca68',
    '#62382f',
    '#c97545',
    '#c1800b'
  ];

  constructor(private fb: FormBuilder, private classService: ClassService,
    private router: Router, private alertify: AlertifyService, private route: ActivatedRoute) { }

  ngOnInit() {
    this.createCourseForm();
    this.setColorSelect();
  }

  createCourseForm() {
    this.courseForm = this.fb.group({
      id: [0],
      courseTypeId: [1],
      name: ['', Validators.required],
      abbrev: [''],
      color: ['', Validators.required]
    });
  }

  setColorSelect() {
    for (let i = 0; i < this.defaultColors.length; i++) {
      const elt = this.defaultColors[i];
      this.colorOptions = [...this.colorOptions, {value: elt, label: elt}];
    }
  }

  addCourse() {
    this.classService.addCourse(this.courseForm.value).subscribe(() => {
      this.alertify.success('le cours est bien enregistré...');
      this.router.navigate(['/courses']);
    }, error => {
      this.alertify.error(error);
    });
  }

}
