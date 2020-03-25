import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { CommService } from 'src/app/_services/comm.service';
import { Utils } from 'src/app/shared/utils';

@Component({
  selector: 'app-email-template',
  templateUrl: './email-template.component.html',
  styleUrls: ['./email-template.component.scss']
})
export class EmailTemplateComponent implements OnInit {
  emailTemplates: any;
  HeadElts = ['action', 'name', 'catégorie', 'message'];

  constructor(private route: ActivatedRoute, private alertify: AlertifyService,
    private commService: CommService, private router: Router) { }

  ngOnInit() {
    this.route.data.subscribe(data => {
      this.emailTemplates = data['templates'];
    });
    Utils.smoothScrollToTop();
  }

  addNew() {
    this.router.navigate(['/AddEmailTemplate']);
  }

  editTemplate(id) {
    this.router.navigate(['/EditEmailTemplate', id]);
  }

}
