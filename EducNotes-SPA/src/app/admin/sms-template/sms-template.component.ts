import { Component, OnInit } from '@angular/core';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { CommService } from 'src/app/_services/comm.service';
import { SmsTemplate } from 'src/app/_models/smsTemplate';
import { Router, ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-sms-template',
  templateUrl: './sms-template.component.html',
  styleUrls: ['./sms-template.component.scss']
})
export class SmsTemplateComponent implements OnInit {
  smsTemplates: any;

  constructor(private route: ActivatedRoute, private alertify: AlertifyService,
    private commService: CommService, private router: Router) { }

  ngOnInit() {
    this.route.data.subscribe(data => {
      this.smsTemplates = data['templates'];
      console.log(this.smsTemplates);
    });
  }

  addNew() {
    this.router.navigate(['/AddSmsTemplate']);
  }

}
