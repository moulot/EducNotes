import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { TresoService } from 'src/app/_services/treso.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { ActivatedRoute, Router } from '@angular/router';
import { SharedAnimations } from 'src/app/shared/animations/shared-animations';
import { Utils } from 'src/app/shared/utils';



@Component({
  selector: 'app-dead-line-form',
  templateUrl: './dead-line-form.component.html',
  styleUrls: ['./dead-line-form.component.scss'],
  animations: [SharedAnimations]
})
export class DeadLineFormComponent implements OnInit {
  deadLineForm: FormGroup;
  formModel: any;
  editMode = 'add';
  myDatePickerOptions = Utils.myDatePickerOptions;

  constructor(private tresoService: TresoService, private alertify: AlertifyService,
    private route: ActivatedRoute, private fb: FormBuilder, private router: Router) { }

  ngOnInit() {
    this.route.data.subscribe(data => {
      if (data.deadline) {
        this.formModel = data.deadline;
        this.editMode = 'edit';
      } else {
        this.initParams();
      }
    });

    this.createDeadLineForm();
  }

  initParams() {
    this.formModel = {
      name: '',
      comment: '',
      dueDate: null,
      percentage: null,
    };
  }

  createDeadLineForm() {
    this.deadLineForm = this.fb.group({
      name: [this.formModel.name, Validators.required],
      percentage: [this.formModel.percentage, Validators.required],
      comment: [this.formModel.comment],
      dueDate: [this.formModel.dueDate, Validators.required]
    });
  }
  save() {
    if (this.editMode === 'add') {
      this.createDealine();
    }
    if (this.editMode === 'edit') {
      this.editDeadline();
    }
  }

  createDealine() {
    const dataToSave =  Object.assign({}, this.deadLineForm.value);
    dataToSave.duedate = Utils.inputDateDDMMYY(dataToSave.dueDate, '/');
      this.tresoService.createDeadLine(dataToSave).subscribe(() => {
        this.alertify.success('enregistrement terminé..');
        this.router.navigate(['/deadLines']);
      }, error => {
        this.alertify.error(error);
      });
  }

  editDeadline() {
    const dataToSave =  Object.assign({}, this.deadLineForm.value);
    dataToSave.duedate = Utils.inputDateDDMMYY(dataToSave.dueDate, '/');
    this.tresoService.editDeadLine(this.formModel.id, dataToSave).subscribe(() => {
      this.alertify.success('modification  éffectuée..');
      this.router.navigate(['/deadLines']);
    }, error => {
      this.alertify.error(error);
    });
  }


}
