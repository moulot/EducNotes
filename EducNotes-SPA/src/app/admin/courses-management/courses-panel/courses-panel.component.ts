import { Component, OnInit } from '@angular/core';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { ActivatedRoute, Router } from '@angular/router';
import { AdminService } from 'src/app/_services/admin.service';
import { SharedAnimations } from 'src/app/shared/animations/shared-animations';
import { ClassService } from 'src/app/_services/class.service';
import { Course } from 'src/app/_models/course';


@Component({
  selector: 'app-courses-panel',
  templateUrl: './courses-panel.component.html',
  styleUrls: ['./courses-panel.component.scss'],
  animations :  [SharedAnimations]
})
export class CoursesPanelComponent implements OnInit {
  constructor(private adminService: AdminService, private route: ActivatedRoute,
    private router: Router, private alertify: AlertifyService, private classService: ClassService) {}

 courses: any[];
 course: Course;
 show = 'all';
 willDownload = false;

  ngOnInit() {
    this.route.data.subscribe(data => {
      this.courses = data.courses;
    });
  }

  getCourses() {
    this.courses = [];
    this.classService.getAllCoursesDetails().subscribe((res: any[]) => {
      this.courses = res;
    });
  }

  saveCourse(data) {
    const course = {
      id: data.id,
      name: data.name,
      abbrev: data.abbrev,
      color: data.color
    };
    this.classService.addCourse(course).subscribe(() => {
      this.alertify.success('le cours a été bien enregistré...');
      const index = this.courses.findIndex(c => c.id === course.id);
      this.courses[index].name = course.name;
      this.courses[index].abbreviation = course.abbrev;
      this.courses[index].color = course.color;
    }, error => {
      this.alertify.error(error);
    });
  }

}
