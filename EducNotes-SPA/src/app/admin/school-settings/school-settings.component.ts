import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, FormArray } from '@angular/forms';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { ActivatedRoute } from '@angular/router';
import { Setting } from 'src/app/_models/setting';
import { AdminService } from 'src/app/_services/admin.service';

@Component({
  selector: 'app-school-settings',
  templateUrl: './school-settings.component.html',
  styleUrls: ['./school-settings.component.scss']
})
export class SchoolSettingsComponent implements OnInit {
  settingsForm: FormGroup;
  settingData: Setting[];
  updatedSettings: Setting[] = [];

  constructor(private fb: FormBuilder, private alertify: AlertifyService,
    private route: ActivatedRoute, private adminService: AdminService) { }

  ngOnInit() {
    this.createSettingForm();

    this.route.data.subscribe(data => {
      this.settingData = data['settings'];
      for (let i = 0; i < this.settingData.length; i++) {
        const elt = this.settingData[i];
        this.addSettingItem(elt.id, elt.name, elt.displayName, elt.value);
      }
    });
  }

  createSettingForm() {
    this.settingsForm = this.fb.group({
      settings: this.fb.array([])
    });
  }

  addSettingItem(id, name, displayName, value): void {
    const settings = this.settingsForm.get('settings') as FormArray;
    settings.push(this.createSettingItem(id, name, displayName, value));
  }

  createSettingItem(id, name, displayName, value): FormGroup {
    return this.fb.group({
      settingId: id,
      name: name,
      displayName: displayName,
      value: value
    });
  }

  saveSettings() {
    this.updatedSettings = [];
    for (let i = 0; i < this.settingsForm.value.settings.length; i++) {
      const elt = this.settingsForm.value.settings[i];
      const settingData = <Setting>{};
      settingData.id = elt.settingId;
      settingData.name = elt.name;
      settingData.displayName = elt.displayName;
      settingData.value = elt.value;
      this.updatedSettings = [...this.updatedSettings, settingData];
    }
    // console.log(this.updatedSettings);
    this.adminService.updateSettings(this.updatedSettings).subscribe(() => {
      this.alertify.success('les paramètres sont mis à jour!');
    }, error => {
      this.alertify.error(error);
    });
  }

}
