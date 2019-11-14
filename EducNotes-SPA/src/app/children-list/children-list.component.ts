import { Component, OnInit, Input } from '@angular/core';
import { User } from '../_models/user';
import { SharedAnimations } from '../shared/animations/shared-animations';
import { Router } from '@angular/router';
import { AuthService } from '../_services/auth.service';

@Component({
  selector: 'app-children-list',
  templateUrl: './children-list.component.html',
  styleUrls: ['./children-list.component.scss'],
  animations: [SharedAnimations]
})
export class ChildrenListComponent implements OnInit {
  @Input() children: User[];
  @Input() url: string;
  viewMode: 'list' | 'grid' = 'list';
  allSelected: boolean;
  page = 1;
  pageSize = 8;

  constructor(private router: Router, private authService: AuthService) { }

  ngOnInit() {
  }

  goToPage(child) {
    this.authService.changeCurrentChild(child);
    this.router.navigate([this.url, child.id]);
  }

}
