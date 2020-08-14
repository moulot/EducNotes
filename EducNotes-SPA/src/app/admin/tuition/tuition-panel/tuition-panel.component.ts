import { Component, OnInit } from '@angular/core';
import { AuthService } from 'src/app/_services/auth.service';
import { Setting } from 'src/app/_models/setting';
import { Utils } from 'src/app/shared/utils';
import { OrderService } from 'src/app/_services/order.service';
import { environment } from 'src/environments/environment';
import { AdminService } from 'src/app/_services/admin.service';

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
  regActive = false;
  nbTuitionPays: number;
  tuitionBalance: string;
  admins: any;
  parents: any;
  teachers: any;
  students: any;

  constructor(private authService: AuthService, private orderService: OrderService, private adminService: AdminService) { }

  ngOnInit() {
    this.getUSersRecap();
    this.settings = this.authService.settings;
    const regDate = this.settings.find(s => s.name === 'RegistrationDate').value;
    this.regDate = Utils.inputDateDDMMYY(regDate, '/');
    const today = new Date();
    if (today >= this.regDate) {
      this.regActive = true;
    }
    this.nbTuitionPays = Number(this.settings.find(s => s.name === 'NbTuitionPayments').value);
    this.orderService.getBalanceData().subscribe((data: any) => {
      this.tuitionBalance = data.openBalance;
    });
  }

  getUSersRecap() {
    this.adminService.getUsersRecap().subscribe((res: any[]) => {
     this.admins = res.find(i => i.userTypeId === this.adminTypeId);
     this.parents = res.find(i => i.userTypeId === this.parentTypeId);
     this.teachers = res.find(i => i.userTypeId === this.teacherTypeId);
     this.students = res.find(i => i.userTypeId === this.studentTypeId);
    }, error => {
      console.log(error);
    });
  }

  counter(i: number) {
    return new Array(i);
  }

}
