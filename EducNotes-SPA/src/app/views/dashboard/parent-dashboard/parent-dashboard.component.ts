import { Component, OnInit } from '@angular/core';
import { UserService } from 'src/app/_services/user.service';
import { User } from 'src/app/_models/user';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { SharedAnimations } from 'src/app/shared/animations/shared-animations';
import { AuthService } from 'src/app/_services/auth.service';
import { EvaluationService } from 'src/app/_services/evaluation.service';

@Component({
  selector: 'app-parent-dashboard',
  templateUrl: './parent-dashboard.component.html',
  styleUrls: ['./parent-dashboard.component.scss'],
  animations: [SharedAnimations]
})
export class ParentDashboardComponent implements OnInit {
  parent: User;
  courses: any;
  children: User[];
  currentChild: User;
  userCourses: any;
  studentAvg: any;

  viewMode: 'list' | 'grid' = 'list';
  allSelected: boolean;
  page = 1;
  pageSize = 8;
  products: any[] = [];

  constructor(private userService: UserService, private alertify: AlertifyService,
    private authService: AuthService, private evalService: EvaluationService) { }

  ngOnInit() {
    this.parent = this.authService.currentUser;
    this.getChildren(this.parent.id);
  }

  getChildren(parentId: number) {
    this.userService.getChildren(parentId).subscribe((users: User[]) => {
      this.children = users;
    }, error => {
      this.alertify.error(error);
    });
  }

}
