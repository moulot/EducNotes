import { Component, Input, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { ClassService } from 'src/app/_services/class.service';

@Component({
  selector: 'app-child-file-class-life',
  templateUrl: './child-file-class-life.component.html',
  styleUrls: ['./child-file-class-life.component.scss']
})
export class ChildFileClassLifeComponent implements OnInit {
  @Input() id: any;
  showInfos = true;
  userLife: any;

  constructor(private classService: ClassService, private route: ActivatedRoute,
    private router: Router, private alertify: AlertifyService) { }

  ngOnInit() {
    this.getChildLife(this.id);
  }

  getChildLife(childId) {
    this.classService.getUserClassLife(childId).subscribe(data => {
      this.userLife = data;
    }, error => {
      this.alertify.error(error);
    });
  }

  toggleInfos() {
    this.showInfos = !this.showInfos;
  }
}
