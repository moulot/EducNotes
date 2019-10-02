import { Component, OnInit } from '@angular/core';
import { AdminService } from 'src/app/_services/admin.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { ClassService } from 'src/app/_services/class.service';
import { FormGroup, FormBuilder, FormControl, Validators } from '@angular/forms';
import { User } from 'src/app/_models/user';
import { debounceTime } from 'rxjs/operators';
import { SharedAnimations } from 'src/app/shared/animations/shared-animations';
import { Class } from 'src/app/_models/class';
import { ToastrService } from 'ngx-toastr';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-inscriptions-list',
  templateUrl: './inscriptions-list.component.html',
  styleUrls: ['./inscriptions-list.component.scss'],
  animations: [SharedAnimations]
})
export class InscriptionsListComponent implements OnInit {

  constructor(private adminService: AdminService, private alertify: AlertifyService,
    private modalService: NgbModal, private fb: FormBuilder, private classService: ClassService,
     private toastr: ToastrService) { }
levels: any;
searchForm: FormGroup;
searchControl: FormControl = new FormControl();
showListDiv = false;
searchParams: any = {};
allSelected: boolean;
students: User[];
classes: Class[];
classId;
noResult: string;
viewMode: 'list' | 'grid' = 'list';
confirmResut;
submitText = 'valider';
filteredStudents: any[] = [];
selectedIds: any[] = [];
page = 1;
pageSize = 8;
className = '';




  ngOnInit() {
   this.getLevels();
   this.createSearchForm();
   this.searchControl.valueChanges.pipe(debounceTime(200)).subscribe(value => {
    this.filerData(value);
  });
  }

  getLevels() {
    this.classService.getLevels().subscribe(res => {
      this.levels = res;
    }, error => {
      console.log(error);
    });
  }
  createSearchForm() {
    this.searchForm = this.fb.group({
      levelId: ['', Validators.required],
      lastName: [''],
      firstName: ['']
    });
  }

  checkValid(content) {
    this.selectedIds = [];
    for (let index = 0; index < this.filteredStudents.length; index++) {
      if ( this.filteredStudents[index].isSelected) {
           // this.classIds = [...this.classIds, teachercourses.classes[index].classId];
           const  s = { id : Number(this.filteredStudents[index].id), userId :  Number(this.filteredStudents[index].userId)};
            this.selectedIds = [...this.selectedIds, s];
      }
    }

    if (this.selectedIds.length === 0) {
      this.toastr.error('Veuillez sélectionnez au moins un élève...', 'Erreur de saisie', { timeOut: 3000 });
    } else {

      // debugger;
      const room = this.classes.find(item => item.id === Number(this.classId));
      this.className = room.name;

      const diff = (room.maxStudent - room.totalStudent);

      if ( diff < this.selectedIds.length) {
        this.toastr.error('il reste seulement ' + diff + 'place(s) disponible(s) pour cette classe');
      } else {
        this.modalService.open(content, { ariaLabelledBy: 'confirmation', centered: true })
          .result.then((result) => {
          this.confirmResut = `Closed with: ${result}`;
          // this.toastr.info('validez', 'Erreur de saisie', { timeOut: 3000 });
          this.studentAffectation();
        }, (reason) => {

          this.confirmResut = `Dismissed with: ${reason}`;
          this.toastr.info('annuler', 'Erreur de saisie', { timeOut: 3000 });

        });
      }

    }

  }

  studentAffectation() {
    this.adminService.studentAffectation(this.classId, this.selectedIds).subscribe( () => {
      this.searchStudents();
      this.alertify.success('enegistrement terminé...');
      this.classId = null;

    }, error => {
      this.alertify.error(error);
    });
  }

  selectAll(e) {
    this.filteredStudents = this.filteredStudents.map(p => {
      p.isSelected = this.allSelected;
      return p;
    });

    if (this.allSelected) {

    }
  }

  resetSessions() {
    // this.filteredSessions = this.allSessions;
    this.searchForm.reset();
  }

  searchStudents() {
    this.showListDiv = false;
    this.allSelected = false;
    this.submitText = 'patienter...';
    this.noResult = '';
    this.students = [];

    this.searchParams = {};
    this.searchParams.levelId = Number(this.searchForm.value.levelId);
    this.searchParams.lastName = this.searchForm.value.lastName;
    this.searchParams.firstName = this.searchForm.value.firstName;

    this.adminService.searchIncription(this.searchParams).subscribe((res: any[])  => {
      if (res.length > 0) {
        this.students = res;
        this.filteredStudents = res;

        this.classService.getClassesByLevelId(this.searchParams.levelId).subscribe((response: Class[]) => {
          this.classes = response;
        });
      } else {
        this.noResult = 'Aucun élève trouvé...';
      }
    this.submitText = 'valider';
    this.showListDiv = true;


    }, error => {
      this.alertify.error(error);
      this.showListDiv = true;

    });

  }

  filerData(val) {
    if (val) {
      val = val.toLowerCase();
    } else {
      return this.students = [...this.students];
    }
    const columns = Object.keys(this.students[0]);
    if (!columns.length) {
      return;
    }

    const rows = this.students.filter(function(d) {
      for (let i = 0; i <= columns.length; i++) {
        const column = columns[i];
        if (d[column] && d[column].toString().toLowerCase().indexOf(val) > -1) {
          return true;
        }
      }
    });
    this.filteredStudents = rows;
  }

}
