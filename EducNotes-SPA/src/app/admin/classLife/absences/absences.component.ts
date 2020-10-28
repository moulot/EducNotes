import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { ClassService } from 'src/app/_services/class.service';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-absences',
  templateUrl: './absences.component.html',
  styleUrls: ['./absences.component.scss']
})
export class AbsencesComponent implements OnInit {
  absenceType = environment.absenceType;
  lateType = environment.lateType;
  weeklyAbs: any;
  showDetails = false;
  day: any;
  showData = [];
  moveWeekData: any;

  constructor(private classService: ClassService, private alertify: AlertifyService,
    private router: Router) { }

  ngOnInit() {
    this.getWeeklyAbsences();
  }

  getWeeklyAbsences() {
    this.classService.getCurrentWeekAbsences().subscribe(data => {
      this.weeklyAbs = data;
    }, error => {
      this.alertify.error(error);
    });
  }

  goToDayPage(dayData) {
    this.day = dayData;
    for (let i = 0; i < this.day.classes.length; i++) {
      this.showData = [...this.showData, true];
    }
    this.showDetails = true;
  }

  goBack() {
    this.showDetails = false;
  }

  showHideDetails(index) {
    this.showData[index] = !this.showData[index];
  }

  prevWeek() {
    const moveData = <any>{};
    moveData.startDate = this.weeklyAbs.startDate;
    moveData.moveDays = -7;
    this.classService.getWeekAbsences(moveData).subscribe(data => {
      this.weeklyAbs = data;
    }, error => {
      this.alertify.error(error);
    });
  }

  nextWeek() {
    const moveData = <any>{};
    moveData.startDate = this.weeklyAbs.startDate;
    moveData.moveDays = 7;
    this.classService.getWeekAbsences(moveData).subscribe(data => {
      this.weeklyAbs = data;
    }, error => {
      this.alertify.error(error);
    });
  }

}
