import { Component, OnInit } from '@angular/core';
import { ClassService } from 'src/app/_services/class.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { User } from 'src/app/_models/user';
import { Class } from 'src/app/_models/class';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-class-life',
  templateUrl: './class-life.component.html',
  styleUrls: ['./class-life.component.scss']
})
export class ClassLifeComponent implements OnInit {
  teacher: User;
  teacherClasses: any;
  selectedClass: Class;
  students: User[];
  nbAbsences = 0;
  nbSanctions = 0;
  nbRewards = 0;
  toggleFormAdd = false;
  absencesList: any = [];
  sanctionsList: any = [];
  title = true;

  constructor(private classService: ClassService, private alertify: AlertifyService,
    private route: ActivatedRoute) { }

  ngOnInit() {
    this.route.params.subscribe(params => {
      const classId = params['classId'];
      this.loadClass(classId);
    });
  }

  toggleForm() {
    this.toggleFormAdd = !this.toggleFormAdd;
  }

  loadClass(classId) {
    this.getClass(classId);
    this.classService.getClassStudents(classId).subscribe(data => {
    this.students = data;
    this.loadAbsences(classId);
    this.loadSanctions(classId);
   }, error => {
     this.alertify.error(error);
   });

 }

 getClass(classId) {
  this.classService.getClass(classId).subscribe((aclass: Class) => {
    this.selectedClass = aclass;
  });
}


 loadAbsences(classId) {
    this.classService.getClassAbsences(classId).subscribe((data: any) => {
      this.absencesList = data.absences;
      this.nbAbsences = data.nbAbsences;
    }, error => {
      this.alertify.error(error);
    });
  }

  loadSanctions(classId) {
    this.classService.getClassSanctions(classId).subscribe((data: any) => {
      this.sanctionsList = data.sanctions;
      this.nbSanctions = data.nbSanctions;
    }, error => {
      this.alertify.error(error);
    });
  }

}
