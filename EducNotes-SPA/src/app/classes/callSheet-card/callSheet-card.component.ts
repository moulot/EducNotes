import { Component, OnInit, Input, Renderer2, ElementRef, Output, EventEmitter, ViewChild } from '@angular/core';
import { first } from 'rxjs/operators';
import { CardRotatingComponent } from 'ng-uikit-pro-standard';
import { FormControl, Validators, FormBuilder, FormGroup } from '@angular/forms';

@Component({
  selector: 'app-callSheet-card',
  templateUrl: './callSheet-card.component.html',
  styleUrls: ['./callSheet-card.component.scss']
})
export class CallSheetCardComponent implements OnInit {
  @Input() student: any;
  @Input() index: number;
  @Output() setAbsences = new EventEmitter<any>();
  @Output() removeAbsence = new EventEmitter<any>();
  isAbsent = false;
  lateForm: FormGroup;
  lateValidated = true;

  constructor(private renderer: Renderer2, private el: ElementRef,
    private fb: FormBuilder) { }
  @ViewChild('card', { static: true }) flippingCard: CardRotatingComponent;

  ngOnInit() {
    this.createLateForm();
    const cardWrapper = this.el.nativeElement.querySelectorAll('.card-wrapper');
    this.renderer.setStyle(cardWrapper[0], 'height', '155px');

    if (this.student.absent || this.student.late) {
      this.flippingCard.toggle();
      if (this.student.absent) {
        this.isAbsent = true;
      } else {
        this.isAbsent = false;
      }
    }
  }

  createLateForm() {
    this.lateForm = this.fb.group({
      minutes: ['', [Validators.required, Validators.maxLength(2)]]// , Validators.pattern('/^-?(0|[1-9]\d*)?$/')]]
    });
  }

  flipLate() {
    this.isAbsent = false;
    this.lateValidated = false;
  }

  addAbsent(index, studentId, isAbsent) {
    // for late arrival
    const lateInMin = this.lateForm.value.minutes;
    this.lateValidated = true;

    this.isAbsent = isAbsent;
    const absenceData = <any>{};
    absenceData.index = index;
    absenceData.studentId = studentId;
    absenceData.isAbsent = isAbsent;
    absenceData.lateInMin = lateInMin;
    this.setAbsences.emit(absenceData);
  }

  cancelAbsent(studentId) {
    this.lateForm.controls['minutes'].setValue('');
    const absenceData = <any>{};
    absenceData.studentId = studentId;
    absenceData.lateValidated = this.lateValidated;
    this.removeAbsence.emit(absenceData);
  }

}
