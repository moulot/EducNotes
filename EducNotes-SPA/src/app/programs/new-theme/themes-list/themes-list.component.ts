import { Component, OnInit } from '@angular/core';
import { ClassService } from 'src/app/_services/class.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { FormGroup, FormBuilder } from '@angular/forms';
import { LessonContent } from 'src/app/_models/lesson-content';
import { Theme } from 'src/app/_models/theme';

@Component({
  selector: 'app-themes-list',
  templateUrl: './themes-list.component.html',
  styleUrls: ['./themes-list.component.scss']
})
export class ThemesListComponent implements OnInit {
  levels: any = [];
  courses: any = [];
  // they can be a simple lesson whitout theme
  themes: Theme[];
  theme: any = {};
  searchForm: FormGroup;
  noResult = '';
  searchDiv = false;

  constructor(private fb: FormBuilder, private classService: ClassService, private alertify: AlertifyService) { }

  ngOnInit() {
    this.getLevels();
    this.getCourses();
    this.createThemeForm();
  }

  createThemeForm() {
    this.searchForm = this.fb.group({
      courseId: [0],
      classLevelId: [0]
    });
  }

  getLevels() {
    this.classService.getLevels().subscribe((res: any[]) => {

      for (let i = 0; i < res.length; i++) {
        const ele = { value: res[i].id, label: res[i].name };
        this.levels = [...this.levels, ele];
      }
    }, error => {
      console.log(error);
    });
  }
  getCourses() {
    this.classService.getAllCourses().subscribe((res: any[]) => {
      for (let i = 0; i < res.length; i++) {
        const element = { value: res[i].id, label: res[i].name };
        this.courses = [...this.courses, element];
      }
    });
  }
  searchTheme() {
    this.searchDiv = true;
    this.themes = [];
    this.noResult = '';
    const params = this.searchForm.value;
    this.classService.searchThemes(params.classLevelId, params.courseId).subscribe((res: Theme[]) => {
      if (res.length > 0) {
         this.themes = res;
         } else {
           this.noResult = 'aucun enregistrement trouvé...';
         }

      console.log(res);
    }, error => {
      console.log(error);
    });
  }

}
