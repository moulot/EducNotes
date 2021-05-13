import { Component, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { AdminService } from 'src/app/_services/admin.service';
import { AlertifyService } from 'src/app/_services/alertify.service';

@Component({
  selector: 'app-config-menu',
  templateUrl: './config-menu.component.html',
  styleUrls: ['./config-menu.component.scss']
})
export class ConfigMenuComponent implements OnInit {
  menuItems: any;
  menuForm: FormGroup;

  constructor(private fb: FormBuilder, private adminService: AdminService, private alertify: AlertifyService) { }

  ngOnInit() {
    this.createMenuForm();
    this.getMenu();
  }

  createMenuForm() {
    this.menuForm = this.fb.group({
      menuItems: this.fb.array([])
    });
  }

  addMenuItem(id, name, parentMenuId, dsplSeq, isAlwaysEnabled, childItems): void {
    const menuItems = this.menuForm.get('menuItems') as FormArray;
    menuItems.push(this.createMenuItem(id, name, parentMenuId, dsplSeq, isAlwaysEnabled, childItems));
  }

  createMenuItem(id, name, parentMenuId, dsplSeq, isAlwaysEnabled, childItems): FormGroup {
    return this.fb.group({
      id: [id, Validators.required],
      name: [name, Validators.required],
      parentMenuId: [parentMenuId],
      dsplSeq: [dsplSeq, Validators.required],
      isAlwaysEnabled: [isAlwaysEnabled, Validators.required],
      childMenuItems: this.fb.array(childItems)
    });
  }

  removeChildItem(index) {
    const children = this.menuForm.get('menuItems') as FormArray;
    children.removeAt(index);
  }

  getMenu() {
    this.adminService.getUserTypeMenu(0).subscribe(data => {
      this.menuItems = data;
      for (let i = 0; i < this.menuItems.length; i++) {
        const item = this.menuItems[i];
        this.addMenuItem(item.id, item.name, item.parentMenuId, item.dsplSeq, item.isAlways, item.childMenuItems);
      }
    }, () => {
      this.alertify.error('problème pour récupérer les données');
    });
  }

  saveMenu() {

  }

}
