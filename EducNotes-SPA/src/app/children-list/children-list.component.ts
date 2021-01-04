import { Component, OnInit, Input, EventEmitter, Output } from '@angular/core';
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
  @Input() children: any[];
  @Output() getUser = new EventEmitter<any>();

  constructor(private router: Router, private authService: AuthService) { }

  ngOnInit() {
  }

  goToChildPage(child) {
    this.authService.changeCurrentChild(child);
    this.getUser.emit(child);
  }

}
