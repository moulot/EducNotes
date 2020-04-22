import { Component, OnInit, Input } from '@angular/core';
import { SharedAnimations } from '../../animations/shared-animations';
import { UserService } from 'src/app/_services/user.service';
import { AuthService } from 'src/app/_services/auth.service';
import { AlertifyService } from 'src/app/_services/alertify.service';

@Component({
  selector: 'app-timeline',
  templateUrl: './timeline.component.html',
  styleUrls: ['./timeline.component.scss'],
  animations: [SharedAnimations]
})
export class TimelineComponent implements OnInit {
  @Input() events: any;
  @Input()  isParentConnected: boolean;

  constructor(private userService: UserService, private authService: AuthService,
    private alertify: AlertifyService) { }

  ngOnInit() {
  }

}
