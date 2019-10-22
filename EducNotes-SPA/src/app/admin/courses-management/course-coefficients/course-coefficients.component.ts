import { Component, OnInit } from '@angular/core';
import { SharedAnimations } from 'src/app/shared/animations/shared-animations';
import { ClassService } from 'src/app/_services/class.service';
import { AlertifyService } from 'src/app/_services/alertify.service';

@Component({
  selector: 'app-course-coefficients',
  templateUrl: './course-coefficients.component.html',
  styleUrls: ['./course-coefficients.component.scss'],
  animations :  [SharedAnimations]

})
export class CourseCoefficientsComponent implements OnInit {
levels: any = [];
coefficients;
levelId: number;
show = false;
  constructor(private classService: ClassService, private alertify: AlertifyService) { }

  ngOnInit() {
    this.getLevels();
  }
  getLevels() {
    this.classService.getLevels().subscribe((res: any[]) => {
      for (let i = 0; i < res.length; i++) {
        const element = {value: res[i].id , label: res[i].name};
        this.levels = [...this.levels, element];
      }
    });
  }

  getLevelCoefficients() {
    this.show = false;
    this.classService.getClasslevelsCoefficients(this.levelId).subscribe((res) => {
      this.coefficients = res;
      this.show = true;
    }, error => {
      this.alertify.error(error);
    });
  }

}
