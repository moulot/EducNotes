import { Component, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { AdminService } from 'src/app/_services/admin.service';
import { AlertifyService } from 'src/app/_services/alertify.service';

@Component({
  selector: 'app-config-zones',
  templateUrl: './config-zones.component.html',
  styleUrls: ['./config-zones.component.scss']
})
export class ConfigZonesComponent implements OnInit {
  zonesForm: FormGroup;
  zones: any;
  wait = false;

  constructor(private adminService: AdminService, private alertify: AlertifyService,
    private fb: FormBuilder) { }

  ngOnInit() {
  }

  createZonesForm() {
    this.zonesForm = this.fb.group({
      zones: this.fb.array([])
    }, {validators: this.formValidator});
  }

  formValidator(g: FormGroup) {

    return false;
  }

  addZoneItems() {
    const zones = this.zonesForm.get('zones') as FormArray;
    this.zones.forEach(x => {
      zones.push(this.fb.group({
        id: x.id,
        name: x.name,
        locations: this.fb.array([])
      }));
    });
  }

  addZoneItem(id, name): void {
    const zones = this.zonesForm.get('zones') as FormArray;
    zones.push(this.createZoneItem(id, name));
  }

  createZoneItem(id, name): FormGroup {
    return this.fb.group({
      id: [id],
      name: [name, Validators.required]
    });
  }

  removeDueDatetem(index) {
    const dueDates = this.zonesForm.get('zones') as FormArray;
    dueDates.removeAt(index);
  }

  addDueDate() {
    this.addZoneItem(0, null);
  }

  saveZones() {

  }

}
