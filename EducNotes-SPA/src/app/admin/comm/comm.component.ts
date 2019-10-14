import { Component, OnInit } from '@angular/core';
import { SharedAnimations } from 'src/app/shared/animations/shared-animations';
import { ClassService } from 'src/app/_services/class.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { ClassLevel } from 'src/app/_models/classLevel';
import { FormControl } from '@angular/forms';
import { AdminService } from 'src/app/_services/admin.service';

@Component({
  selector: 'app-comm',
  templateUrl: './comm.component.html',
  styleUrls: ['./comm.component.scss'],
  animations :  [SharedAnimations]
})
export class CommComponent implements OnInit {
  classLevels: any;
  classes: any;
  userTypes: any;
  userTypeControl = new FormControl();
  classLevelControl = new FormControl();
  classControl = new FormControl();
  classLevelSelected = false;
  userTypeOptions = [];
  classLevelOptions = [];
  classOptions = [];

  constructor(private classService: ClassService, private alertify: AlertifyService,
    private adminService: AdminService) { }


  ngOnInit() {

    this.getUserTypes();
    this.getClassLevels();

  }

  setSenderList() {
    const usertypeSelected = this.userTypeControl.value;
    const classLevelSelected = this.classLevelControl.value;
    const classSelected = this.classControl.value;

    
  }

  getClassLevels() {
    this.classService.getLevels().subscribe((data: any) => {
      this.classLevels = data;
      for (let i = 0; i < data.length; i++) {
        const elt = data[i];
        const option = {value: elt.id, label: elt.name};
        this.classLevelOptions = [...this.classLevelOptions, option];
      }
    }, error => {
      this.alertify.error(error);
    });
  }

  getUserTypes() {
    this.adminService.getUserTypes().subscribe((data: any) => {
      this.userTypes = data;
      for (let i = 0; i < data.length; i++) {
        const elt = data[i];
        const option = {value: elt.id, label: elt.name};
        this.userTypeOptions = [...this.userTypeOptions, option];
      }
    });
  }

  getClasses(event) {
    const clevelId = this.classLevelControl.value;
    console.log(clevelId);
    this.classOptions = [];
    if (clevelId !== '' || clevelId !== 'all') {
      this.classLevelSelected = true;
      this.classService.getClassesByLevelId(clevelId).subscribe((data: any) => {
        this.classes = data;
        for (let i = 0; i < data.length; i++) {
          const elt = data[i];
          const option = {value: elt.id, label: elt.name};
          this.classOptions = [...this.classOptions, option];
          }
      }, error => {
        this.alertify.error(error);
      });
    } else {
      this.classLevelSelected = false;
      this.classControl.reset();
    }
  }

}
