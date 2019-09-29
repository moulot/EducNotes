import { Component, OnInit } from '@angular/core';
import { FormGroup,  FormBuilder, Validators } from '@angular/forms';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { ClassService } from 'src/app/_services/class.service';

@Component({
  selector: 'app-cours-classes-management',
  templateUrl: './cours-classes-management.component.html',
  styleUrls: ['./cours-classes-management.component.css']
})
export class CoursClassesManagementComponent implements OnInit {
  classForm: FormGroup;
   editClassForm: FormGroup;
   courseForm: FormGroup;
  selectedClass: any = {};
  levels: any;
  courses: any;
  coursesTeachers: any;
  teacherDisplay: any = [];
  allCourses: any;
  levelsForSearch: any;
 suffixes = [{id: 1, name: 'A,B,C,...'}, {id: 2, name: '1,2,3,.....' }];
  classes: any;
  levelId: number;
  isVisible = false;
  showSearch = false;
  noResult = '';
  teacherId: any;
  visible = false;
  drawerTitle = '';
  modalTitle = '';
  classAdding = false;
  errorMessage = '';
  classEditing = false;
 submitText = 'enregistrer';
 coursName = '';
 courseId: number;
 teacher: any;

  // classAdding = false;
  // classAdding = false;
  values: any[] = null;


  constructor(private fb: FormBuilder, private classService: ClassService, private alertify: AlertifyService) { }

  ngOnInit() {
    this.classService.getCoursesTeachers().subscribe((response: any) => {
      this.coursesTeachers = response;

    });
    this.classService.getAllCoursesDetails().subscribe((response: any) => {
      this.allCourses = response;
    });
    this.createCourseForm();
    this.getLevels();
    this.getCourses();
    this.createClassForm();

  }

  createClassForm() {
    this.classForm = this.fb.group({
      levelId: [null, Validators.required],
       name: [, Validators.nullValidator],
      suffixe: [null, Validators.nullValidator],
      number: [null, Validators.nullValidator],
      courseIds: [null, Validators.required]});
  }

  createCourseForm() {
    this.courseForm = this.fb.group({
       name: ['', Validators.required]});
  }

  createEditClassForm(val: any) {
    this.editClassForm = this.fb.group({
      classLevelId: [val.class.classLevelId, Validators.required],
       name: [val.class.name, Validators.nullValidator],
       courseIds: [val.courseIds, Validators.required]});
  }

  getLevels() {
    this.classService.getLevels().subscribe( (response: any) => {
      this.levels = response;
      this.levelsForSearch = response;
    }, error => {
      console.log(error);
    });
  }

  getCourses() {
    this.classService.getAllCourses().subscribe((response: any) => {
      this.courses = response;
    });
  }

  searchClasses() {
    this.noResult = '';
  this.showSearch = true;
  this.classes = [];
    this.classService.getClassesByLevelId(this.levelId).subscribe((response: any) => {
      if (response.length === 0) {
        this.noResult = 'Aucune classe trouvée...';
      } else {
        this.classes = response;
          }
    }, error => {
      console.log(error);
    });
  }

  addClass() {
    this.drawerTitle = 'ENREGISTREMENT CLASSE';
    this.visible = true;
    this.classAdding = true;
    this.classEditing = false;
    // ajout de la class
  }

  close(): void {
    this.visible = false;
  }

  saveClass() {
    this.errorMessage = '';
    const classFromForm =  Object.assign({}, this.classForm.value);
    if (classFromForm.suffixe) {
      if (!classFromForm.nombre) { this.errorMessage = 'veuillez saisir le nombre de classe'; }

    } else if (!classFromForm.name) {
      this.errorMessage = 'veuillez saisir le nom de la classe';
    } else {
    this.submitText = 'patienter...';
    this.classService.saveNewClasses(classFromForm).subscribe(next => {
        this.visible = false;
        this.submitText = 'enregistrer';
        this.alertify.success('enregistrement terminé...');
        this.createClassForm();

      }, error => {
        console.log(error);
        this.submitText = 'enregistrer';
        this.errorMessage = error;
      });
    }

  }

  modifierClass(element: any ) {
    this.drawerTitle = 'MODIFICATION CLASSE';
    this.selectedClass = element;
     element.courseIds = [];
    for (let i = 0; i < element.courses.length; i++) {
      element.courseIds = [...element.courseIds, element.courses[i].id];
    }
    this.createEditClassForm(element);
    this.visible = true;
        this.classEditing = true;
        this.classAdding = false;

  }

  modifierCours(coursId: number ) {
    this.courseId = coursId;
    const itemIndex = this.allCourses.findIndex(item => item.course.id === coursId);
    this.cancelCourseEditing();
    this.coursName =  this.allCourses[itemIndex].course.name;
    this.allCourses[itemIndex].visible = true;

  }

  cancelCourseEditing() {
    this.coursName = '';
    for (let i = 0; i < this.allCourses.length; i++) {
      const element = this.allCourses[i].visible = false;
    }
  }

  saveCourseEdition() {
    this.alertify.confirm('confirmez-vous cette modification ?', () => {
      console.log(this.coursName);
      console.log(this.courseId);
          this.classService.updateCourse(this.courseId, this.coursName).subscribe((response: any) => {
            const itemIndex = this.allCourses.findIndex(item => item.course.id === this.courseId);
           this.allCourses[itemIndex].course.name = this.coursName;
            this.cancelCourseEditing();
            this.alertify.success('modfification éffectuée...');
        this.visible = false;

      }, error => {
        console.log(error);
      });

      });
  }

  deleteClass(id: number) {
    this.alertify.confirm('confirmez-vous cette suppression ?', () => {
      this.classService.deleteClass(id).subscribe((response: any) => {
            this.classes.splice(this.classes.findIndex(p => p.class.id === id), 1);
            this.alertify.success('classe supprimée...');
        this.visible = false;

      }, error => {
        console.log(error);
      });

  });
  }

  saveClassChange() {
          this.alertify.confirm('Enregistrer ces modifications ?', () => {
            this.selectedClass.class.name = this.editClassForm.value.name;
            this.selectedClass.class.classLevelId = this.editClassForm.value.classLevelId;
            this.selectedClass.courseIds = [];
            this.selectedClass.class.courseIds = this.editClassForm.value.courseIds;

        const data: any = {
          class : this.selectedClass.class,
          courseIds : this.editClassForm.value.courseIds
        };
        console.log(data);
        this.classService.saveClassModification(this.selectedClass.class.id, data).subscribe((response: any) => {
          this.searchClasses();
          this.alertify.success('modification enregistrée...');
          this.visible = false;

        }, error => {
          console.log(error);
        });

        });
  }

  ppseting(classId: number, teacherId: number) {
    if (teacherId !== null) {
      this.teacherId = teacherId;
    } else {
      this.teacherId = null;
    }

  }

  addCourse() {
    this.createCourseForm();
    this.modalTitle = 'AJOUTER COURS';
    this.isVisible = true;

  }

  handleOk(): void {
    // enregistrement
    const dataFromForm = Object.assign(this.courseForm.value);
    console.log(this.courseForm.value.name);
    this.isVisible = false;
  }

  handleCancel(): void {
    this.isVisible = false;
  }

  saveNewCourse() {
    this.alertify.confirm('voulez-vous vraiment ajouter ce cours ?', () => {
      this.classService.addNewCourse(this.courseForm.value.name).subscribe((response: any) => {
         response.type = 'new';
         this.allCourses = [...this.allCourses, response];
         // console.log(response);
          this.createCourseForm();
            this.alertify.success('cours enregistrée...');

      }, error => {
        console.log(error);
      });

  });

  }

}
