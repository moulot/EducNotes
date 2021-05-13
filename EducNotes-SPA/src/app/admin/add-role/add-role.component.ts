import { Component, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AdminService } from 'src/app/_services/admin.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-add-role',
  templateUrl: './add-role.component.html',
  styleUrls: ['./add-role.component.scss']
})
export class AddRoleComponent implements OnInit {
  adminTypeId = environment.adminTypeId;
  menu: any;
  roleForm: FormGroup;
  editionMode = false;
  wait = false;

  constructor(private adminService: AdminService, private alertify: AlertifyService, 
    private fb: FormBuilder, private router: Router) { }

  ngOnInit() {
    this.getMenuWithCapabilities();
    this.createRoleForm();
  }

  createRoleForm() {
    this.roleForm = this.fb.group({
      roleId: [0],
      roleName: ['', Validators.required],
      menuItems: this.fb.array([])
    });
  }

  addMenuItem(id, name, capabilities, children): void {
    const menuItems = this.roleForm.get('menuItems') as FormArray;
    menuItems.push(this.createMenuItem(id, name, capabilities, children));
  }

  createMenuItem(id, name, capabilities, children): FormGroup {
    return this.fb.group({
      id: id,
      name: name,
      capabilities: [capabilities],
      childItems: [children]
    });
  }

  getMenuWithCapabilities() {
    this.adminService.getMenuWithCapabilities(this.adminTypeId).subscribe(data => {
      this.menu = data;
      for (let i = 0; i < this.menu.length; i++) {
        const mnu = this.menu[i];
        // add accessFlag property to capabilities
        for (let j = 0; j < mnu.capabilities.length; j++) {
          mnu.capabilities[j].accessFlag = 0;
          // this.addCapabilityItem(i, elt.id, elt.name, elt.menuItemId, elt.accessType, 0);
        }
        this.addMenuItem(mnu.menuItemId, mnu.menuItemName, mnu.capabilities, mnu.childMenuItems);
      }
      console.log(this.menu);
    }, () => {
      this.alertify.error('problème pour récupérer les donéées');
    });
  }

  setMenuItemFlag(itemIndex, capIndex, flag) {
    this.menu[itemIndex].capabilities[capIndex].accessFlag = flag;
  }

  setChildFlag(itemIndex, childIndex, capIndex, flag) {
    this.menu[itemIndex].childMenuItems[childIndex].capabilities[capIndex].accessFlag = flag;
  }

  saveRole() {
    this.wait = true;
    const role = <any>{};
    role.roleId = this.roleForm.value.roleId;
    role.roleName = this.roleForm.value.roleName;
    role.capabilities = [];
    const roleid = this.roleForm.value.roleId;
    for (let i = 0; i < this.menu.length; i++) {
      const elt = this.menu[i];
      const rc = <any>{};
      for (let j = 0; j < elt.capabilities.length; j++) {
        const capability = elt.capabilities[j];
        rc.roleId = roleid;
        rc.capabilityId = capability.id;
        rc.accessFlag = capability.accessFlag;
        role.capabilities = [...role.capabilities, rc];
      }
      if (elt.childMenuItems.length > 0) {
        for (let k = 0; k < elt.childMenuItems.length; k++) {
          const child = elt.childMenuItems[k];
          const childRC = <any>{};
          for (let m = 0; m < child.capabilities.length; m++) {
            const cap = child.capabilities[m];
            childRC.roleId = roleid;
            childRC.capabilityId = cap.id;
            childRC.accessFlag = cap.accesFlag;
            role.capabilities = [...role.capabilities, childRC];
          }
        }
      }
    }

    this.adminService.saveRole(role).subscribe(() => {
      this.alertify.success('le rôle est bien enregistré');
      this.router.navigate(['/roles']);
      this.wait = false;
    }, () => {
      this.alertify.error('problème pour enregistrer le rôle');
      this.wait = false;
    });
  }

}
