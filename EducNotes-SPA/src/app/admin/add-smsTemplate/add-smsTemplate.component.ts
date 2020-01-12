import { Component, OnInit } from '@angular/core';
import { SmsTemplate } from 'src/app/_models/smsTemplate';
import { Validators, FormBuilder, FormGroup } from '@angular/forms';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { CommService } from 'src/app/_services/comm.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-add-smsTemplate',
  templateUrl: './add-smsTemplate.component.html',
  styleUrls: ['./add-smsTemplate.component.scss']
})
export class AddSmsTemplateComponent implements OnInit {
  templateForm: FormGroup;
  optionsSmsCategory = [];
  smsCategories: any;

  constructor(private fb: FormBuilder, private alertify: AlertifyService,
    private commService: CommService, private router: Router) { }

  ngOnInit() {
    this.createTemplateForm();
    this.getCategories();
  }

  createTemplateForm() {

    this.templateForm = this.fb.group({
      name: ['', Validators.required],
      category: ['', Validators.required],
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
    templateData.smsCategoryId = this.templateForm.value.category;
    templateData.content = this.templateForm.value.content;
    this.commService.saveSmsTemplate(templateData).subscribe(() => {
      this.alertify.success('le modèle de sms a bien été enregistré.');
      this.router.navigate(['/SmsTemplates']);
    }, error => {
      this.alertify.error(error);
    });
  }

  cancelForm() {
    this.templateForm.reset();
  }
}
