import { Component, OnInit } from '@angular/core';
import { AuthService } from 'src/app/_services/auth.service';
import { User } from 'src/app/_models/user';

@Component({
  selector: 'app-firststep-finishment',
  templateUrl: './firstStep-finishment.component.html',
  styleUrls: ['./firstStep-finishment.component.css']
})
export class FirstStepFinishmentComponent implements OnInit {
user: User;
  constructor(private authService: AuthService) { }

  ngOnInit() {
    this.user = this.authService.newUser;

  }

}
