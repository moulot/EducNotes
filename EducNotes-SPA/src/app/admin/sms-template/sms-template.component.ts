import { Component, OnInit } from '@angular/core';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { CommService } from 'src/app/_services/comm.service';
import { Router, ActivatedRoute } from '@angular/router';
import { Utils } from 'src/app/shared/utils';

@Component({
  selector: 'app-sms-template',
  templateUrl: './sms-template.component.html',
  styleUrls: ['./sms-template.component.scss']
})
export class SmsTemplateComponent implements OnInit {
  smsTemplates: any;
  HeadElts = ['action', 'name', 'catégorie', 'message'];

  constructor(private route: ActivatedRoute, private alertify: AlertifyService,
    private commService: CommService, private router: Router) { }

  ngOnInit() {
    this.route.data.subscribe(data => {
      this.smsTemplates = data['templates'];
    });
    Utils.smoothScrollToTop(10);
  }

  addNew() {
    this.router.navigate(['/AddSmsTemplate']);
  }

  editSmsTemplate(id) {
    this.router.navigate(['/EditSmsTemplate', id]);
  }

}
