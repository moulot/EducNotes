import { Component, OnInit } from '@angular/core';
import { AuthService } from 'src/app/_services/auth.service';
import { Setting } from 'src/app/_models/setting';
import { Utils } from 'src/app/shared/utils';
import { OrderService } from 'src/app/_services/order.service';
import { environment } from 'src/environments/environment';
import { AdminService } from 'src/app/_services/admin.service';
import { AlertifyService } from 'src/app/_services/alertify.service';

@Component({
  selector: 'app-tuition-panel',
  templateUrl: './tuition-panel.component.html',
  styleUrls: ['./tuition-panel.component.scss']
})
export class TuitionPanelComponent implements OnInit {
  teacherTypeId = environment.teacherTypeId;
  parentTypeId = environment.parentTypeId;
  studentTypeId = environment.studentTypeId;
  adminTypeId = environment.adminTypeId;
  settings: Setting[];
  regDate: Date;
  regActive = true;
  nbTuitionPays: number;
  tuitionBalance: string;
  admins: any;
  parents: any;
  teachers: any;
  students: any;
  totalTuitions: any;
  tuitionsNotValidated: any;
  tuitionsValidated: any;
  classSpaces: any;

  constructor(private authService: AuthService, private orderService: OrderService,
    private alertify: AlertifyService, private adminService: AdminService) { }

  ngOnInit() {
    this.getUSersRecap();
    this.getTuitionFigures();
    this.settings = this.authService.settings;
    const regDate = this.settings.find(s => s.name === 'RegistrationDate').value;
    this.regDate = Utils.inputDateDDMMYY(regDate, '/');
    // const today = new Date();
    // if (today >= this.regDate) {
    //   this.regActive = true;
    // }
    // this.nbTuitionPays = Number(this.settings.find(s => s.name === 'NbTuitionPayments').value);
    this.orderService.getBalanceData().subscribe((data: any) => {
      this.tuitionBalance = data.openBalance;
    });
  }

  getUSersRecap() {
    this.adminService.getUsersRecap().subscribe((data: any) => {
      this.admins = data.find(i => i.userTypeId === this.adminTypeId);
      this.parents = data.find(i => i.userTypeId === this.parentTypeId);
      this.teachers = data.find(i => i.userTypeId === this.teacherTypeId);
      this.students = data.find(i => i.userTypeId === this.studentTypeId);
    }, error => {
      this.alertify.error(error);
    });
  }

  getTuitionFigures() {
    this.orderService.getTuitionFigures().subscribe((data: any) => {
      this.totalTuitions = data.totalTuitions;
      this.tuitionsNotValidated = data.tuitionsNotValidated;
      this.tuitionsValidated = data.tuitionsValidated;
      this.classSpaces = data.classSpaces;
    }, error => {
      this.alertify.error(error);
    });
  }

  counter(i: number) {
    return new Array(i);
  }

}
