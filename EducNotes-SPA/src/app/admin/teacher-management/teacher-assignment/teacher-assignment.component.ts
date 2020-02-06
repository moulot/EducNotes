import { Component, OnInit } from '@angular/core';
import { ClassService } from 'src/app/_services/class.service';
import { SharedAnimations } from 'src/app/shared/animations/shared-animations';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { ActivatedRoute, Router } from '@angular/router';
import { FormBuilder, FormGroup, Validators, FormArray } from '@angular/forms';

@Component({
  selector: 'app-teacher-assignment',
  templateUrl: './teacher-assignment.component.html',
  styleUrls: ['./teacher-assignment.component.scss'],
  animations: [SharedAnimations]
})
export class TeacherAssignmentComponent implements OnInit {
  teacher: any;
  courses: any;
  selectedClassIds: any;
  levels: any;
  classIds: any[] = [];
  courseId = 0;
  levelId = 0;
  classes: any;
  Form: FormGroup;
  items: FormArray;
  courseOptions: any = [];
  levelOptions: any = [];
  classOptions: any = [];
  arrayClasses: {
    id: number;
    name: string;
  }[] = [];

  constructor(private classService: ClassService, private alertify: AlertifyService,
    private route: ActivatedRoute, private fb: FormBuilder, private router: Router) { }

  ngOnInit() {
    this.route.data.subscribe(data => {
      this.teacher = data.teacher;
      this.courses = this.teacher.courses;
      this.selectedClassIds = this.teacher.classIds;
      this.createForm(this.selectedClassIds);

      for (let i = 0; i < this.courses.length; i++) {
        const elt = this.courses[i];
        this.courseOptions = [...this.courseOptions, {value: elt.id, label: elt.name}];
      }
    });
    this.getLevels();

    this.Form.get('classes').valueChanges.subscribe( (value) => {
      this.classIds = value;
    });
  }

  createForm(classSelected) {
    this.Form = this.fb.group({
      course: ['', Validators.required],
      level: ['', Validators.required],
      classes: [classSelected, this.minSelectedCheckboxes]
    });
  }

  minSelectedCheckboxes() {
    return this.classIds.length > 0;
  }

  get classArray() {
    return this.Form.get('classArray') as FormArray;
  }

  getLevels() {
    this.classService.getLevels().subscribe((res) => {
      this.levels = res;
      for (let i = 0; i < this.levels.length; i++) {
        const elt = this.levels[i];
        this.levelOptions = [...this.levelOptions, {value: elt.id, label: elt.name}];
      }
    }, error => {
      console.log(error);
    });
  }

  getClasses(): void {
    this.levelId = this.Form.value.level;

    this.classService.getClassesByLevelId(this.levelId).subscribe((classes: any[]) => {
      this.classes = classes;
      this.classOptions = [];
      for (let i = 0; i < this.classes.length; i++) {
        const elt = this.classes[i];
        this.classOptions = [...this.classOptions, {label: 'classe ' + elt.name, value: elt.id}];
      }

      // recupération  des classes en fonction du niveau, de cours et du prof
      this.classIds = [];
      this.classService.teacherClassCoursByLevel(this.teacher.id, this.levelId, this.courseId).subscribe((data: any[]) => {
        for (let i = 0; i < data.length; i++) {
          this.classIds = [...this.classIds, data[i].classId];
        }
        this.Form.controls['classes'].setValue(this.classIds);
      });
    });
  }

  changeCourse(): void {
    this.levelId = 0;
    this.courseId = this.Form.value.course;
    this.classes = [];
  }

  assignment() {
    this.classIds = this.Form.value.classes;
    // console.log(this.teacher.id + ' ' + this.courseId + ' ' + this.levelId + ' ' + this.classIds);
    this.classService.saveTeacherAffectation(this.teacher.id, this.courseId, this.levelId, this.classIds).subscribe(response => {
      this.alertify.success('affectation terminée...');
      this.router.navigate(['/teachers']);
    }, error => {
      this.alertify.error(error);
    });
  }

}
