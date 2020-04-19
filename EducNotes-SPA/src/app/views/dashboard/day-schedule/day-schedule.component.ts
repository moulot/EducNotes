import { Component, OnInit, Input } from '@angular/core';
import { ClassService } from 'src/app/_services/class.service';
import { AlertifyService } from 'src/app/_services/alertify.service';

@Component({
  selector: 'app-day-schedule',
  templateUrl: './day-schedule.component.html',
  styleUrls: ['./day-schedule.component.scss']
})
export class DayScheduleComponent implements OnInit {
  @Input() items = [];
  @Input() dayName: string;

  constructor(private classService: ClassService, private alertify: AlertifyService) { }

  ngOnInit() {
  }

  deleteCourse(scheduleId, dayName, delInfo) {
    if (confirm('voulez vous vraiment effacer le cours du ' + dayName + ': ' + delInfo)) {
      this.classService.delCourseFromSchedule(scheduleId).subscribe(() => {
        this.alertify.success('le cours a bien été supprimé');
      }, error => {
        this.alertify.error(error);
      });
    }
  }

}
