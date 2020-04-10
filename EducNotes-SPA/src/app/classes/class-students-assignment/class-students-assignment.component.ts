import { Component, OnInit } from '@angular/core';
import * as XLSX from 'xlsx';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ClassService } from 'src/app/_services/class.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { environment } from 'src/environments/environment';
import { Class } from 'src/app/_models/class';


@Component({
  selector: 'app-class-students-assignment',
  templateUrl: './class-students-assignment.component.html',
  styleUrls: ['./class-students-assignment.component.scss']
})
export class ClassStudentsAssignmentComponent implements OnInit {
  levels: any = [];
  classes: any = [];
  searchForm: FormGroup;

  importedStudents: any = [];
  showImport = false;
  studentTypeId = environment.studentTypeId;
  parentTypeId = environment.parentTypeId;
  className: string;
  allSelected = false;
  waitDiv = false;

  constructor(private fb: FormBuilder, private classService: ClassService, private alertify: AlertifyService) { }

  ngOnInit() {
    this.getLevels();
    this.createSearchForm();


  }

  createSearchForm() {
    this.searchForm = this.fb.group({
      classId: [null, Validators.required],
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

  onFileChange(ev) {
    let workBook = null;
    let jsonData = null;
    const reader = new FileReader();
    const file = ev.target.files[0];
    reader.onload = (event) => {
      const data = reader.result;
      workBook = XLSX.read(data, { type: 'binary' });
      jsonData = workBook.SheetNames.reduce((initial, name) => {
        const sheet = workBook.Sheets[name];
        initial[name] = XLSX.utils.sheet_to_json(sheet);
        return initial;
      }, {});

      this.importedStudents = [];
      const d = jsonData;
      for (let i = 0; i < d.eleves.length; i++) {
        const la_ligne = d.eleves[i];
        const element: any = {};
        element.lastName = la_ligne.nom_Enfant;
        element.firstName = la_ligne.prenoms_Enfant,
          element.idnum = la_ligne.matricule_Enfant,
          element.phoneNumber = la_ligne.cellulaire_Enfant,
          element.secondPhoneNumber = la_ligne.second_contact_Enfant,
          element.email = la_ligne.email_Enfant;
        element.userTypeId = this.studentTypeId;
        element.sendEmail = false;

        element.parent = {
          lastName: la_ligne.nom_Parent,
          firstName: la_ligne.prenoms_Parent,
          phoneNumber: la_ligne.cellulaire_Parent,
          secondPhoneNumber: la_ligne.second_contact_Parent,
          email: la_ligne.email_Parent,
          sendEmail: false,
          userTypeId: this.parentTypeId
        };
        this.importedStudents = [...this.importedStudents, element];
      }

      this.showImport = true;
      // this.setDownload(dataString);
    };
    reader.readAsBinaryString(file);
  }

  getSelectedClassName() {
    this.className = this.classes.find(a => a.value === this.searchForm.value.classId).label;
  }

  getClasses() {
    const classLevelId = this.searchForm.value.classLevelId;
    this.classService.getClassesByLevelId(classLevelId).subscribe((response: Class[]) => {
      // this.classes = response;
      this.classes = [];
      for (let i = 0; i < response.length; i++) {
        const elt = response[i];
        const aclass = { value: elt.id, label: elt.name };
        this.classes = [...this.classes, aclass];
      }
    });
  }

  save() {
    this.waitDiv = true;
    const classId = this.searchForm.value.classId;
    this.classService.classStudentsAssignment(classId, this.importedStudents).subscribe(() => {
      this.alertify.success('enregistrement terminé...');
      this.allSelected = false;
      this.showImport = false;
      this.importedStudents = [];
      this.waitDiv = false;
    }, error => {
      this.alertify.error('oups... une erreur est survenue');
    });
  }

  selectAll() {
    this.allSelected = !this.allSelected;
    for (let i = 0; i < this.importedStudents.length; i++) {
      const element = this.importedStudents[i];
      element.sendEmail = this.allSelected;
    }
  }

}
