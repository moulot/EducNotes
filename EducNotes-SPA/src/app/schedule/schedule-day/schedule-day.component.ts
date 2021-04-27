import { Component, OnInit, Input, EventEmitter, Output } from '@angular/core';
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

  constructor(private alertify: AlertifyService, private classService: ClassService) { }

  ngOnInit() {
    console.log('tt');
    console.log(this.items);
  }

  deleteCourse(scheduleId, dayName, delInfo) {
    if (confirm('voulez vous vraiment effacer le cours du ' + dayName + ': ' + delInfo)) {
      this.classService.delCourseFromSchedule(scheduleId).subscribe((deleteOk: boolean) => {
        if (deleteOk === true) {
          this.alertify.success('le cours a bien été supprimé');
        } else {
          this.alertify.info('le cours du ' + dayName + ': ' + delInfo + ' ne peut être supprimé, il a déjà été utilisé. merci.');
        }
      }, error => {
        this.alertify.error(error);
      }, () => {
        this.reloadSchedule.emit();
      });
    }
  }

  showConflictDetails() {
    alert('conflict details');
  }
}
