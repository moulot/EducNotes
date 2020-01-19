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
  parent: any;
  selectedSms = [];
  // smsChoiceChanged = false;

  constructor(private alertify: AlertifyService, private route: ActivatedRoute,
    private userService: UserService) { }

  ngOnInit() {
    this.route.data.subscribe((data: any) => {
      this.parent = data['parent'];
      // set initial users sms activated
      for (let i = 0; i < parent.length; i++) {
        const elt = parent[i];
        
      }
    });
  }

  setSmsChoice(data) {
    const childId = data.childId;
    const smsId = data.smsId;
    const active = data.active;
    const smsData = <any>{};
    smsData.childId = childId;
    smsData.smsId = smsId;
    const smsFound = this.selectedSms.findIndex(s => s.smsId === smsId && s.childId === childId);
    if (smsFound === -1) {
      if (active === true) {
        this.selectedSms = [...this.selectedSms, smsData];
      }
    } else {
      if (active === false) {
        this.selectedSms.splice(smsFound , 1);
      }
    }
    // console.log(this.selectedSms);
  }

  saveUserSMS() {
    console.log(this.selectedSms);
    console.log(this.parent.id);
    this.userService.saveUserSms(this.parent.id, this.selectedSms).subscribe(() => {
      Utils.smoothScrollToTop();
      this.alertify.success('le choix des sms est validé');
    }, error => {
      this.alertify.error(error);
    });
  }

}
