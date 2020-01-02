import { Component, OnInit } from '@angular/core';
import { CommService } from 'src/app/_services/comm.service';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { Sms } from 'src/app/_models/sms';
import { clickatellParams } from 'src/app/_models/clickatellParams';
import { SmsCallback } from 'src/app/_models/smsCallback';

@Component({
  selector: 'app-sendSms',
  templateUrl: './sendSms.component.html',
  styleUrls: ['./sendSms.component.scss']
})
export class SendSmsComponent implements OnInit {
  smsForm: FormGroup;
  phoneMask = ['+', /\d/, /\d/, /\d/, ' ', /\d/, /\d/, ' ', /\d/, /\d/, ' ', /\d/, /\d/, ' ', /\d/, /\d/];

  constructor(private fb: FormBuilder, private commService: CommService, private alertify: AlertifyService) { }

  ngOnInit() {
    this.createSmsForm();
  }

  createSmsForm() {
    this.smsForm = this.fb.group({
      to: ['', Validators.required],
      body: ['', Validators.required]
    });
  }

  // sendSms() {
  //   const smsData = <Sms>{};
  //   smsData.to = this.smsForm.value.to;
  //   smsData.body = this.smsForm.value.body;
  //   this.commService.sendSms(smsData).subscribe(() => {
  //     this.alertify.success('le sms a bien été envoyé.');
  //   }, error => {
  //     this.alertify.error(error);
  //   });
  // }

  sendSms() {
    const smsData = <clickatellParams>{};
    smsData.to = this.smsForm.value.to;
    smsData.content = this.smsForm.value.body;
    this.commService.sendSms(smsData).subscribe((result: any) => {
      const messages = result.messages;
      console.log(messages);
      this.alertify.success('message(s) bien envoyé(s)!');
    }, error => {
      this.alertify.error(error);
    });
  }

  cancelForm() {
    this.smsForm.reset();
  }
}
