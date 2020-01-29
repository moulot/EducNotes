import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { ClassService } from 'src/app/_services/class.service';
import { SharedAnimations } from 'src/app/shared/animations/shared-animations';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { ActivatedRoute, Router } from '@angular/router';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

@Component({
  selector: 'app-teacher-assignment',
  templateUrl: './teacher-assignment.component.html',
  styleUrls: ['./teacher-assignment.component.scss'],
  animations :  [SharedAnimations]
})
export class TeacherAssignmentComponent implements OnInit {
teacher: any;

courses: any;
selectedClassIds: any;
levels: any;
classIds: any[];
courseId: number;
classes: any;
levelId: any;
submitText = 'enregistrer';
Form: FormGroup;
courseOptions: any = [];
levelOptions: any = [];
classOptions: any = [];

  constructor(private classService: ClassService, private alertify: AlertifyService,
    private route: ActivatedRoute, private fb: FormBuilder, private router: Router) { }

  ngOnInit() {
    this.route.data.subscribe(data => {
      this.teacher = data.teacher;
      this.courses = this.teacher.courses;
      this.selectedClassIds = this.teacher.classIds;

      for (let i = 0; i < this.courses.length; i++) {
        const elt = this.courses[i];
        this.courseOptions = [...this.courseOptions, {value: elt.id, label: elt.name}];
      }
    });
    this.getLevels();
  }

  createParentsForms() {
    this.Form = this.fb.group({
      course: ['', Validators.required],
      level: ['', Validators.required],
      classes: ['', Validators.required],
    });
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
    this.classIds = [];

    this.classService.getClassesByLevelId(this.levelId).subscribe((res: any[]) => {
      this.classes = res;
      // recupération  des classes en fonction du niveau, de cours et du prof
      this.classService.teacherClassCoursByLevel(this.teacher.id, this.levelId, this.courseId).subscribe((val: any[]) => {
        for (let i = 0; i < val.length; i++) {
          this.classIds = [...this.classIds, val[i].classId];
        }
      });

    });
  }

  changeCourseId(): void {
    this.levelId = null;
    this.classes = [];
  }

  assignment() {

    this.classService.saveTeacherAffectation(this.teacher.id, this.courseId, this.levelId, this.classIds).subscribe(response => {
      this.alertify.success('affectation terminée...');
      this.router.navigate(['/teachers']);
      this.submitText = 'enregistrer';
   }, error => {
      this.submitText = 'enregistrer';
      this.alertify.error(error);
    });

  }

}
