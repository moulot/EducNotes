import { Component, OnInit } from '@angular/core';
import { User } from 'src/app/_models/user';
import { ClassService } from 'src/app/_services/class.service';
import { AuthService } from 'src/app/_services/auth.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { UserService } from 'src/app/_services/user.service';
import { CourseUser } from 'src/app/_models/courseUser';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';


@Component({
  selector: 'app-class-panel',
  templateUrl: './class-panel.component.html',
  styleUrls: ['./class-panel.component.css']
})
export class ClassPanelComponent implements OnInit {
  // @ViewChild(SchedulePanelComponent, {static: false}) scheduleCmp: SchedulePanelComponent;
  teacher: User;
  teacherClasses: any;
  selectedClass: any;
  students: User[];
  firstClick = false;
  classAgendaDays: any;
  nbDayTasks = [0, 0, 0, 0, 0, 0];
  weekDays = [0, 0, 0, 0, 0, 0];
  agendaParams: any = {};
  monday: Date;
  strMonday: string;
  strSaturday: string;
  strSunday: string;
  scheduleItems: any;
  toggleFormAdd = false;
  absencesList: any = [];
  sanctionsList: any = [];
  nbAbsences = 0;
  nbSanctions = 0;
  nbRewards = 0;
  title = true;
  monCourses = [];
  tueCourses = [];
  wedCourses = [];
  thuCourses = [];
  friCourses = [];
  satCourses = [];
  sunCourses = [];
  courseOptions = [];
  nextCourses: any;
  sessionForm: FormGroup;

  constructor(private userService: UserService, private classService: ClassService,
    private authService: AuthService, public alertify: AlertifyService,
    private router: Router, private fb: FormBuilder) { }

  ngOnInit() {
    this.createSessionForm();
    this.teacher = this.authService.currentUser;
    this.getTeacherClasses(this.teacher.id);
    // this.getTeacherNextCourses(this.teacher.id);
    // this.getNextCoursesByClass(this.teacher.id);
  }

  createSessionForm() {
    this.sessionForm = this.fb.group({
      course: [null, Validators.required]
    });
  }

  getTeacherClasses(teacherId) {
    this.userService.getTeacherClasses(teacherId).subscribe((classes: CourseUser[]) => {
      this.teacherClasses = classes;
      this.getNextCoursesByClass(this.teacher.id);
    }, error => {
      this.alertify.error(error);
    });
  }

  getNextCoursesByClass(teacherId) {
    this.userService.getNextCoursesByClass(teacherId).subscribe((data: any) => {
      this.nextCourses = data;
      // console.log(this.nextCourses);
      for (let i = 0; i < this.teacherClasses.length; i++) {
        const aclass = this.teacherClasses[i];
        const index = this.nextCourses.findIndex(item => item.classId === aclass.classId);
        // console.log(index);
        const coursesByClass = this.nextCourses[index];
        let classOptions = [];
        for (let j = 0; j < coursesByClass.courses.length; j++) {
          const course = coursesByClass.courses[j];
          // console.log(course);
          const option = {value: course.scheduleId, label: aclass.className + '. ' + course.courseName + '. '
            + course.startHourMin + ' à ' + course.endHourMin};
          classOptions = [...classOptions, option];
        }
        this.courseOptions = [...this.courseOptions, classOptions];
      }
      // console.log(this.courseOptions);
    });
  }

  goToClass() {
    const scheduleId = this.sessionForm.value.course;
    // console.log(scheduleId);
    this.router.navigate(['/classSession', scheduleId]);
  }

  loadClass(aclass) {
     this.firstClick = true;
    this.selectedClass = aclass;
    this.classService.getClassStudents(aclass.classId).subscribe(data => {
      this.students = data;
      this.loadWeekAgenda();
      this.loadAbsences(aclass.classId);
      this.loadSanctions(aclass.classId);
      // this.scheduleCmp.loadWeekSchedule(aclass.classId);
      this.loadWeekSchedule(aclass.classId);
      }, error => {
      this.alertify.error(error);
    });

  }

  loadWeekSchedule(classId) {

    this.resetSchedule();
    this.classService.getClassSchedule(classId).subscribe((response: any) => {
      this.scheduleItems = response.scheduleItems;
      this.strMonday = response.strMonday;
      this.strSunday = response.strSunday;
      this.weekDays = response.weekDays;

      // add courses on the schedule
      for (let i = 1; i <= 7; i++) {
        const filtered = this.scheduleItems.filter(items => items.day === i);
        for (let j = 0; j < filtered.length; j++) {
          switch (i) {
            case 1:
            this.monCourses.push(filtered[j]);
            break;
            case 2:
            this.tueCourses.push(filtered[j]);
            break;
            case 3:
            this.wedCourses.push(filtered[j]);
            break;
            case 4:
            this.thuCourses.push(filtered[j]);
            break;
            case 5:
            this.friCourses.push(filtered[j]);
            break;
            case 6:
            this.satCourses.push(filtered[j]);
            break;
            case 7:
            this.sunCourses.push(filtered[j]);
            break;
            default:
              break;
          }
        }
      }

    }, error => {
      this.alertify.error(error);
    });
  }

  loadWeekAgenda() {

    this.classService.getClassCurrWeekAgenda(this.selectedClass.classId).subscribe((response: any) => {

      this.classAgendaDays = response.agendaItems;
      this.monday = response.firstDayWeek;
      this.strMonday = response.strMonday;
      this.strSaturday = response.strSaturday;
      this.weekDays = response.weekDays;
      this.nbDayTasks = response.nbDayTasks;

    }, error => {
      this.alertify.error(error);
    });
  }

  loadMovedWeek(move: number) {

    this.agendaParams.dueDate = this.monday;
    this.agendaParams.moveWeek = move;

    this.classService.getClassMovedWeekAgenda(this.selectedClass.classId, this.agendaParams).subscribe((response: any) => {

      this.classAgendaDays = response.agendaItems;
      this.monday = response.firstDayWeek;
      this.strMonday = response.strMonday;
      this.strSaturday = response.strSaturday;
      this.weekDays = response.weekDays;
      this.nbDayTasks = response.nbDayTasks;

    }, error => {
      this.alertify.error(error);
    });
  }

  loadScores() {

  }

  toggleForm() {
    this.toggleFormAdd = !this.toggleFormAdd;
  }

  loadClassStudents(classId) {
    this.classService.getClassStudents(classId).subscribe(data => {
      this.students = data;
    }, error => {
      this.alertify.error(error);
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

  resetSchedule() {
    this.monCourses = [];
    this.tueCourses = [];
    this.wedCourses = [];
    this.thuCourses = [];
    this.friCourses = [];
    this.satCourses = [];
    this.sunCourses = [];
    }

}
