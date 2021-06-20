import { Component, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { FormatInputPathObject } from 'path';
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
  districts: any;
  districtOptions = [];
  wait = false;

  constructor(private adminService: AdminService, private alertify: AlertifyService,
    private fb: FormBuilder) { }

  ngOnInit() {
    this.createZonesForm();
    this.getZones();
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
        locations: this.addLocationItems(x.locations)
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
      name: [name, Validators.required],
      locations: this.fb.array([0])
    });
  }

  removeZoneItem(index) {
    const zones = this.zonesForm.get('zones') as FormArray;
    zones.removeAt(index);
  }

  addLocationItems(locations) {
    const arr = new FormArray([]);
    locations.forEach(x => {
      arr.push(this.fb.group({
        districtId: [x.districtId]
      }));
    });
    return arr;
  }

  addLocationItem(zoneIndex) {
    const zones = this.zonesForm.get('zones') as FormArray;
    const zone = zones.at(zoneIndex);
    const locations = zone.get('locations') as FormArray;
    locations.push(this.createLocationItem(0, ''));
  }

  createLocationItem(id, name) {
    return this.fb.group({
      districtId: [0]
    });
  }

  removeDistrictItem(zoneIndex, districtIndex) {
    const zones = this.zonesForm.get('zones') as FormArray;
    const zone = zones.at(zoneIndex);
    const locations = zone.get('locations') as FormArray;
    locations.removeAt(districtIndex);
  }

  addLocation(zoneIndex) {
    this.addLocationItem(zoneIndex);
  }

  addZone() {
    this.addZoneItem(0, '');
  }

  getZones() {
    this.adminService.getZones().subscribe((data: any) => {
      this.zones = data.zones;
      this.districts = data.districts;
      for (let i = 0; i < this.districts.length; i++) {
        const elt = this.districts[i];
        const district = {value: elt.id, label: elt.name + ' (' + elt.cityName + ')'};
        this.districtOptions = [...this.districtOptions, district];
      }
      this.addZoneItems();
    }, () => {
      this.alertify.error('problème pour récupérer les données');
    });
  }

  saveZones() {

  }

}
