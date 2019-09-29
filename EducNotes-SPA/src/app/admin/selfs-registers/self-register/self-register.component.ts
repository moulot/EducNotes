import { Component, OnInit } from '@angular/core';
import { AuthService } from 'src/app/shared/services/auth.service';
import { FormBuilder } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { UserService } from 'src/app/_services/user.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { User } from 'src/app/_models/user';
import { SharedAnimations } from 'src/app/shared/animations/shared-animations';
import { environment } from 'src/environments/environment';


@Component({
  selector: 'app-self-register',
  templateUrl: './self-register.component.html',
  styleUrls: ['./self-register.component.scss'],
  animations: [SharedAnimations]
})
export class SelfRegisterComponent implements OnInit {

  user: User;
  maxChild: number;
  parentTypeId = environment.parentTypeId;
  teacherTypeId = environment.teacherTypeId;


  constructor(private authService: AuthService, private fb: FormBuilder, private route: ActivatedRoute,
     private alertify: AlertifyService,  private router: Router, private userService: UserService) { }

  ngOnInit() {

    this.route.data.subscribe(data => {
       this.user = data['user'].user;
       this.maxChild = data['user'].maxChild;
      //  this.maxChild = data['user'].maxChild;
      });
   }

 
}
