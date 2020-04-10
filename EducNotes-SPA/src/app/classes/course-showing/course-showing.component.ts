import { Component, OnInit } from '@angular/core';
import { ClassService } from 'src/app/_services/class.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { UploadFile, UploadInput, UploadOutput } from 'ng-uikit-pro-standard';
import { Theme } from 'src/app/_models/theme';
import { Lesson } from 'src/app/_models/lesson';

@Component({
  selector: 'app-course-showing',
  templateUrl: './course-showing.component.html',
  styleUrls: ['./course-showing.component.scss']
})
export class CourseShowingComponent implements OnInit {
  levels: any = [];
  type: string;
  courses: any = [];
  // they can be a simple lesson whitout theme
  themesSelect: any = [];
  lessonsSelect: any = [];
  searchForm: FormGroup;
  noResult = '';
  courseComment: string;
  searchDiv = false;
  mainVideo: File;
  mainPdf: File;
  otherFiles: File[];
  otherFilesName : string[];


  constructor(private fb: FormBuilder, private classService: ClassService, private alertify: AlertifyService) { }

  ngOnInit() {
    this.getLevels();
    this.getCourses();
    this.createThemeForm();
  }

  createThemeForm() {
    this.searchForm = this.fb.group({
      courseId: [0],
      classLevelId: [0],
      lessonContentIds: [null, Validators.required]
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

    const params = this.searchForm.value;
    if (params.classLevelId && params.courseId) {
      this.searchDiv = true;
      this.noResult = '';
      this.classService.searchThemes(params.classLevelId, params.courseId).subscribe((res: any) => {
        if (res) {
          if (res.type === 'byTheme') {
            this.type = 'byTheme';
            this.makeThemesSelectable(res.themes);
          } else if (res.type === 'byLesson') {
            this.type = 'byLesson';
            this.makeLessonSelectable(res.lessons);
          }
        } else {
          this.noResult = 'Aucun résultat trouvé...';
        }
      }, error => {
        console.log(error);
      });

    }

  }

  makeLessonSelectable(lessons: Lesson[]) {
    // debugger;
    this.themesSelect = [];
    this.lessonsSelect = [];
    for (let i = 0; i < lessons.length; i++) {
      const currentLesson = lessons[i];
      const element = { value: currentLesson.id, label: currentLesson.name, group: true };
      this.lessonsSelect = [...this.lessonsSelect, element];
      for (let j = 0; j < currentLesson.lessonContents.length; j++) {
        const content = currentLesson.lessonContents[j];
        const elt = { value: content.id, label: content.name };
        this.lessonsSelect = [...this.lessonsSelect, elt];
      }
    }
  }

  makeThemesSelectable(themes: Theme[]) {
    this.themesSelect = [];
    this.lessonsSelect = [];
    for (let i = 0; i < themes.length; i++) {
      const currentTheme = themes[i];
      const element = { value: currentTheme.id, label: currentTheme.name, group: true };
      this.themesSelect = [...this.themesSelect, element];
      for (let k = 0; k < currentTheme.lessons.length; k++) {
        const currentLesson = currentTheme.lessons[k];
        const el = { value: currentLesson.id, label: currentLesson.name, group: true };
        this.themesSelect = [...this.themesSelect, el];
        for (let j = 0; j < currentLesson.lessonContents.length; j++) {
          const content = currentLesson.lessonContents[j];
          const elt = { value: content.id, label: content.name };
          this.themesSelect = [...this.themesSelect, elt];
        }
      }
    }

  }

  getVideoResult(event) {
    this.mainVideo = null;
    this.mainVideo = <File>event.target.files[0];
  }

  getMainPdfResult(event) {
    this.mainPdf = null;
    this.mainPdf = <File>event.target.files[0];
  }

  getOtherFilesResult(event) {
    this.otherFiles = null;
    this.otherFilesName = [];
    this.otherFiles = event.target.files;
    for (let i = 0; i < this.otherFiles.length; i++) {
      const element = this.otherFiles[i].name;
      this.otherFilesName = [...this.otherFilesName, element];
    }
  }



}
