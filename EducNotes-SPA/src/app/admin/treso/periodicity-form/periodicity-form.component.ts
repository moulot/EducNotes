import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { SharedAnimations } from 'src/app/shared/animations/shared-animations';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { TresoService } from 'src/app/_services/treso.service';

@Component({
  selector: 'app-periodicity-form',
  templateUrl: './periodicity-form.component.html',
  styleUrls: ['./periodicity-form.component.scss'],
  animations: [SharedAnimations]
})
export class PeriodicityFormComponent implements OnInit {
  periodicityForm: FormGroup;
  formModel: any;
  editMode = 'add';

  constructor(private tresoService: TresoService, private alertify: AlertifyService,
    private fb: FormBuilder, private router: Router, private route: ActivatedRoute) { }

  ngOnInit() {

    this.route.data.subscribe(data => {
      if (data.periodicity) {
        this.formModel = data.periodicity;
        this.editMode = 'edit';
      } else {
        this.initParams();
      }
    });

    this.createPeriodicityForm();
  }

  initParams() {
    this.formModel = {
      name: ''
    };
  }

  createPeriodicityForm() {
    this.periodicityForm = this.fb.group({
      name: [this.formModel.name, Validators.required]
    });
  }

  save() {
    if (this.editMode === 'add') {
      this.createPeriodicity();
    }

    if (this.editMode === 'edit') {
      this.editPeriodicity();
    }
  }

  createPeriodicity() {
    this.tresoService.createPeriodicity(this.periodicityForm.value.name).subscribe(() => {
      this.alertify.success('ernegistrement terminé...');
      this.router.navigate(['/periodicities']);
    }, error => {
      this.alertify.error(error);
    });
  }

  editPeriodicity() {
    // const data = { id: this.formModel.id, name: this.periodicityForm.value.name };
    this.tresoService.EditPeriodicity(this.formModel.id, this.periodicityForm.value.name).subscribe(() => {
      this.alertify.success('modificication terminée...');
      this.router.navigate(['/periodicities']);
    }, error => {
      this.alertify.error(error);
    });
  }

}
