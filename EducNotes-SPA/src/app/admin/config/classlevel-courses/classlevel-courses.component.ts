import { Component, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Utils } from 'src/app/shared/utils';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { ClassService } from 'src/app/_services/class.service';

@Component({
  selector: 'app-classlevel-courses',
  templateUrl: './classlevel-courses.component.html',
  styleUrls: ['./classlevel-courses.component.scss']
})
export class ClasslevelCoursesComponent implements OnInit {
  levelCoursesForm: FormGroup;
  levels: any;
  courses: any;
  levelCourses: any;

  constructor(private fb: FormBuilder, private alertify: AlertifyService,
    private classService: ClassService) { }

  ngOnInit() {
    this.createLevelForm();
    this.getClassLevelCourses();
  }

  createLevelForm() {
    this.levelCoursesForm = this.fb.group({
      levels: this.fb.array([])
    });
  }

  addLevelItems() {
    const levels = this.levelCoursesForm.get('levels') as FormArray;
    this.levels.forEach(x => {
      levels.push(this.fb.group({
        id: x.id,
        name: x.name,
        courses: this.addCourseItems(x.id)
      }));
    });
  }

  addCourseItems(levelid) {
    const arr = new FormArray([]);
    this.courses.forEach(x => {
      let selected = false;
      if (this.levelCourses) {
        selected = this.levelCourses.findIndex(c => c.courseId === x.id && c.classLevelId === levelid) !== -1 ? true : false;
      }
      arr.push(this.fb.group({
        id: x.id,
        name: x.name,
        selected: selected
      }));
    });
    return arr;
  }

  save() {
    let levelcourses = [];
    for (let i = 0; i < this.levelCoursesForm.value.levels.length; i++) {
      const level = this.levelCoursesForm.value.levels[i];
      const item = <any>{};
      item.levelId = level.id;
      item.courses = [];
      for (let j = 0; j < level.courses.length; j++) {
        const elt = level.courses[j];
        if (elt.selected) {
          const course = <any>{};
          course.id = elt.id;
          course.selected = elt.selected;
          item.courses = [...item.courses, course];
        }
      }
      levelcourses = [...levelcourses, item];
    }
    this.classService.saveLevelCourses(levelcourses).subscribe(() => {
      Utils.smoothScrollToTop(40);
      this.alertify.success('les cours ont bien été enregistrés. merci');
    }, error => {
      this.alertify.error(error);
    });
}

  getClassLevels() {
    this.classService.getLevels().subscribe(data => {
      this.levels = data;
      this.getCourses();
      }, error => {
      this.alertify.error(error);
    });
  }

  getCourses() {
    this.classService.getCourses().subscribe(data => {
      this.courses = data;
      this.addLevelItems();
    }, error => {
      this.alertify.error(error);
    });
  }

  getClassLevelCourses() {
    this.classService.getClassLevelCourses().subscribe(data => {
      this.levelCourses = data;
      this.getClassLevels();
    }, error => {
      this.alertify.error(error);
    });
  }

}
