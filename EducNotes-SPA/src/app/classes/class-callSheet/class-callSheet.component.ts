import { Component, OnInit } from '@angular/core';
import { Class } from 'src/app/_models/class';
import { FormControl } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ClassService } from 'src/app/_services/class.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { debounceTime } from 'rxjs/operators';
import { SharedAnimations } from 'src/app/shared/animations/shared-animations';
import { Absence } from 'src/app/_models/absence';
import { environment } from 'src/environments/environment';
import { AuthService } from 'src/app/_services/auth.service';

@Component({
  selector: 'app-class-callSheet',
  templateUrl: './class-callSheet.component.html',
  styleUrls: ['./class-callSheet.component.scss'],
  animations: [SharedAnimations]
})
export class ClassCallSheetComponent implements OnInit {
  absenceType = environment.absenceType;
  lateType = environment.lateType;
  classRoom: Class;
  searchControl: FormControl = new FormControl();
  session: any;
  sessionData: any;
  students: any = [];
  filteredStudents: any;
  absences: Absence[] = [];
  sessionAbsents: any = [];
  touched = -1;
  nbAbsents = 0;
  nbLate = 0;

  constructor(private classService: ClassService, private alertify: AlertifyService,
    private route: ActivatedRoute, private router: Router, private authService: AuthService) { }

  ngOnInit() {
    this.route.data.subscribe((data: any) => {
      this.sessionData = data['session'];
      this.session = this.sessionData.session;
      this.sessionAbsents = this.sessionData.sessionAbsences;
      this.students = this.sessionData.classStudents;
      if (this.sessionAbsents.length > 0) {
        for (let i = 0; i < this.students.length; i++) {
          const id = this.students[i].id;
          this.students[i].absent = false;
          this.students[i].late = false;
          const pos = this.sessionAbsents.findIndex(s => Number(s.userId) === Number(id));
          if (pos !== -1) {
            const abs = this.sessionAbsents[pos];
            if (abs.absenceTypeId === this.absenceType) {
              // it's an absence
              this.nbAbsents++;
              this.students[i].absent = true;
              this.students[i].late = false;
            } else if (abs.absenceTypeId === this.lateType) {
              // it's a late arrival
              this.nbLate++;
              this.students[i].absent = false;
              this.students[i].late = true;
            }
            this.students[i].lateDateTime = abs.endDate;
            this.students[i].lateInMin = abs.lateInMin;
          }
        }
      } else {
        for (let i = 0; i < this.students.length; i++) {
          this.students[i].absent = false;
          this.students[i].late = false;
        }
      }
      this.filteredStudents = this.students;
    });

    this.searchControl.valueChanges.pipe(debounceTime(200)).subscribe(value => {
      this.filerData(value);
    });
  }

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

  getStudents(classId, absents) {
    this.classService.getCallSheetStudents(classId).subscribe(data => {
      this.students = data;
      this.filteredStudents = data;
      for (let i = 0; i < this.students.length; i++) {
        console.log('i:' + i);
        const id = this.students[i].id;
        this.students[i].absent = false;
        this.students[i].late = false;
        console.log('nb abs: ' + absents.length);
        if (absents.length > 0) {
          const pos = absents.findIndex(s => Number(s.userId) === Number(id));
          console.log('pos: ' + pos);
          if (pos !== -1) {
            const abs = absents[pos];
            if (abs.absenceTypeId === this.absenceType) {
              // it's an absence
              this.nbAbsents++;
              this.students[i].absent = true;
              this.students[i].late = false;
            } else if (abs.absenceTypeId === this.lateType) {
              // it's a late arrival
              this.nbLate++;
              this.students[i].absent = false;
              this.students[i].late = true;
            }
            this.students[i].lateDateTime = abs.endDate;
            this.students[i].lateInMin = abs.lateInMin;
          }
        }
      }
      this.filteredStudents = this.students;
      console.log(this.students);
    }, error => {
      this.alertify.error(error);
    });
  }

  setAbsences(absenceData) {
    const index = absenceData.index;
    const isAbsent = absenceData.isAbsent;
    const lateInMin = absenceData.lateInMin;
    this.students[index].lateInMin = lateInMin;
    // is it a absence or a late arrival?
    if (isAbsent) {
      this.students[index].absent = isAbsent;
      this.students[index].late = false;
      this.nbAbsents++;
    } else {
      this.students[index].absent = isAbsent;
      this.students[index].late = true;
      this.nbLate++;
    }
  }

  removeAbsence(absenceData) {
    const studentId = absenceData.studentId;
    const lateValidated = absenceData.lateValidated;
    const pos = this.students.findIndex(elt => elt.id === studentId);
    this.students[pos].absent = false;
    this.students[pos].late = false;
    if (lateValidated) {
      this.nbLate--;
    }
  }

  validateAbsences() {
    for (let i = 0; i < this.students.length; i++) {
        const newAbsence = <Absence>{};
      const student = this.students[i];
      if (student.absent || student.late) {
        newAbsence.userId = student.id;
        newAbsence.sessionId = this.session.id;

        const sessionDate = this.session.strSessionDate.split('/');
        const day = sessionDate[0];
        const month = sessionDate[1];
        const year = sessionDate[2];
        const shour = this.session.startHourMin.split(':')[0];
        const smin = this.session.startHourMin.split(':')[1];
        const ehour = this.session.endHourMin.split(':')[0];
        const emin = this.session.endHourMin.split(':')[1];

        newAbsence.startDate = new Date(year, month - 1, day, shour, smin);
        if (student.absent) {
          newAbsence.absenceTypeId = this.absenceType;
          newAbsence.endDate = new Date(year, month - 1, day, ehour, emin);
        } else { // late arrival
          newAbsence.absenceTypeId = this.lateType;
          const endLateMin = Number(smin) + Number(student.lateInMin);
          newAbsence.endDate = new Date(year, month - 1, day, shour, endLateMin);
        }
        newAbsence.reason = 'absent lors de l\'appel du cours de ';
        newAbsence.doneById = this.authService.currentUser.id;
        this.absences = [...this.absences, newAbsence];
      }
    }

    this.classService.saveCallSheet(this.session.id, this.absences).subscribe(() => {
      this.alertify.success('l\'appel est enregistré');
    }, error => {
      this.alertify.error(error);
    }, () => {
      this.router.navigate(['/classSession', this.session.scheduleCourseId]);
    });
  }

}
