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
import { debounceTime } from 'rxjs/operators';
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
  selectForm: FormGroup;
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

  constructor(private userService: UserService, private fb: FormBuilder,
    private classService: ClassService, private authService: AuthService,
    public alertify: AlertifyService, private modalService: NgbModal) { }

  ngOnInit() {
    this.createSelectForm();
    this.teacher = this.authService.currentUser;
    this.getTeacherSessions(this.teacher.id);
    this.getTeacherClasses(this.teacher.id);
    this.getTeacherCourses(this.teacher.id);

    this.searchControl.valueChanges.pipe(debounceTime(200)).subscribe(value => {
      this.filerData(value);
    });

  }

  filerData(val) {
    if (val) {
      val = val.toLowerCase();
    } else {
      return this.sessions = [...this.sessions];
    }
    const columns = Object.keys(this.sessions[0]);
    if (!columns.length) {
      return;
    }

    const rows = this.sessions.filter(function(d) {
      for (let i = 0; i <= columns.length; i++) {
        const column = columns[i];
        if (d[column] && d[column].toString().toLowerCase().indexOf(val) > -1) {
          return true;
        }
      }
    });
    this.filteredSessions = rows;
  }

  createSelectForm() {
    this.selectForm = this.fb.group({
      aclass: [null],
      course: [null],
      dates: [null]
    });
  }

  selectCourses() {
    const classid = Number(this.selectForm.value.aclass);
    const courseid = Number(this.selectForm.value.course);
    this.selectedSessions = [];
    console.log('sess len:' + this.allSessions.length + ' - ' + classid + ' - ' + courseid);
    for (let i = 0; i < this.allSessions.length; i++) {
      const elt = this.allSessions[i];
      if (Number(elt.classId) === classid && Number(elt.courseId) === courseid) {
        this.selectedSessions = [...this.selectedSessions, elt];
      }
    }

    this.filteredSessions = this.selectedSessions;
  }

  resetSessions() {
    this.filteredSessions = this.allSessions;
    this.selectForm.reset();
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
      instance.fct.subscribe((data) => {
        this.saveAgenda(data);
      });
    }, 200);
  }

  saveAgenda(session) {
    this.agendaForSave.id = session.id;
    this.agendaForSave.classId = session.classId;
    this.agendaForSave.courseId = session.courseId;
    this.agendaForSave.dueDate = session.dayDate;
    this.agendaForSave.taskDesc = session.tasks;

    return this.classService.saveAgendaItem(this.agendaForSave).subscribe(() => {
      this.alertify.success(session.courseName + '. devoirs du ' + session.strDayDate + ' validés!');
    }, error => {
      this.alertify.error(error);
    });
  }

  getTeacherSessions(teacherId) {
    this.userService.getTeacherSessions(teacherId).subscribe((sessions: any[]) => {
      this.sessions = sessions;
      this.filteredSessions = sessions;
      this.allSessions = sessions;
    }, error => {
      this.alertify.error(error);
    });
  }

  getTeacherClasses(teacherId) {
    this.userService.getTeacherClasses(teacherId).subscribe((courses: CourseUser[]) => {
      this.teacherClasses = courses;
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

}
