import { Component, OnInit } from '@angular/core';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { ActivatedRoute } from '@angular/router';
import { UserService } from 'src/app/_services/user.service';
import { Utils } from 'src/app/shared/utils';

@Component({
  selector: 'app-user-account',
  templateUrl: './user-account.component.html',
  styleUrls: ['./user-account.component.scss']
})
export class UserAccountComponent implements OnInit {
  user: any;
  selectedSms = [];
  smsChoiceChanged = false;

  constructor(private alertify: AlertifyService, private route: ActivatedRoute,
    private userService: UserService) { }

  ngOnInit() {
    this.route.data.subscribe((data: any) => {
      this.user = data['user'];
      for (let i = 0; i < this.user.userSms.length; i++) {
        const elt = this.user.userSms[i];
        this.selectedSms = [...this.selectedSms, elt.smsTemplateId];
      }
    });
  }

  setSmsChoice(smsId: number, active: boolean) {
    this.smsChoiceChanged = true;
    const smsFound = this.selectedSms.findIndex((value) => value === smsId);
    if (smsFound === -1) {
      if (active === true) {
        this.selectedSms = [...this.selectedSms, smsId];
      }
    } else {
      if (active === false) {
        this.selectedSms.splice(smsFound, 1);
      }
    }
  }

  saveUserSMS() {
    this.userService.saveUserSms(this.user.id, this.selectedSms).subscribe(() => {
      Utils.smoothScrollToTop();
      this.smsChoiceChanged = false;
      this.alertify.success('le choix des sms est validé');
    }, error => {
      this.alertify.error(error);
    });
  }

}
