import { Component, OnInit, ViewChild } from '@angular/core';
import { ClassService } from 'src/app/_services/class.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { IfStmt } from '@angular/compiler';

@Component({
  selector: 'app-new-theme',
  templateUrl: './new-theme.component.html',
  styleUrls: ['./new-theme.component.scss']
})
export class NewThemeComponent implements OnInit {


  /*Besoin fonctionnel :
  -liste de tous les cours
  -liste des niveaux de classe
  */
  levels: any = [];
  courses: any = [];
  theme: any = {};
  lesson: any = {};
  showDetails = false;
  waitDiv = false;

  @ViewChild('content', { static: true }) public contentModal;
  constructor(private classService: ClassService, private alertify: AlertifyService) { }

  ngOnInit() {
    this.getLevels();
    this.getCourses();
    this.theme.lessons = [];
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

  show() {
    this.showDetails = false;
    this.lesson = {};
    this.lesson.contents = [];
    for (let i = 0; i < 10; i++) {
      this.lesson.contents[i] = {};
      this.lesson.contents[i].dsplSeq = i + 1;
      this.lesson.contents[i].title = '';
    }
    this.contentModal.show();
  }

  add() {
    const chaps = this.lesson.contents.filter(t => t.nbHours != null);
    this.lesson.contents = chaps;
    this.lesson.nbHours = 0;
    for (let i = 0; i < chaps.length; i++) {
      this.lesson.nbHours = this.lesson.nbHours + Number(chaps[i].nbHours);
    }
    this.theme.lessons = [...this.theme.lessons, this.lesson];

    this.contentModal.hide();
  }
  detail(lesson) {
    this.showDetails = true;
    this.lesson = lesson;
    this.contentModal.show();

  }



  delete(index) {
    if (confirm('voulez-vous vraiment supprimer cet élement ?')) {
      let less = [];
      less = this.theme.lessons;
      less.splice(index, 1);
      this.theme.lessons = less;
    }
  }

  saveAll() {
    this.waitDiv = true;
    for (let i = 0; i < this.theme.lessons.length; i++) {
      const element = this.theme.lessons[i];
      element.dsplSeq = i + 1;
    }
    this.classService.saveNewTheme(this.theme).subscribe(() => {
      this.alertify.success('enregistrement terminé...');
      this.waitDiv = false;
      this.theme = {};
      this.theme.lessons = [];
    }, error => {
      console.log(error);
    });
  }

  ConfirmTosaveAll() {

  }
}
