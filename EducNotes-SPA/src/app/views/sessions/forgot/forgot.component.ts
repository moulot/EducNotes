import { Component, OnInit } from '@angular/core';
import { SharedAnimations } from 'src/app/shared/animations/shared-animations';
import { AuthService } from 'src/app/_services/auth.service';
import { AlertifyService } from 'src/app/_services/alertify.service';

@Component({
  selector: 'app-forgot',
  templateUrl: './forgot.component.html',
  styleUrls: ['./forgot.component.scss'],
  animations: [SharedAnimations]
})
export class ForgotComponent implements OnInit {
  email: string;

  constructor(private authService: AuthService, private alertify: AlertifyService) { }

  ngOnInit() {
  }
  sendLink() {
    this.authService.forgotPassord(this.email).subscribe(res => {
      this.alertify.success('lien envoyé');
    });
  }

}
