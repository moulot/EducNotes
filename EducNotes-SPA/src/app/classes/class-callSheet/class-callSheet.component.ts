import { Component, OnInit, Renderer2, ElementRef } from '@angular/core';
import { Class } from 'src/app/_models/class';
import { FormControl } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ClassService } from 'src/app/_services/class.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { debounceTime } from 'rxjs/operators';
import { SharedAnimations } from 'src/app/shared/animations/shared-animations';
import { Absence } from 'src/app/_models/absence';
import { environment } from 'src/environments/environment';
import { Session } from 'src/app/_models/session';
import { CallSheet } from 'src/app/_models/callSheet';
import { CardsComponent } from 'src/app/views/ui-kits/cards/cards.component';
import { fakeAsync } from '@angular/core/testing';

@Component({
  selector: 'app-class-callSheet',
  templateUrl: './class-callSheet.component.html',
  styleUrls: ['./class-callSheet.component.scss'],
  animations: [SharedAnimations]
})
export class ClassCallSheetComponent implements OnInit {
  classRoom: Class;
  searchControl: FormControl = new FormControl();
  schedule: any;
  session = <Session>{};
  sessionData: any;
  viewMode: 'list' | 'grid' = 'grid';
  allSelected: boolean;
  page = 1;
  pageSize = 50;
  students: any = [];
  filteredStudents: any;
  isAbsent: CallSheet[] = [];
  absents: number[] = [];
  absences: Absence[] = [];
  sessionAbsents: any = [];
  callSheetAbsence = environment.callSheetAbsence;

  constructor(private classService: ClassService, private alertify: AlertifyService,
    private route: ActivatedRoute, private router: Router) { }

  ngOnInit() {

    this.route.data.subscribe((data: any) => {
      this.sessionData = data['session'];
      this.schedule = this.sessionData.sessionSchedule;
      this.session = this.sessionData.session;
      this.sessionAbsents = this.sessionData.sessionAbsences;
      this.students = this.sessionData.classStudents;

      for (let i = 0; i < this.students.length; i++) {
        const id = this.students[i].id;
        const elt = <CallSheet>{};
        elt.id = id;
        elt.absent = false;
        this.students[i].isAbsent = false;

        if (this.sessionAbsents.length > 0) {
          if (this.sessionAbsents.findIndex(s => Number(s.userId) === Number(id)) !== -1) {
            elt.absent = true;
            this.absents = [...this.absents, id];
            this.students[i].isAbsent = true;
          }
        }
        this.isAbsent = [...this.isAbsent, elt];
      }
      this.filteredStudents = this.students;
    });

    this.searchControl.valueChanges.pipe(debounceTime(200)).subscribe(value => {
      this.filerData(value);
    });
  }

  // getSessionData(scheduleId) {
  //   this.classService.getSessionData(scheduleId).subscribe((session: any) => {
  //     this.session = session.session;
  //     this.getSessionAbsents(this.session.id);
  //     this.students = session.classStudents;
  //     this.sessionAbsents = session.sessionAbsences;
  //   }, error => {
  //     this.alertify.error(error);
  //   });
  // }

  // getSession(scheduleId) {
  //   this.classService.getSession(scheduleId).subscribe((session: Session) => {
  //     this.session = session;
  //     this.getSessionAbsents(this.session.id);
  //     this.loadStudents(this.schedule.classId);
  //   }, error => {
  //     this.alertify.error(error);
  //   });
  // }

  // loadStudents(classId) {
  //   this.classService.getClassStudents(classId).subscribe(data => {
  //     this.students = data;
  //     for (let i = 0; i < this.students.length; i++) {
  //       const id = this.students[i].id;
  //       let elt = <CallSheet>{};
  //       elt.id = id;
  //       elt.absent = false;
  //       if (this.sessionAbsents.length > 0) {
  //         if (this.sessionAbsents.findIndex(s => Number(s.userId) === Number(id)) !== -1) {
  //           elt.absent = true;
  //           this.absents = [...this.absents, id];
  //         }
  //       }
  //       this.isAbsent = [...this.isAbsent, elt];
  //     }
  //     this.filteredStudents = data;
  //   }, error => {
  //     this.alertify.error(error);
  //   });
  // }

  // getSessionAbsents(sessionId: number) {
  //   this.classService.getAbsencesBySession(sessionId).subscribe(data => {
  //     this.sessionAbsents = data;
  //     console.log(this.sessionAbsents);
  //     console.log('nb abs: ' + this.sessionAbsents.length);
  //   });
  // }

  filerData(val) {
    if (val) {
      val = val.toLowerCase();
    } else {
      return this.filteredStudents = [...this.students];
    }
    const columns = Object.keys(this.students[0]);
    if (!columns.length) {
      return;
    }

    const rows = this.students.filter(function(d) {
      for (let i = 0; i <= columns.length; i++) {
        const column = columns[i];
        if (d[column] && d[column].toString().toLowerCase().indexOf(val) > -1) {
          return true;
        }
      }
    });
    this.filteredStudents = rows;
  }

  // setAbsences(index, studentId) {
  setAbsences(absenceData) {
    const index = absenceData.index;
    const studentId = absenceData.studentId;
    this.isAbsent[index].absent = !this.isAbsent[index].absent;
    const isAbsent = this.isAbsent[index].absent;
    const id = Number(studentId);
    const student = this.students.find(s => s.id === studentId);

    if (isAbsent) {
      student.isAbsent = true;
      this.absents = [...this.absents, id];
    } else {
      student.isAbsent = false;
      const pos = this.absents.findIndex((value) => value === id);
      this.absents.splice(pos, 1);
    }
  }

  validateAbsences() {

    for (let i = 0; i < this.absents.length; i++) {
      const newAbsence = <Absence>{};
      newAbsence.userId = this.absents[i];
      newAbsence.absenceTypeId = this.callSheetAbsence;
      newAbsence.sessionId = this.session.id;
      newAbsence.startDate = this.schedule.startHourMin;
      newAbsence.endDate = this.schedule.endHourMin;
      newAbsence.reason = '';
      const justified = 0;
      newAbsence.comment = 'absent lors de l\'appel';
      this.absences = [...this.absences, newAbsence];
    }

    this.classService.saveCallSheet(this.session.id, this.absences).subscribe(() => {
      this.alertify.success('classe ' + this.schedule.className + ' - ' + this.schedule.courseName +
        'de ' + this.schedule.strStartHourMin + ' à ' + this.schedule.strEndHourMin + '. appel Validé');
    }, error => {
      this.alertify.error(error);
    }, () => {
      this.router.navigate(['/teacher']);
    });
  }

}
