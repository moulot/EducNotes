import { Component, OnInit, ViewChild, HostListener, AfterViewInit, ChangeDetectorRef } from '@angular/core';
import { MdbTableDirective } from 'ng-uikit-pro-standard';
import { UserService } from 'src/app/_services/user.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { User } from 'src/app/_models/user';
import { ActivatedRoute } from '@angular/router';
import { AuthService } from 'src/app/_services/auth.service';

@Component({
  selector: 'app-student-life',
  templateUrl: './student-life.component.html',
  styleUrls: ['./student-life.component.scss']
})
export class StudentLifeComponent implements OnInit, AfterViewInit {
  // @ViewChild(MdbTablePaginationComponent, { static: true }) mdbTablePagination: MdbTablePaginationComponent;
  @ViewChild(MdbTableDirective, { static: true }) mdbTable: MdbTableDirective;
  // @ViewChild(MdbTablePaginationComponent, { static: true }) mdbTablePagination1: MdbTablePaginationComponent;
  @ViewChild(MdbTableDirective, { static: true }) mdbTable1: MdbTableDirective;
  student: User;
  absences: any;
  nbAbsences = 0;
  sanctions: any;
  nbSanctions = 0;
  absenceHeadElts = ['type', 'absence', 'justifié', 'motif', 'commentaire'];
  sanctionHeadElts = ['date', 'sanction', 'motif', 'sanctionné par', 'commentaire'];
  showChildrenList = false;
  children: User[];
  isParentConnected = false;
  url = '/studentLifeP';
  parent: User;
  userIdFromRoute: any;
  searchText = '';
  searchText1 = '';
  previous: string;
  previous1: string;

  constructor(private userService: UserService, private alertify: AlertifyService,
    private cdRef: ChangeDetectorRef, private route: ActivatedRoute, private authService: AuthService) { }

  @HostListener('input') oninput() {
    this.searchItems();
  }

  ngOnInit() {

    // this.route.data.subscribe(data => {
    //   this.student = data['student'];
    //   this.getLifeData(this.student.id);
    // });

    this.route.params.subscribe(params => {
      this.userIdFromRoute = params['id'];
    });

    // is the parent connected?
    if (Number(this.userIdFromRoute) === 0) {
      this.showChildrenList = true;
      this.parent = this.authService.currentUser;
      this.getChildren(this.parent.id);
    } else {
      this.showChildrenList = false;
      this.getUser(this.userIdFromRoute);
    }

    this.mdbTable.setDataSource(this.absences);
    this.previous = this.mdbTable.getDataSource();

    this.mdbTable1.setDataSource(this.sanctions);
    this.previous1 = this.mdbTable1.getDataSource();
  }

  ngAfterViewInit() {
    // this.mdbTablePagination.setMaxVisibleItemsNumberTo(5);
    // this.mdbTablePagination.calculateFirstItemIndex();
    // this.mdbTablePagination.calculateLastItemIndex();
    // this.cdRef.detectChanges();

    // this.mdbTablePagination1.setMaxVisibleItemsNumberTo(5);
    // this.mdbTablePagination1.calculateFirstItemIndex();
    // this.mdbTablePagination1.calculateLastItemIndex();
    // this.cdRef.detectChanges();
  }

  getChildren(parentId: number) {
    this.userService.getChildren(parentId).subscribe((users: User[]) => {
      this.children = users;
    }, error => {
      this.alertify.error(error);
    });
  }

  getUser(id) {
    this.userService.getUser(id).subscribe((user: User) => {
      this.student = user;

      const loggedUser = this.authService.currentUser;
      if (loggedUser.id !== this.student.id) {
        this.isParentConnected = true;
      }

      this.getLifeData(this.student.id);
      this.showChildrenList = false;
    }, error => {
      this.alertify.error(error);
    });
  }

  getLifeData(userId) {
    this.userService.getStudentLifeData(userId).subscribe((data: any) => {
      this.absences = data.absences;
      this.nbAbsences = this.absences.length;
      this.sanctions = data.sanctions;
      this.nbSanctions = this.sanctions.length;
    });
  }

  searchItems() {
    const prev = this.mdbTable.getDataSource();

    if (!this.searchText) {
      this.mdbTable.setDataSource(this.previous);
      this.absences = this.mdbTable.getDataSource();
    }

    if (this.searchText) {
      this.absences = this.mdbTable.searchLocalDataBy(this.searchText);
      this.mdbTable.setDataSource(prev);
    }
  }

  searchItems1() {
    const prev = this.mdbTable1.getDataSource();

    if (!this.searchText1) {
      this.mdbTable1.setDataSource(this.previous1);
      this.sanctions = this.mdbTable1.getDataSource();
    }

    if (this.searchText1) {
      this.sanctions = this.mdbTable1.searchLocalDataBy(this.searchText1);
      this.mdbTable1.setDataSource(prev);
    }
  }

}
