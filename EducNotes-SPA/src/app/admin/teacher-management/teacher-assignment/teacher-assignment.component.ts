import { Component, OnInit } from '@angular/core';
import { ClassService } from 'src/app/_services/class.service';
import { SharedAnimations } from 'src/app/shared/animations/shared-animations';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { ActivatedRoute, Router } from '@angular/router';
import { FormBuilder, FormGroup, Validators, FormArray } from '@angular/forms';
import { AuthService } from 'src/app/_services/auth.service';
import { UserService } from 'src/app/_services/user.service';

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
  teacherForm: FormGroup;
  items: FormArray;
  courseOptions: any = [];
  levelOptions: any = [];
  classOptions: any = [];

  constructor(private classService: ClassService, private alertify: AlertifyService, private authService: AuthService,
    private userService: UserService, private route: ActivatedRoute, private fb: FormBuilder, private router: Router) { }

  ngOnInit() {
    this.createForm();
    this.route.data.subscribe(data => {
      const teacherData = data['teacher'];
      this.courses = teacherData.classes;
      this.teacher = teacherData.teacher;
      this.addCourseItems();
    });

    this.getLevels();
  }

  createForm() {
    this.teacherForm = this.fb.group({
      courses: this.fb.array([])
    });
  }

  addCourseItems(): void {
    const courses = this.teacherForm.get('courses') as FormArray;
    this.courses.forEach(x => {
      courses.push(this.fb.group({
        courseId: x.courseId,
        courseName: x.courseName,
        levels: this.addLevelItems(x.levels)
      }));
    });
  }

  addLevelItems(x) {
    const arr = new FormArray([]);
    x.forEach(y => {
      arr.push(this.fb.group({
        levelId: y.levelId,
        levelName: y.levelName,
        classes: this.addClassItems(y.classes)
      }));
    });
    return arr;
  }

  addClassItems(x) {
    const arr = new FormArray([]);
    x.forEach(y => {
      arr.push(this.fb.group({
        classId: y.classId,
        className: y.className,
        active: y.active,
        assigned: y.assigned
      }));
    });
    return arr;
  }

  minSelectedCheckboxes() {
    return this.classIds.length > 0;
  }

  getLevels() {
    this.classService.getLevels().subscribe(data => {
      this.levels = data;
      for (let i = 0; i < this.levels.length; i++) {
        const elt = this.levels[i];
        this.levelOptions = [...this.levelOptions, {value: elt.id, label: elt.name}];
      }
    });
  }

  assignment() {
    const courses = this.teacherForm.value.courses;
    this.userService.assignClasses(this.teacher.id, courses).subscribe(() => {
      this.alertify.success('affectation des classes enregistrée');
      this.router.navigate(['/teachers']);
    }, error => {
      this.alertify.error(error);
    });
  }

}
