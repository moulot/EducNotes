import { Component, OnInit, Input, EventEmitter, Output } from '@angular/core';
import { ClassService } from 'src/app/_services/class.service';
import { AdminService } from 'src/app/_services/admin.service';
import * as XLSX from 'xlsx';
import { environment } from 'src/environments/environment';
import { AuthService } from 'src/app/_services/auth.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
// import { environment } from 'src/environments/environment';



@Component({
  selector: 'app-import-users',
  templateUrl: './import-users.component.html',
  styleUrls: ['./import-users.component.scss']
})
export class ImportUsersComponent implements OnInit {
  parentTypeId: number;
  confirmResut;
  wait = false;
  teacherTypeId: number;
  userTypeId: number;
  @Input() userTypes: any[] = [];
  @Output() toggleForm = new EventEmitter<boolean>();

  file: File = null;
  showExport = false;
  importedUsers: any[] = [];
  isCollapsed = true;
  currentUserId;

  constructor(private authService: AuthService, private alertify: AlertifyService, private modalService: NgbModal, private classService: ClassService, private adminService: AdminService) { }

  ngOnInit() {
    this.currentUserId = this.authService.currentUser.id;
    this.teacherTypeId = environment.teacherTypeId;
    this.parentTypeId = environment.parentTypeId;

  }
  // this.toggleForm.emit();
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

      this.importedUsers = [];
      const d = jsonData;
      if (this.userTypeId === this.teacherTypeId) {
        for (let i = 0; i < d.professeur.length; i++) {
          const la_ligne = d.professeur[i];
          const element: any = {};
          element.lastName = la_ligne.nom;
          element.firstName = la_ligne.prenoms,
            element.phoneNumber = la_ligne.contact1,
            element.secondPhoneNumber = la_ligne.contact2,
            element.userTypeId = this.userTypeId;
          element.email = la_ligne.email;
          this.importedUsers = [...this.importedUsers, element];
        }
      }

      if (this.userTypeId === this.parentTypeId) {
        for (let i = 0; i < d.parent.length; i++) {
          const la_ligne = d.parent[i];
          const element: any = {};
          element.lastName = la_ligne.nom;
          element.firstName = la_ligne.prenoms,
            element.phoneNumber = la_ligne.contact1,
            element.secondPhoneNumber = la_ligne.contact2,
            element.email = la_ligne.email;
          element.maxChild = la_ligne.nbre_enfant;
          element.userTypeId = this.userTypeId;
          this.importedUsers = [...this.importedUsers, element];
        }
      }
      this.showExport = true;
      // this.setDownload(dataString);
    };
    reader.readAsBinaryString(file);
  }
  hideDiv() {
    this.showExport = false;
  }
  cheichCollaped() {
    this.isCollapsed = !this.isCollapsed;
  }

  saveImportation() {
    this.wait = true;
    this.adminService.saveImportedUsers(this.importedUsers, this.currentUserId).subscribe(() => {
      this.alertify.success('enregistrement terminé...');
      this.showExport = false;
      this.wait = false;
      this.importedUsers = [];
    }, error => {
      this.alertify.error('Oups... Une erreur est servénue....');
    });
  }

  confirm(content) {
    this.modalService.open(content, { ariaLabelledBy: 'confirmation', centered: true })
      .result.then((result) => {
        this.confirmResut = `Closed with: ${result}`;
        this.saveImportation();
      }, (reason) => {
        this.confirmResut = `Dismissed with: ${reason}`;
      });
  }
}
