import { Component, OnInit, Input, ElementRef, ViewChild, Renderer2, EventEmitter, Output } from '@angular/core';
import { CardRotatingComponent, MDBModalRef, MDBModalService } from 'ng-uikit-pro-standard';
import { ModalAddClassComponent } from '../modal-add-class/modal-add-class.component';

@Component({
  selector: 'app-class-card',
  templateUrl: './class-card.component.html',
  styleUrls: ['./class-card.component.scss']
})
export class ClassCardComponent implements OnInit {
  @ViewChild('card', { static: true }) flippingCard: CardRotatingComponent;
  @Input() level: any;
  @Output() reloadClasses = new EventEmitter();
  modalRef: MDBModalRef;

  constructor(private el: ElementRef, private renderer: Renderer2, private modalService: MDBModalService) { }

  ngOnInit() {
    const cardWrapper = this.el.nativeElement.querySelectorAll('.card-wrapper');
    this.renderer.setStyle(cardWrapper[0], 'height', '200px');
  }

  openModal(level) {
    const modalOptions = {
      backdrop: true,
      keyboard: false,
      focus: true,
      show: false,
      scroll: true,
      ignoreBackdropClick: false,
      class: 'modal-dialog modal-dialog-centered',
      containerClass: '',
      animated: true,
      data: { level: level } // { courses: courses, startHM: startHM, endHM: endHM, dayName: this.dayName }
    };
    this.modalRef = this.modalService.show(ModalAddClassComponent, modalOptions);
    this.modalRef.content.reloadClassesModal.subscribe(() => {
      this.reloadClasses.emit();
    });
  }

}
