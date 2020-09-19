import { Component, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormGroup } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { TresoService } from 'src/app/_services/treso.service';

@Component({
  selector: 'app-tuition-fees',
  templateUrl: './tuition-fees.component.html',
  styleUrls: ['./tuition-fees.component.scss']
})
export class TuitionFeesComponent implements OnInit {
  productFees: any;
  levelProdForm: FormGroup;
  productOptions = [];

  constructor(private fb: FormBuilder, private alertify: AlertifyService, private router: Router,
    private route: ActivatedRoute, private tresoService: TresoService) { }

  ngOnInit() {
    this.createLevelProdForm();
    this.route.data.subscribe(data => {
      this.productFees = data['fees'];
      for (let i = 0; i < this.productFees.length; i++) {
        const elt = this.productFees[i];
        this.addLevelProdItem(elt.id, elt.productId, elt.classLevelId, elt.price);
      }
    });
  }

  createLevelProdForm() {
    this.levelProdForm = this.fb.group({
      levelprods: this.fb.array([])
    });
  }

  addLevelProdItem(id, prodid, levelid, price): void {
    const settings = this.levelProdForm.get('levelprods') as FormArray;
    settings.push(this.createLevelProdItem(id, prodid, levelid, price));
  }

  createLevelProdItem(id, prodid, levelid, price): FormGroup {
    return this.fb.group({
      id: id,
      prodid: prodid,
      levelid: levelid,
      price: price
    });
  }

  saveLevelProducts() {
    // this.updatedSettings = [];
    // for (let i = 0; i < this.settingsForm.value.settings.length; i++) {
    //   const elt = this.settingsForm.value.settings[i];
    //   const settingData = <Setting>{};
    //   settingData.id = elt.settingId;
    //   settingData.name = elt.name;
    //   settingData.displayName = elt.displayName;
    //   settingData.value = elt.value;
    //   this.updatedSettings = [...this.updatedSettings, settingData];
    // }
    // this.adminService.updateSettings(this.updatedSettings).subscribe(() => {
    //   this.alertify.success('les paramètres sont mis à jour!');
    //   this.router.navigate(['home']);
    // }, error => {
    //   this.alertify.error(error);
    // });
  }

}
