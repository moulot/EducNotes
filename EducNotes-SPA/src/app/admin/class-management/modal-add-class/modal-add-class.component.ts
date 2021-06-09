import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MDBModalRef } from 'ng-uikit-pro-standard';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { ClassService } from 'src/app/_services/class.service';

@Component({
  selector: 'app-modal-add-class',
  templateUrl: './modal-add-class.component.html',
  styleUrls: ['./modal-add-class.component.scss']
})
export class ModalAddClassComponent implements OnInit {
  @Output() reloadClassesModal = new EventEmitter();
  level: any;
  classForm: FormGroup;
  classTypeOptions = [];
  classTypes: any;
  needClassType = false;

  constructor(public activeModal: MDBModalRef, private classService: ClassService,
    private fb: FormBuilder, private alertify: AlertifyService) { }

  ngOnInit() {
    this.getClassTypes();
    this.createClassForm();
    if (this.level.id === 14 || this.level.id === 15 || this.level.id === 16) {
      this.needClassType = true;
    }
  }

  createClassForm() {
    this.classForm = this.fb.group({
      classTypeId: [null, Validators.required],
      maxStudents: [null, [ Validators.required, Validators.min(1) ]]
    });
  }

  getClassTypes() {
    this.classService.getClassTypes().subscribe(data => {
      this.classTypes = data;
      for (let i = 0; i < this.classTypes.length; i++) {
        const elt = this.classTypes[i];
        const type = { value: elt.id, label: elt.name };
        this.classTypeOptions = [...this.classTypeOptions, type];
      }
    });
  }

  addClass(levelId) {
    const classData = <any>{};
    classData.levelId = levelId;
    classData.maxStudent = this.classForm.value.maxStudents;
    if (this.needClassType) {
      classData.classTypeId = this.classForm.value.classTypeId;
    } else {
      classData.classTypeId = 0;
    }

    this.classService.addClassToLevel(classData).subscribe(() => {
      this.alertify.success('la classe a été ajoutée');
    }, () => {
      this.alertify.error('problème pour ajouter la classe');
    }, () => {
      this.reloadClassesModal.emit();
      this.activeModal.hide();
    });
  }

}
