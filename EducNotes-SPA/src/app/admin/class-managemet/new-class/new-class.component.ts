import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { SharedAnimations } from 'src/app/shared/animations/shared-animations';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { ClassService } from 'src/app/_services/class.service';

@Component({
  selector: 'app-new-class',
  templateUrl: './new-class.component.html',
  styleUrls: ['./new-class.component.scss'],
  animations: [SharedAnimations]
})
export class NewClassComponent implements OnInit {
  @Input() levels: any;
  @Output() addClassResult = new EventEmitter();

  classTypes;
  classForm: FormGroup;
  errorMessage = '';
  submitText = 'enregistrer';
 suffixes = [{id: 1, name: 'A,B,C,...'}, {id: 2, name: '1,2,3,.....' }];


  constructor(private fb: FormBuilder, private classService: ClassService, private alertify: AlertifyService) { }

  ngOnInit() {
    this.createClassForm();
    this.getClassTypes();

  }
  getClassTypes() {
   this.classService.getClassTypes().subscribe((res) => {
     this.classTypes = res;
   });
  }
  createClassForm() {
    this.classForm = this.fb.group({
      levelId: [null, Validators.required],
      classTypeId: [null, Validators.nullValidator],
       name: [, Validators.nullValidator],
      suffixe: [null, Validators.nullValidator],
      maxStudent: [null, Validators.nullValidator],
      number: [null, Validators.nullValidator]});
  }

  save() {
    this.errorMessage = '';
    const classFromForm =  Object.assign({}, this.classForm.value);
    if (classFromForm.suffixe) {
      if (!classFromForm.number) {
        this.alertify.error('veuillez saisir le nombre de classe');
         } else {
           this.saveClass(classFromForm);
         }

    } else if (!classFromForm.name) {
      if (!classFromForm.suffixe) {
        this.alertify.error('veuillez saisir au moins le nom de la classe');
      } else {
        this.saveClass(classFromForm);
      }

    } else {
      this.saveClass(classFromForm);
    }


  }

  saveClass(element) {
    this.submitText = 'patientez...';

     this.classService.saveNewClasses(element).subscribe(next => {
        this.submitText = 'enregistrer';
        this.alertify.success('enregistrement terminé...');
        this.addClassResult.emit(true);

      }, error => {
        console.log(error);
        this.submitText = 'enregistrer';
        this.errorMessage = error;
      });
  }

  cancel() {
    this.addClassResult.emit(false);
  }


}
