import { Component, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { Utils } from 'src/app/shared/utils';
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
    private fb: FormBuilder, private router: Router) { }

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
    let formNOK = false;
    const zones = g.get('zones').value;

    let namesNOK = false;
    let locNOK = false;
    for (let i = 0; i < zones.length; i++) {
      const zone = zones[i];
      // console.log(zone);
      const name = zone.name;
      if (name === '' || name === null) {
        formNOK = true;
        namesNOK = true;
      }
      const locations = zone.locations;
      for (let j = 0; j < locations.length; j++) {
        const loc = locations[j].districtId;
        // console.log('loc: ' + loc);
        if (loc === 0 || loc === null) {
          formNOK = true;
          locNOK = true;
        }
      }
    }

    return {'formNOK': formNOK, 'namesNOK': namesNOK, 'locNOK': locNOK};
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
      locations: this.addLocationItems([{districtId: 0}])
    });
  }

  removeZoneItem(index) {
    const zones = this.zonesForm.get('zones') as FormArray;
    zones.removeAt(index);
  }

  removeAllZones() {
    const zones = this.zonesForm.get('zones') as FormArray;
    while (zones.length > 0) {
      zones.removeAt(0);
    }
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

  reloadZones() {
    this.removeAllZones();
    this.adminService.getZones().subscribe((data: any) => {
      this.zones = data.zones;
      this.addZoneItems();
    }, () => {
      this.alertify.error('problème pour récupérer les données');
    });
  }

  saveZones() {
    this.wait = true;
    let zones = [];

    for (let i = 0; i < this.zonesForm.value.zones.length; i++) {
      const item = this.zonesForm.value.zones[i];
      const locations = item.locations;
      let districts = [];
      for (let j = 0; j < locations.length; j++) {
        const districtid = locations[j].districtId;
        const loc = {districtId: districtid};
        districts = [...districts, loc];
      }
      const zone = {id: item.id, name: item.name, locations: districts};
      zones = [...zones, zone];
    }

    this.adminService.saveZones(zones).subscribe(() => {
      this.reloadZones();
      this.wait = false;
      Utils.smoothScrollToTop(40);
      this.alertify.success('les zones ont été enregistrées');
    }, error => {
      this.alertify.error(error);
      this.wait = false;
    });
  }

}
