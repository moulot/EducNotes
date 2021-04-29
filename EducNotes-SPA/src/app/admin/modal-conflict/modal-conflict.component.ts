import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { MDBModalRef } from 'ng-uikit-pro-standard';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { ClassService } from 'src/app/_services/class.service';

@Component({
  selector: 'app-modal-conflict',
  templateUrl: './modal-conflict.component.html',
  styleUrls: ['./modal-conflict.component.scss']
})
export class ModalConflictComponent implements OnInit {
  courses: any;
  startHM: string;
  endHM: string;
  dayName: string;
  @Output() reloadScheduleModal = new EventEmitter();
  modalRef: MDBModalRef;

  constructor(public classService: ClassService, public alertify: AlertifyService, public activeModal: MDBModalRef) { }

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
        this.reloadScheduleModal.emit();
        this.activeModal.hide();
      });
    }
  }

}
