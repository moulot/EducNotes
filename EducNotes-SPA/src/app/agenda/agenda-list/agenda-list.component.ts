import { Component, OnInit, ViewChild } from '@angular/core';
import { NgbModal, NgbModalRef } from '@ng-bootstrap/ng-bootstrap';
import { UserService } from 'src/app/_services/user.service';
import { AuthService } from 'src/app/_services/auth.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { User } from 'src/app/_models/user';
import { Agenda } from 'src/app/_models/agenda';
import { NgForm, FormGroup, FormBuilder, FormControl } from '@angular/forms';
import { ClassService } from 'src/app/_services/class.service';
import { CourseUser } from 'src/app/_models/courseUser';
import { SharedAnimations } from 'src/app/shared/animations/shared-animations';
import { AgendaModalComponent } from '../agenda-modal/agenda-modal.component';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-agenda-list',
  templateUrl: './agenda-list.component.html',
  styleUrls: ['./agenda-list.component.css'],
  animations: [SharedAnimations]
})
export class AgendaListComponent implements OnInit {
  // @ViewChild('agendaForm', {static: false}) agendaForm: NgForm;
  classId: number;
  modalSession: any;
  allSessions: any = [];
  selectedSessions: any = [];
  teacher: User;
  teacherClasses: any;
  teacherCourses: any;
  agendaForSave = <Agenda>{};
  model: any = {};
  color_width = '50px';
  ngbModalRef: NgbModalRef;
  sessions: any[] = [];
  filteredSessions;
  optionsClass = [];
  allCourses = true;
  coursesWithTasks: any = [];
  nbDayTasks = [0, 0, 0, 0, 0, 0];
  weekDays = [];
  weekDates = [];
  classControl = new FormControl();
  classForm: FormGroup;
  showSessionsData = false;
  agendaParams: any = {};
  startDate: Date;
  sessionsByDate = [];
  sessionsByCourse = [];
  dayOfWeekActive = -1;

  constructor(private userService: UserService, private fb: FormBuilder, private classService: ClassService,
    private authService: AuthService, public alertify: AlertifyService, private modalService: NgbModal,
    private route: ActivatedRoute) { }

  ngOnInit() {
    this.teacher = this.authService.currentUser;
    this.getTeacherClasses(this.teacher.id);
    this.getTeacherCourses(this.teacher.id);
    this.route.params.subscribe(params => {
      this.classId = params['classId'];
      if (this.classId > 0) {
        this.classSelected(this.classId);
      }
      this.createClassForm();
    });
  }

  createClassForm() {
    this.classForm = this.fb.group({
      classRoom: [this.classId]
    });
  }

  resetSessions() {
    this.filteredSessions = this.allSessions;
  }

  editTasks(session) {
    this.ngbModalRef = this.modalService.open(AgendaModalComponent, {
      ariaLabelledBy: 'modal-basic-title',
      size: 'lg',
      centered: true
    });

    setTimeout(() => {
      const instance = this.ngbModalRef.componentInstance;
      instance.session = session;
      instance.saveAgenda.subscribe((data) => {
        this.saveAgenda(data);
        if (data.id === 0) {
          const itemIndex = this.coursesWithTasks.findIndex(item => item.courseId === data.courseId);
          const nb = this.coursesWithTasks[itemIndex].nbTasks;
          this.coursesWithTasks[itemIndex].nbTasks = nb + 1;
        }
      });
    }, 200);
  }

  saveAgenda(session) {
    this.agendaForSave.id = session.id;
    this.agendaForSave.sessionId = session.sessionId;
    this.agendaForSave.classId = session.classId;
    this.agendaForSave.courseId = session.courseId;
    this.agendaForSave.taskDesc = session.tasks;

    return this.classService.saveAgendaItem(this.agendaForSave).subscribe(() => {
      this.alertify.success(session.courseName + '. devoirs du ' + session.strDayDate + ' validés!');
    }, error => {
      this.alertify.error(error);
    });
  }

  getTeacherSessions(teacherId, classId) {
    this.userService.getTeacherSessionsFromToday(teacherId, classId).subscribe((data: any) => {
      this.filteredSessions = data.agendas;
      this.allSessions = data.agendas;
      this.startDate = data.today;
      this.weekDays = data.weekDays;
      this.weekDates = data.weekDates;
      this.coursesWithTasks = data.coursesWithTasks;
    }, error => {
      this.alertify.error(error);
    });
  }

  getTeacherClasses(teacherId) {
    this.userService.getTeacherClasses(teacherId).subscribe((courses: CourseUser[]) => {
      this.teacherClasses = courses;
      for (let i = 0; i < this.teacherClasses.length; i++) {
        const elt = this.teacherClasses[i];
        const element = {value: elt.classId, label: 'classe ' + elt.className};
        this.optionsClass = [...this.optionsClass, element];
      }
      // this.optionsClass = [...this.optionsClass, {value: 2, label: 'classe XXX'}];
    }, error => {
      this.alertify.error(error);
    });
  }

  getTeacherCourses(teacherId) {
    this.userService.getTeacherCourses(teacherId).subscribe(courses => {
      this.teacherCourses = courses;
    }, error => {
      this.alertify.error(error);
    });
  }

  showAllCourses() {
    if (this.allCourses === true) {
      this.dayOfWeekActive = -1;
      this.filteredSessions = this.allSessions;
      this.setCoursesWithTasks(this.filteredSessions);
    }
  }

  classSelected(classId) {
    this.classId = classId;
    this.getTeacherSessions(this.teacher.id, this.classId);
    this.showSessionsData = true;
  }

  classChanged() {
    const classId = this.classForm.value.classRoom;
    if (classId !== '') {
      this.classId = this.classForm.value.classRoom;
      this.getTeacherSessions(this.teacher.id, this.classId);
      this.showSessionsData = true;
    } else {
      this.showSessionsData = false;
    }
  }

  showCourseItems(courseId) {
    this.allCourses = false;
    this.filteredSessions = [];
    this.sessionsByCourse = [];

    for (let i = 0; i < this.allSessions.length; i++) {
      const elt = this.allSessions[i];
      const result = elt.agendaItems.map(item => {
        if (item.courseId === courseId) {
          return item;
        }
      }).filter(item => !!item);
      if (result.length > 0) {
        const filteredElt = {
          'dueDate': elt.dueDate,
          'shortDueDate': elt.shortDueDate,
          'longDueDate': elt.longDueDate,
          'dueDateAbbrev': elt.DueDateAbbrev,
          'nbItems': result.length,
          'agendaItems': result
        };
        this.filteredSessions = [...this.filteredSessions, filteredElt];
        this.sessionsByCourse = [...this.sessionsByDate, filteredElt];
      }
    }
    this.setCoursesWithTasks(this.filteredSessions);
  }

  showItemsByDate(selectedDate, index) {
    this.dayOfWeekActive = index;
    this.allCourses = false;
    this.filteredSessions = [];
    this.sessionsByDate = [];

    const sessionsDay = this.allSessions.find(item => item.dueDate === selectedDate);
    this.filteredSessions = [...this.filteredSessions, sessionsDay];
    this.setCoursesWithTasks(this.filteredSessions);
  }

  setCoursesWithTasks(sessions) {
    for (let i = 0; i < this.coursesWithTasks.length; i++) {
      const elt = this.coursesWithTasks[i];
      this.coursesWithTasks[i].nbTasks = 0;
      const courseId = elt.courseId;
      let nbTasks = 0;
      for (let j = 0; j < sessions.length; j++) {
        const session = sessions[j];
        const items = session.agendaItems;
        for (let k = 0; k < items.length; k++) {
          const item = items[k];
          if (item.courseId === courseId && item.id > 0) {
            nbTasks++;
          }
        }
        this.coursesWithTasks.find(c => c.courseId === courseId).nbTasks = nbTasks;
      }
    }
  }

  loadMovedWeek(move: number) {
    this.dayOfWeekActive = -1;
    this.allCourses = true;
    this.agendaParams.dueDate = this.startDate;
    this.agendaParams.moveWeek = move;

    this.userService.getMovedWeekSessions(this.teacher.id, this.classId, this.agendaParams).subscribe((data: any) => {
      this.sessions = data.agendas;
      this.filteredSessions = data.agendas;
      this.allSessions = data.agendas;
      this.startDate = data.date;
      this.weekDays = data.weekDays;
      this.weekDates = data.weekDates;
      this.coursesWithTasks = data.coursesWithTasks;
    }, error => {
      this.alertify.error(error);
    });
  }

}
