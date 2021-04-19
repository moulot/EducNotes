import { Component, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Utils } from 'src/app/shared/utils';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { ClassService } from 'src/app/_services/class.service';

@Component({
  selector: 'app-config-activities',
  templateUrl: './config-activities.component.html',
  styleUrls: ['./config-activities.component.scss']
})
export class ConfigActivitiesComponent implements OnInit {
  activityForm: FormGroup;
  activities: any;

  constructor(private fb: FormBuilder, private alertify: AlertifyService,
    private classService: ClassService) { }

  ngOnInit() {
    this.createProductForm();
    this.getActivities();
  }

  createProductForm() {
    this.activityForm = this.fb.group({
      activities: this.fb.array([])
    });
  }

  addActivityItems() {
    const activities = this.activityForm.get('activities') as FormArray;
    this.activities.forEach(x => {
      activities.push(this.fb.group({
        id: x.id,
        name: x.name,
        abbrev: x.abbreviation,
        toBeDeleted: false
      }));
    });
  }

  addActivityItem(id, name, abbrev): void {
    const activities = this.activityForm.get('activities') as FormArray;
    activities.push(this.createActivityItem(id, name, abbrev));
  }

  createActivityItem(id, name, abbrev): FormGroup {
    return this.fb.group({
      id: [id],
      name: [name, Validators.required],
      abbrev: [abbrev, Validators.required],
      toBeDeleted: [false]
    });
  }

  removeActivitytem(index) {
    const activities = this.activityForm.get('activities') as FormArray;
    const id = activities.at(index).get('id').value;
    if (id === 0) {
      activities.removeAt(index);
    } else {
      activities.at(index).get('toBeDeleted').setValue(true);
    }
  }

  removeAllItems() {
    const activities = this.activityForm.get('activities') as FormArray;
    while (activities.length > 0) {
      activities.removeAt(0);
    }
  }

  resetActivitytem(index) {
    const activities = this.activityForm.get('activities') as FormArray;
    activities.at(index).get('toBeDeleted').setValue(false);
  }

  addActivity() {
    this.addActivityItem(0, '', '');
  }

  getActivities() {
    this.classService.getActivities().subscribe(data => {
      this.activities = data;
      this.addActivityItems();
    }, error => {
      this.alertify.error('problème pour récupérer les données.');
    });
  }

  save() {
    let activities = [];
    for (let i = 0; i < this.activityForm.value.activities.length; i++) {
      const activity = this.activityForm.value.activities[i];
      const item = {id: activity.id, name: activity.name, abbrev: activity.abbrev, toBeDeleted: activity.toBeDeleted};
      activities = [...activities, item];
    }
    this.classService.saveActivities(activities).subscribe(() => {
      Utils.smoothScrollToTop(40);
      this.removeAllItems();
      this.getActivities();
      this.alertify.success('les activités sont enregistrées.');
    }, error => {
      this.alertify.error('problème pour enregistrer les activités');
    });
}

}
