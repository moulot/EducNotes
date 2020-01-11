import { Component, OnInit } from '@angular/core';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { CommService } from 'src/app/_services/comm.service';
import { SmsTemplate } from 'src/app/_models/smsTemplate';

@Component({
  selector: 'app-sms-template',
  templateUrl: './sms-template.component.html',
  styleUrls: ['./sms-template.component.scss']
})
export class SmsTemplateComponent implements OnInit {
  templateForm: FormGroup;
  optionsSmsCategory = [];
  smsCategories: any;

  constructor(private fb: FormBuilder, private alertify: AlertifyService, private commService: CommService) { }

  ngOnInit() {
    this.createTemplateForm();
  }

  createTemplateForm() {

    this.templateForm = this.fb.group({
      name: ['', Validators.required],
      categoryId: ['', Validators.required],
      content: ['', Validators.required]
    });
  }

  getCategories() {
    this.commService.getSmsCategories().subscribe((data: any) => {
      for (let i = 0; i < data.length; i++) {
        const elt = data[i];
        const element = {value: elt.id, label: elt.name};
        this.optionsSmsCategory = [...this.optionsSmsCategory, element];
      }
    }, error => {
      this.alertify.error(error);
    });
  }

  saveTemplate() {
    const templateData = <SmsTemplate>{};
    templateData.id = 0;
    templateData.name = this.templateForm.value.name;
    templateData.smsCategoryId = this.templateForm.value.categoryId;
    templateData.content = this.templateForm.value.content;
    this.commService.addSmsTemplate(templateData).subscribe(() => {
      this.alertify.success('le modèle de sms a bien été enregistré.');
    }, error => {
      this.alertify.error(error);
    });
  }

  cancelForm() {
    this.templateForm.reset();
  }
}
