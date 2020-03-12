import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { ClassService } from 'src/app/_services/class.service';
import { AlertifyService } from 'src/app/_services/alertify.service';

@Component({
  selector: 'app-classday-schedule',
  templateUrl: './classday-schedule.component.html',
  styleUrls: ['./classday-schedule.component.scss']
})
export class ClassdayScheduleComponent implements OnInit {
  @Input() items = [];
  @Output() reloadSchedule = new EventEmitter();

  constructor(private classService: ClassService, private alertify: AlertifyService) { }

  ngOnInit() {
  }

  deleteCourse(scheduleId) {
    if (confirm('delete really?')) {
      this.classService.delCourseFromSchedule(scheduleId).subscribe(() => {
        this.alertify.success('le cours a bien été supprimé');
      }, error => {
        this.alertify.error(error);
      }, () => {
        this.reloadSchedule.emit();
      });
    }
  }

}
