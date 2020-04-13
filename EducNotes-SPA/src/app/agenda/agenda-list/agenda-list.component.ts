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

@Component({
  selector: 'app-agenda-list',
  templateUrl: './agenda-list.component.html',
  styleUrls: ['./agenda-list.component.css'],
  animations: [SharedAnimations]
})
export class AgendaListComponent implements OnInit {
  @ViewChild('agendaForm', {static: false}) agendaForm: NgForm;
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
  searchControl: FormControl = new FormControl();
  viewMode: 'list' | 'grid' = 'list';
  allSelected: boolean;
  page = 1;
  pageSize = 8;
  sessions: any[] = [];
  filteredSessions;
  optionsClass: any[] = [];

  allCourses = true;
  coursesWithTasks: any = [];
  nbDayTasks = [0, 0, 0, 0, 0, 0];
  weekDays = [];
  weekDates = [];
  classControl = new FormControl();
  showSessionsData = false;
  agendaParams: any = {};
  startDate: Date;
  sessionsByDate = [];
  sessionsByCourse = [];

  constructor(private userService: UserService, private fb: FormBuilder, private classService: ClassService,
    private authService: AuthService, public alertify: AlertifyService, private modalService: NgbModal) { }

  ngOnInit() {
    this.teacher = this.authService.currentUser;
    this.getTeacherClasses(this.teacher.id);
    this.getTeacherCourses(this.teacher.id);
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
      this.filteredSessions = this.allSessions;
    }
  }

  classChanged() {
    if (this.classControl.value !== '') {
      this.classId = this.classControl.value;
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
      console.log(result);
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
  }

  showItemsByDate(selectedDate) {
    this.allCourses = false;
    this.filteredSessions = [];
    this.sessionsByDate = [];

    const sessionsDay = this.allSessions.find(item => item.dueDate === selectedDate);
    this.filteredSessions = [...this.filteredSessions, sessionsDay];
  }

  loadMovedWeek(move: number) {
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
