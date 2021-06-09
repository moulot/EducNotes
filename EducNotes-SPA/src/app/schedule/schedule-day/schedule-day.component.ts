import { Component, OnInit, Input, EventEmitter, Output } from '@angular/core';
import { MDBModalRef, MDBModalService } from 'ng-uikit-pro-standard';
import { ModalConflictComponent } from 'src/app/admin/modal-conflict/modal-conflict.component';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { ClassService } from 'src/app/_services/class.service';

@Component({
  selector: 'app-schedule-day',
  templateUrl: './schedule-day.component.html',
  styleUrls: ['./schedule-day.component.css']
})
export class ScheduleDayComponent implements OnInit {
  @Input() items = [];
  @Input() dayName: string;
  @Output() reloadSchedule = new EventEmitter();
  modalRef: MDBModalRef;

  constructor(private alertify: AlertifyService, private classService: ClassService,
    private modalService1: MDBModalService) { }

  ngOnInit() {
  }

  deleteCourse(scheduleCourseId, delInfo) {
    if (confirm('voulez vous vraiment effacer le cours du ' + this.dayName + ': ' + delInfo)) {
      this.classService.delCourseFromSchedule(scheduleCourseId).subscribe((deleteOk: boolean) => {
        if (deleteOk === true) {
          this.alertify.success('le cours a bien été supprimé');
        } else {
          this.alertify.info('le cours du ' + this.dayName + ': ' + delInfo + ' ne peut être supprimé, il a déjà été utilisé. merci.');
        }
      }, error => {
        this.alertify.error(error);
      }, () => {
        this.reloadSchedule.emit();
      });
    }
  }

  openModal(courses, startHM, endHM) {
    const modalOptions = {
      backdrop: true,
      keyboard: false,
      focus: true,
      show: false,
      scroll: true,
      ignoreBackdropClick: false,
      class: 'modal-xl',
      containerClass: '',
      animated: true,
      data: { courses: courses, startHM: startHM, endHM: endHM, dayName: this.dayName }
    };
    this.modalRef = this.modalService1.show(ModalConflictComponent, modalOptions);
    this.modalRef.content.reloadScheduleModal.subscribe(() => {
      this.reloadSchedule.emit();
    });
  }

}
