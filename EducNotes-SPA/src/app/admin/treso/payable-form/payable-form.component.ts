import { Component, OnInit } from '@angular/core';
import { SharedAnimations } from 'src/app/shared/animations/shared-animations';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { TresoService } from 'src/app/_services/treso.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { Router, ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-payable-form',
  templateUrl: './payable-form.component.html',
  styleUrls: ['./payable-form.component.scss'],
  animations: [SharedAnimations]
})
export class PayableFormComponent implements OnInit {

  payableForm: FormGroup;
  formModel: any;
  editMode = 'add';

  constructor(private tresoService: TresoService, private alertify: AlertifyService,
    private fb: FormBuilder, private router: Router, private route: ActivatedRoute) { }

  ngOnInit() {

    this.route.data.subscribe(data => {
      if (data.payableAt) {
        this.formModel = data.payableAt;
        this.editMode = 'edit';
      } else {
        this.initParams();
      }
    });

    this.createPayableForm();
  }

  initParams() {
    this.formModel = {
      name: '',
      dayCount: null
    };
  }

  createPayableForm() {
    this.payableForm = this.fb.group({
      name: [this.formModel.name, Validators.required],
      dayCount: [this.formModel.dayCount]

    });
  }

  save() {
    if (this.editMode === 'add') {
      this.createPayableAt();
    }

    if (this.editMode === 'edit') {
      this.editPayableAt();
    }
  }

  createPayableAt() {
      this.tresoService.createPayableAt(this.payableForm.value).subscribe(() => {
        this.alertify.success('ernegistrement terminé...');
        this.router.navigate(['/payableAts']);
      }, error => {
        this.alertify.error(error);
      });
  }

  editPayableAt() {
    const data = { id: this.formModel.id, name: this.payableForm.value };
    this.tresoService.EditPayableAt(this.formModel.id, this.payableForm.value).subscribe(() => {
      this.alertify.success('modificication terminée...');
      this.router.navigate(['/payableAts']);
    }, error => {
      this.alertify.error(error);
    });
  }
}
