import { Component, OnInit, Input, ElementRef, EventEmitter, Output } from '@angular/core';
import { FormGroup, Validators, FormBuilder } from '@angular/forms';
import { User } from 'src/app/_models/user';
import { Absence } from 'src/app/_models/absence';
import { UserService } from 'src/app/_services/user.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-class-absence',
  templateUrl: './class-absence.component.html',
  styleUrls: ['./class-absence.component.css']
})
export class ClassAbsenceComponent implements OnInit {
  @Input() students: User[];
  @Output() toggleForm = new EventEmitter<boolean>();
  @Output() loadAbsences = new EventEmitter<any>();
  absenceForm: FormGroup;
  newAbsence = <Absence>{};

  constructor(private userService: UserService, private fb: FormBuilder,
    private alertify: AlertifyService, private router: Router) { }

  ngOnInit() {
    this.createAbsenceForm();
  }

  createAbsenceForm() {
    this.absenceForm = this.fb.group({
      student: [null, Validators.required],
      dates: [null, Validators.required],
      reason: ['', Validators.required],
      justified: [null, Validators.required],
      type: [null, Validators.required],
      comment: ['', Validators.required]
    });
  }

  saveAbsence(more: boolean) {
    this.newAbsence.userId = this.absenceForm.value.student;
    this.newAbsence.absenceTypeId = this.absenceForm.value.type;
    this.newAbsence.startDate = this.absenceForm.value.dates[0];
    this.newAbsence.endDate = this.absenceForm.value.dates[1];
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
