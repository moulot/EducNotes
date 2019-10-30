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
editField: string;
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
    this.coefficients = [];
    this.classService.getClasslevelsCoefficients(this.levelId).subscribe((res) => {
      this.coefficients = res;
      this.show = true;
    }, error => {
      this.alertify.error(error);
    });
  }

  updateList(id: number, property: string, event: any) {
    const editField = event.target.textContent;
    this.coefficients[id][property] = editField;
  }

  remove(id: any) {
    // this.awaitingPersonList.push(this.personList[id]);
    // this.personList.splice(id, 1);
  }

  add() {
    // if (this.awaitingPersonList.length > 0) {
    //   const person = this.awaitingPersonList[0];
    //   this.personList.push(person);
    //   this.awaitingPersonList.splice(0, 1);
    // }
  }

  changeValue(id: number, property: string, event: any) {
    // console.log('id du coefficient: ' + id);
    // debugger;
    const elementId = this.coefficients[id].id;
    this.editField = event.target.textContent;
    // mise a jour de la ligne
    if (this.editField && elementId) {
      this.classService.updateCourseCoefficient(elementId, Number(this.editField)).subscribe(() => {
        this.alertify.success('modificiation terminéé');
      }, error => {
        this.alertify.error(error);
      });
    }
  }

}
