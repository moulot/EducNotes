import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { ClassService } from 'src/app/_services/class.service';
import { SharedAnimations } from 'src/app/shared/animations/shared-animations';
import { AlertifyService } from 'src/app/_services/alertify.service';

@Component({
  selector: 'app-teacher-assignment',
  templateUrl: './teacher-assignment.component.html',
  styleUrls: ['./teacher-assignment.component.scss'],
  animations :  [SharedAnimations]
})
export class TeacherAssignmentComponent implements OnInit {
@Input() teacher: any;
@Output() addUserResult = new EventEmitter();

courses: any;
selectedClass: any;
levels: any;
classIds: any[];
courseId: number;
classes: any;
levelId: any;
submitText = 'enregistrer';

  constructor(private classService: ClassService, private alertify: AlertifyService) { }

  ngOnInit() {
    if (this.teacher) {
      this.courses = this.teacher.courses;
      this.selectedClass = this.teacher.classes;
    }
    this.getLevels();
  }

  getLevels() {
    this.classService.getLevels().subscribe((res) => {
      this.levels = res;
    }, error => {
      console.log(error);
    });
  }

  getClasses(): void {
    this.classIds = [];

    this.classService.getClassesByLevelId(this.levelId).subscribe((res: any[]) => {
      this.classes = res;
      // for (let i = 0; i < this.selectedClass.length; i++) {
      //   const element = this.selectedClass[i];
      //   if (element.classLevelId === this.levelId) {
      //     if (!this.classIds.find( item => item === element.id)) {
      //       this.classIds = [...this.classIds, element.id];
      //     }
      //   }
      // }

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

   close() {
    this.addUserResult.emit(false);

   }

   affectation() {

    this.classService.saveTeacherAffectation(this.teacher.id, this.courseId, this.levelId, this.classIds).subscribe(response => {
      this.alertify.success('affectation terminée...');
      this.addUserResult.emit(true);
      this.submitText = 'enregistrer';
   }, error => {
        this.submitText = 'enregistrer';
        this.alertify.error(error);

    });

  }



}
