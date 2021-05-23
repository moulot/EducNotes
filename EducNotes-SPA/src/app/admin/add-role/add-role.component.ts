import { Component, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
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
  role: any;
  menu: any;
  roleForm: FormGroup;
  editionMode = false;
  wait = false;
  empsInRole: any;
  empsNotInRole: any;
  inRolePage = 1;
  inRolePageSize = 5;
  notInRolePage = 1;
  notInRolePageSize = 5;

  constructor(private adminService: AdminService, private alertify: AlertifyService,
    private fb: FormBuilder, private router: Router, private route: ActivatedRoute) { }

  ngOnInit() {
    this.route.data.subscribe(data => {
      this.role = data['role'];
      if (this.role) {
        this.editionMode = true;
      } else {
        this.initialValues();
      }
      this.getMenuWithCapabilities();
      this.getRoleEmployees(this.role.id);
    });
    this.createRoleForm();
  }

  initialValues() {
    this.role = {id: 0, name: ''};
  }

  createRoleForm() {
    this.roleForm = this.fb.group({
      roleId: [this.role.id],
      roleName: [this.role.name, Validators.required],
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

  // getEmployess() {
  //   this.adminService.getEmployees().subscribe(data => {
  //     this.employees = data;
  //   }, () => {
  //     this.alertify.error('problème pour récupérer les données');
  //   });
  // }

  getRoleEmployees(roleId) {
    this.adminService.getRoleEmployees(roleId).subscribe((data: any) => {
      this.empsInRole = data.usersInRole;
      this.empsNotInRole = data.usersNotInRole;
    }, () => {
      this.alertify.error('problème pour récupérer les données');
    });
  }

  moveToRole(empId) {
    const index = this.empsNotInRole.findIndex(u => u.id === empId);
    const userToMove = this.empsNotInRole[index];
    this.empsNotInRole.splice(index, 1);
    this.empsInRole = [...this.empsInRole, userToMove];
    this.empsInRole.sort((a,b) => a.lastName < b.lastName ? -1 : 1);
    this.empsInRole.sort((a,b) => a.firstName < b.firstName ? -1 : 1);
  }

  removeFromRole(empId) {
    const index = this.empsInRole.findIndex(u => u.id === empId);
    const userToMove = this.empsInRole[index];
    this.empsInRole.splice(index, 1);
    // console.log(this.empsNotInRole);
    this.empsNotInRole = [...this.empsNotInRole, userToMove];
    // console.log(this.empsNotInRole);
    this.empsNotInRole.sort((a,b) => a.lastName < b.lastName ? -1 : a.lastName > b.lastName ? 1 : 0);
    this.empsNotInRole.sort((a,b) => a.firstName < b.firstName ? -1 : a.firstName > b.firstName ? 1 : 0);
    // console.log(this.empsNotInRole);
  }

  getMenuWithCapabilities() {
    this.adminService.getMenuWithCapabilities(this.adminTypeId).subscribe(data => {
      this.menu = data;
      for (let i = 0; i < this.menu.length; i++) {
        const mnu = this.menu[i];
        // add accessFlag property to capabilities
        for (let j = 0; j < mnu.capabilities.length; j++) {
          const capability = mnu.capabilities[j];
          if (this.role.capabilities) {
            capability.accessFlag = this.getAccessFlag(capability.id);
          } else {
            capability.accessFlag = 0;
          }
        }

        if (mnu.childMenuItems) {
          for (let k = 0; k < mnu.childMenuItems.length; k++) {
            const child = mnu.childMenuItems[k];
            // add accessFlag property to capabilities
            for (let j = 0; j < child.capabilities.length; j++) {
              const capability = child.capabilities[j];
              if (this.role.capabilities) {
                capability.accessFlag = this.getAccessFlag(capability.id);
              } else {
                capability.accessFlag = 0;
              }
            }
          }
        }

        this.addMenuItem(mnu.menuItemId, mnu.menuItemName, mnu.capabilities, mnu.childMenuItems);
      }
    }, () => {
      this.alertify.error('problème pour récupérer les donéées');
    });
  }

  getAccessFlag(capabilityId) {
    const flagIndex = this.role.capabilities.findIndex(c => c.capabilityId === capabilityId);
    const accessFlag = flagIndex !== -1 ? this.role.capabilities[flagIndex].accessFlag : 0;
    return accessFlag;
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
    const roleid = this.roleForm.value.roleId;
    role.roleId = roleid;
    role.roleName = this.roleForm.value.roleName;
    role.usersInRole = this.empsInRole;
    role.capabilities = [];
    for (let i = 0; i < this.menu.length; i++) {
      const elt = this.menu[i];
      for (let j = 0; j < elt.capabilities.length; j++) {
        const rc = <any>{};
        const capability = elt.capabilities[j];
        if (capability.accessFlag > 0) {
          rc.roleId = roleid;
          rc.capabilityId = capability.id;
          rc.accessFlag = capability.accessFlag;
          role.capabilities = [...role.capabilities, rc];
        }
      }

      if (elt.childMenuItems.length > 0) {
        for (let k = 0; k < elt.childMenuItems.length; k++) {
          const child = elt.childMenuItems[k];
          for (let m = 0; m < child.capabilities.length; m++) {
            const childRC = <any>{};
            const cap = child.capabilities[m];
            if (cap.accessFlag > 0) {
              childRC.roleId = roleid;
              childRC.capabilityId = cap.id;
              childRC.accessFlag = cap.accessFlag;
              role.capabilities = [...role.capabilities, childRC];
            }
          }
        }
      }
    }

    this.adminService.saveRole(role).subscribe(() => {
      this.alertify.success('le rôle est bien enregistré');
      this.router.navigate(['/roles']);
      this.wait = false;
    }, error => {
      this.alertify.error('problème pour enregistrer le rôle');
      console.log(error);
      this.wait = false;
    });
  }

  goToRoles() {
    this.router.navigate(['/roles']);
  }

}
