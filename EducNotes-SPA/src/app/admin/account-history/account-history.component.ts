import { Component, OnInit } from '@angular/core';
import { AdminService } from 'src/app/_services/admin.service';

@Component({
  selector: 'app-account-history',
  templateUrl: './account-history.component.html',
  styleUrls: ['./account-history.component.scss']
})
export class AccountHistoryComponent implements OnInit {
  lastActivated;
  lastAdded;

  constructor(private adminService: AdminService) { }

  ngOnInit() {
    this.getLastAdded();
    this.getLastActivated();
  }
  getLastAdded() {
    this.adminService.getLastAdded().subscribe((res) => {
      this.lastAdded = res;
    }, error => {
      console.log(error);
    });
  }

  getLastActivated() {
    this.adminService.getLastActivated().subscribe((res) => {
      this.lastActivated = res;
    }, error => {
      console.log(error);
    });
  }


}
