import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { CommService } from 'src/app/_services/comm.service';
import { Router, ActivatedRoute } from '@angular/router';
import { EmailTemplate } from 'src/app/_models/emailTemplate';

@Component({
  selector: 'app-add-emailTemplate',
  templateUrl: './add-emailTemplate.component.html',
  styleUrls: ['./add-emailTemplate.component.scss']
})
export class AddEmailTemplateComponent implements OnInit {
  templateForm: FormGroup;
  optionsEmailCategory = [];
  emailCategories: any;
  template: EmailTemplate;
  templateId = 0;
  name: string;
  subject: string;
  body: string;
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
        this.subject = this.template.subject;
        this.body = this.template.body;
        this.categoryId = this.template.emailCategoryId;
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
      subject: [this.subject, Validators.required],
      body: [this.body, Validators.required]
    });
  }

  getCategories() {
    this.commService.getEmailCategories().subscribe((data: any) => {
      for (let i = 0; i < data.length; i++) {
        const elt = data[i];
        const element = {value: elt.id, label: elt.name};
        this.optionsEmailCategory = [...this.optionsEmailCategory, element];
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
    const templateData = <EmailTemplate>{};
    templateData.id = this.templateId;
    templateData.name = this.templateForm.value.name;
    templateData.emailCategoryId = this.templateForm.value.category;
    templateData.subject = this.templateForm.value.subject;
    templateData.body = this.templateForm.value.body;
    this.commService.saveEmailTemplate(templateData).subscribe(() => {
      this.alertify.success('le modèle d\'email a bien été enregistré.');
      this.router.navigate(['/EmailTemplates']);
    }, error => {
      this.alertify.error(error);
    });
  }

  cancelForm() {
    this.router.navigate(['/EmailTemplates']);
  }
}
