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
    console.log(index);
    console.log(this.showData);
    this.showData[index] = !this.showData[index];
  }

  prevWeek() {

  }

  nextWeek() {

  }

}
