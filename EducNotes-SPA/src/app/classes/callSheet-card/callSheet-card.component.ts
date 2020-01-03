import { Component, OnInit, Input, Renderer2, ElementRef, Output, EventEmitter, ViewChild } from '@angular/core';
import { first } from 'rxjs/operators';
import { CardRotatingComponent } from 'ng-uikit-pro-standard';

@Component({
  selector: 'app-callSheet-card',
  templateUrl: './callSheet-card.component.html',
  styleUrls: ['./callSheet-card.component.scss']
})
export class CallSheetCardComponent implements OnInit {
  @Input() student: any;
  @Input() index: number;
  @Output() setAbsences = new EventEmitter<any>();
  firstClick = true;

  constructor(private renderer: Renderer2, private el: ElementRef) { }
  @ViewChild('cards', { static: true }) flippingCard: CardRotatingComponent;

  ngOnInit() {
    const cardWrapper = this.el.nativeElement.querySelectorAll('.card-wrapper');
    this.renderer.setStyle(cardWrapper[0], 'height', '120px');

    if (this.student.isAbsent) {
      // const cardRotating = this.el.nativeElement.querySelectorAll('.card-rotating');
      // this.renderer.addClass(cardRotating[0], 'flipped');
      this.flippingCard.toggle();
    }
  }

  addAbsent(index, studentId) {
    // if (this.firstClick) {
    //   if (this.student.isAbsent) {
    //     const cardRotating = this.el.nativeElement.querySelectorAll('.card-rotating');
    //     this.renderer.removeClass(cardRotating[0], 'flipped');
    //   }
    //   this.firstClick = false;
    // }

    const absenceData = <any>{};
    absenceData.index = index;
    absenceData.studentId = studentId;
    this.setAbsences.emit(absenceData);
  }

}
