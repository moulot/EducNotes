import { Component, OnInit, Input, ElementRef, EventEmitter, Output, ViewChild } from '@angular/core';
import { FormGroup, Validators, FormBuilder } from '@angular/forms';
import { User } from 'src/app/_models/user';
import { Absence } from 'src/app/_models/absence';
import { UserService } from 'src/app/_services/user.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { Router } from '@angular/router';
import { Utils } from 'src/app/shared/utils';
import { MDBDatePickerComponent, ClockPickerComponent } from 'ng-uikit-pro-standard';

@Component({
  selector: 'app-class-absence',
  templateUrl: './class-absence.component.html',
  styleUrls: ['./class-absence.component.scss']
})
export class ClassAbsenceComponent implements OnInit {
  @ViewChild('inputStartD', { static: true }) inputStartD: ElementRef;
  @ViewChild('inputEndD', { static: true }) inputEndD: ElementRef;
  @ViewChild('startDate', { static: true }) startDate: MDBDatePickerComponent;
  @ViewChild('startTime', { static: true }) startTime: ClockPickerComponent;
  @ViewChild('endDate', { static: true }) endDate: MDBDatePickerComponent;
  @ViewChild('endTime', { static: true }) endTime: ClockPickerComponent;
  @Input() students: User[];
  @Output() toggleForm = new EventEmitter<boolean>();
  @Output() loadAbsences = new EventEmitter<any>();
  absenceForm: FormGroup;
  newAbsence = <Absence>{};
  studentOptions = [];
  justifiedOptions = [{value: 0, label: 'NON'}, {value: 1, label: 'OUI'}];
  myDatePickerOptions = Utils.myDatePickerOptions;

  constructor(private userService: UserService, private fb: FormBuilder,
    private alertify: AlertifyService, private router: Router) { }

  ngOnInit() {
    this.createAbsenceForm();
    for (let i = 0; i < this.students.length; i++) {
      const student = this.students[i];
      const element = {value: student.id, label: student.lastName + ' ' + student.firstName};
      this.studentOptions = [...this.studentOptions, element];
    }
  }

  onSDateChange = (event: { actualDateFormatted: string; }) => {
    this.inputStartD.nativeElement.value = event.actualDateFormatted; // set value to input
    this.startDate.closeBtnClicked(); // close date picker
    this.startTime.openBtnClicked(); // open time picker
  }

  onSTimeChange = (event: string) => {
    this.inputStartD.nativeElement.value = `${this.inputStartD.nativeElement.value}, ${event}`; // set value to input
  }

  onEDateChange = (event: { actualDateFormatted: string; }) => {
    this.inputEndD.nativeElement.value = event.actualDateFormatted; // set value to input
    this.endDate.closeBtnClicked(); // close date picker
    this.endTime.openBtnClicked(); // open time picker
  }

  onETimeChange = (event: string) => {
    this.inputEndD.nativeElement.value = `${this.inputEndD.nativeElement.value}, ${event}`; // set value to input
  }

  createAbsenceForm() {
    this.absenceForm = this.fb.group({
      student: [null, Validators.required],
      startDate: ['', Validators.required],
      startTime: ['', Validators.required],
      endDate: ['', Validators.required],
      endTime: ['', Validators.required],
      reason: ['', Validators.required],
      justified: [null, Validators.required],
      type: [null, Validators.required],
      comment: ['', Validators.required]
    });
  }

  saveAbsence(more: boolean) {
    this.newAbsence.userId = this.absenceForm.value.student;
    this.newAbsence.absenceTypeId = this.absenceForm.value.type;
    const startDateData = this.absenceForm.value.startDate.split('/');
    const startTimeDate = this.absenceForm.value.startTime.split(':');
    this.newAbsence.startDate = new Date(startDateData[2], startDateData[1] - 1,
      startDateData[0], startTimeDate[0], startTimeDate[1]);
    const endDateData = this.absenceForm.value.endDate.split('/');
    const endTimeDate = this.absenceForm.value.endTime.split(':');
    this.newAbsence.endDate = new Date(endDateData[2], endDateData[1] - 1,
      endDateData[0], endTimeDate[0], endTimeDate[1]);
    this.newAbsence.reason = this.absenceForm.value.reason;
    const justified = this.absenceForm.value.justified;
    if (justified === '0') {
      this.newAbsence.justified = false;
    } else {
      this.newAbsence.justified = true;
    }
    this.newAbsence.comment = this.absenceForm.value.comment;
    this.userService.saveAbsence(this.newAbsence).subscribe(() => {
      this.alertify.success('ajout de l\'absence validé.');
    }, error => {
      this.alertify.error(error);
    }, () => {
      this.absenceForm.reset();
      if (more === false) {
        this.toggleForm.emit();
        this.loadAbsences.emit();
      }
    });
  }

  cancel() {
    this.absenceForm.reset();
  }

}
