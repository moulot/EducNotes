import { Component, OnInit, HostListener, ɵConsole } from '@angular/core';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { Establishment } from 'src/app/_models/establishment';
import { AdminService } from 'src/app/_services/admin.service';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-school',
  templateUrl: './school.component.html',
  styleUrls: ['./school.component.scss']
})
export class SchoolComponent implements OnInit {
  loading: boolean;
  schoolForm: FormGroup;
  school = <Establishment>{};
  phoneMask = [/\d/, /\d/, '-', /\d/, /\d/, '-', /\d/, /\d/, '-', /\d/, /\d/];
  timeMask = [/\d/, /\d/, ':', /\d/, /\d/];
  @HostListener('window:beforeunload', ['$event'])
  unloadNotification($event: any) {
    if (this.schoolForm.dirty) {
      $event.returnValue = true;
    }
  }

  constructor(private adminService: AdminService, private alertify: AlertifyService,
    private fb: FormBuilder, private route: ActivatedRoute, private router: Router) { }

  ngOnInit() {
    this.route.data.subscribe(data => {
      this.school = data['school'];
    });
    this.createSchoolForm();
  }

  createSchoolForm() {
    const startDate = new Date(this.school.startCoursesHour);
    const startDateH = startDate.getHours().toString();
    const startH = startDateH.length === 1 ? '0' + startDateH : startDateH;
    const startDateM = startDate.getMinutes().toString();
    const startM = startDateM.length === 1 ? '0' + startDateM : startDateM;

    const endDate = new Date(this.school.endCoursesHour);
    const endDateH = endDate.getHours().toString();
    const endH = endDateH.length === 1 ? '0' + endDateH : endDateH;
    const endDateM = endDate.getMinutes().toString();
    const endM = endDateM.length === 1 ? '0' + endDateM : endDateM;

    this.schoolForm = this.fb.group({
      name: [this.school.name],
      phone: [this.school.phone],
      email: [this.school.email],
      webSite: [this.school.webSite],
      startCoursesHour: [startH + ':' + startM, [Validators.minLength(4), Validators.maxLength(4)]],
      endCoursesHour: [endH + ':' + endM, [Validators.minLength(4), Validators.maxLength(4)]]
    });
  }

  saveInfos() {

    this.loading = true;

    this.school.name = this.schoolForm.value.name;
    this.school.phone = this.schoolForm.value.phone;
    this.school.email = this.schoolForm.value.email;
    this.school.webSite = this.schoolForm.value.webSite;

    const startHM = this.schoolForm.value.startCoursesHour.split(':');
    const startH = Number(startHM[0]);
    const startM = Number(startHM[1]);
    this.school.startCoursesHour = new Date(2019, 0, 1, startH, startM);

    const endHM = this.schoolForm.value.endCoursesHour.split(':');
    const endH = Number(endHM[0]);
    const endM = Number(endHM[1]);
    this.school.endCoursesHour = new Date(2019, 0, 1, endH, endM);

    this.adminService.saveSchool(this.school).subscribe(() => {
      this.alertify.successBar('infos de l\'école mises à jour');
      this.loading = false;
      this.router.navigate(['/home']);
    }, error => {
      this.alertify.errorBar(error);
      this.loading = false;
    });

  }

}
