import { Component, OnInit } from '@angular/core';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { ActivatedRoute, Router } from '@angular/router';
import { AdminService } from 'src/app/_services/admin.service';
import { SharedAnimations } from 'src/app/shared/animations/shared-animations';

@Component({
  selector: 'app-courses-panel',
  templateUrl: './courses-panel.component.html',
  styleUrls: ['./courses-panel.component.scss'],
  animations :  [SharedAnimations]
})
export class CoursesPanelComponent implements OnInit {
  constructor(private adminService: AdminService, private route: ActivatedRoute,
    private router: Router, private alertify: AlertifyService) {}

 courses: any;
 addNew = false;
ngOnInit() {
    this.route.data.subscribe(data => {
       this.courses = data.courses;
    });
    }
    newCourse() {
      this.addNew = !this.addNew;
    }

    resultMode(val: boolean) {
      if (val) {
        this.router.navigateByUrl('/CoursesPanelComponent', {skipLocationChange: true}).then(() =>
         this.router.navigate(['/courses']));
      } else {
        this.addNew = !this.addNew;
      }
    }

}
