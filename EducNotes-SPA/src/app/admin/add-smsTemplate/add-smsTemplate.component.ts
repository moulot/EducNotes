import { Component, OnInit } from '@angular/core';
import { SmsTemplate } from 'src/app/_models/smsTemplate';
import { Validators, FormBuilder, FormGroup } from '@angular/forms';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { CommService } from 'src/app/_services/comm.service';
import { Router, ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-add-smsTemplate',
  templateUrl: './add-smsTemplate.component.html',
  styleUrls: ['./add-smsTemplate.component.scss']
})
export class AddSmsTemplateComponent implements OnInit {
  templateForm: FormGroup;
  optionsSmsCategory = [];
  smsCategories: any;
  template: SmsTemplate;
  templateId = 0;
  name: string;
  content: string;
  categoryId: number;
  tokens: any;

  constructor(private fb: FormBuilder, private alertify: AlertifyService,
    private commService: CommService, private router: Router, private route: ActivatedRoute) { }

  ngOnInit() {
    this.route.data.subscribe(data => {
      this.template = data['template'];
      if (this.template) {
        this.templateId = this.template.id;
        this.name = this.template.name;
        this.content = this.template.content;
        this.categoryId = this.template.smsCategoryId;
      }
    });
    this.createTemplateForm();
    this.getCategories();
    this.getTokens();

  }

  createTemplateForm() {

    this.templateForm = this.fb.group({
      name: [this.name, Validators.required],
      category: [this.categoryId, Validators.required],
      content: [this.content, Validators.required]
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

  getTokens() {
    this.commService.getTokens().subscribe(tokens => {
      this.tokens = tokens;
    }, error => {
      this.alertify.error(error);
    });
  }

  saveTemplate() {
    const templateData = <SmsTemplate>{};
    templateData.id = this.templateId;
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
    this.router.navigate(['/SmsTemplates']);
  }
}
