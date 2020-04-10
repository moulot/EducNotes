import { Component, OnInit, Input, ElementRef, Renderer2, ViewChild, EventEmitter, Output } from '@angular/core';
import { CardRotatingComponent } from 'ng-uikit-pro-standard';
import { FormGroup, FormBuilder, Validators, FormArray } from '@angular/forms';
import { ClassService } from 'src/app/_services/class.service';
import { Router } from '@angular/router';
import { AlertifyService } from 'src/app/_services/alertify.service';

@Component({
  selector: 'app-course-card',
  templateUrl: './course-card.component.html',
  styleUrls: ['./course-card.component.scss']
})
export class CourseCardComponent implements OnInit {
  @Input() course: any;
  @Output() saveCourse = new EventEmitter<any>();
  courseForm: FormGroup;
  coursePrevValues: any;
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

  constructor(private fb: FormBuilder, private el: ElementRef, private renderer: Renderer2,
    private classService: ClassService, private router: Router, private alertify: AlertifyService) { }
  @ViewChild('card', { static: true }) flippingCard: CardRotatingComponent;

  ngOnInit() {
    const cardWrapper = this.el.nativeElement.querySelectorAll('.card-wrapper');
    this.renderer.setStyle(cardWrapper[0], 'height', '270px');
    this.createCourseForm();
    this.setColorSelect();
  }

  createCourseForm() {
    this.courseForm = this.fb.group({
      course: this.fb.array([])
    });
  }

  setColorSelect() {
    for (let i = 0; i < this.defaultColors.length; i++) {
      const elt = this.defaultColors[i];
      this.colorOptions = [...this.colorOptions, {value: elt, label: elt}];
    }
  }

  addCourseItem(course): void {
    const courseCtrl = this.courseForm.get('course') as FormArray;
    courseCtrl.removeAt(0);
    courseCtrl.push(this.createCourseItem(course));
  }

  createCourseItem(course): FormGroup {
    return this.fb.group({
      id: course.id,
      name: course.name,
      abbrev: course.abbreviation,
      color: course.color
    });
  }

  resetValues() {
    this.addCourseItem(this.coursePrevValues);
  }

  showEditForm(course) {
    this.coursePrevValues = course;
    this.addCourseItem(course);
  }

  setColor(color) {
    this.courseForm.patchValue({color: color});
  }

  addCourse() {
    this.saveCourse.emit(this.courseForm.value.course[0]);
  }
}
