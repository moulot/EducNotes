import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { AlertifyService } from 'src/app/_services/alertify.service';

@Component({
  selector: 'app-parent-file',
  templateUrl: './parent-file.component.html',
  styleUrls: ['./parent-file.component.scss']
})
export class ParentFileComponent implements OnInit {

  constructor(private alertify: AlertifyService,
    private route: ActivatedRoute) { }
  parentFile: any;
  showInfos = [true, true, true];

  ngOnInit() {
    this.route.data.subscribe((data: any) => {
      this.parentFile = data['file'];
    }, error => {
      this.alertify.error(error);
    });
  }

  toggleInfos(index) {
    this.showInfos[index] = !this.showInfos[index];
  }

}
