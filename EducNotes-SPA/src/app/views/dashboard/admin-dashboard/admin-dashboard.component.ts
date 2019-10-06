import { Component, OnInit } from '@angular/core';
import { AdminService } from 'src/app/_services/admin.service';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-admin-dashboard',
  templateUrl: './admin-dashboard.component.html',
  styleUrls: ['./admin-dashboard.component.scss']
})
export class AdminDashboardComponent implements OnInit {

  teacherTypeId = environment.teacherTypeId;
  parentTypeId = environment.parentTypeId;
  studentTypeId = environment.studentTypeId;
  adminTypeId = environment.adminTypeId;
  admins: any;
  parents: any;
  teachers: any;
  students: any;
  constructor(private adminService: AdminService) { }

  ngOnInit() {
    this.getUSersRecap();
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

}
