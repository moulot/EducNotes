import { Component, OnInit } from '@angular/core';
import { AdminService } from 'src/app/_services/admin.service';
import { environment } from 'src/environments/environment';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { Subject, Observable } from 'rxjs';
import { startWith, map } from 'rxjs/operators';
import { UserService } from 'src/app/_services/user.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { Router } from '@angular/router';
import { UserFileData } from 'src/app/_models/userFileData';

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
  searchForm: FormGroup;
  searchText = new Subject();
  results: Observable<any[]>;
  data: any = [];

  constructor(private adminService: AdminService, private fb: FormBuilder, private router: Router,
    private userService: UserService, private alertify: AlertifyService) { }

  ngOnInit() {
    this.createSearchForm();
    this.getUSersRecap();
    this.loadStudentData();

    this.results = this.searchForm.controls.searchData.valueChanges
      .pipe(
        startWith(''),
        map((value: string) => this.filter(value))
      );
  }

  filter(value: any): any[] | undefined {
    const filterValue = value.toLowerCase();
    if (this.data) {
      return this.data.filter((item: any) =>
        item.lastName.toLowerCase().includes(filterValue) ||
        item.firstName.toLowerCase().includes(filterValue) ||
        item.idNum.toLowerCase().includes(filterValue));
    }
  }

  loadStudentData() {
    this.userService.loadStudentData().subscribe((data: any) => {
      this.data = data;
    }, error => {
      this.alertify.error(error);
    });
  }

  createSearchForm() {
    this.searchForm = this.fb.group({
      searchData: ['', Validators.required],
      userid: ['', Validators.required]
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

  loadUserFile() {
    const userid = this.searchForm.value.userid;
    const searchData = this.searchForm.value.searchData;
    const userFileData = <UserFileData>{};
    userFileData.userId = userid;
    userFileData.searchData = searchData;
    if (userid) {
      this.userService.loadUserFile(userFileData).subscribe((userData: any) => {
        this.router.navigate(['userFile', userData.id]);
      }, error => {
        this.alertify.error(error);
      });
    } else {
      if (searchData !== '') {

      }
    }
  }

  selectUserId(userid) {
    this.searchForm.patchValue({userid: userid});
  }

}
