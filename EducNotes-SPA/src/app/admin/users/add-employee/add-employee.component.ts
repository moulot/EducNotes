import { Component, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { env } from 'process';
import { Utils } from 'src/app/shared/utils';
import { AdminService } from 'src/app/_services/admin.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { AuthService } from 'src/app/_services/auth.service';
import { UserService } from 'src/app/_services/user.service';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-add-employee',
  templateUrl: './add-employee.component.html',
  styleUrls: ['./add-employee.component.scss']
})
export class AddEmployeeComponent implements OnInit {
  adminTypeId = environment.adminTypeId;
  employee: any;
  photoUrl = '';
  editionMode = false;
  wait = false;
  empForm: FormGroup;
  gender = [{value: 0, label: 'femme'}, {value: 1, label: 'homme'}];
  myDatePickerOptions = Utils.myDatePickerOptions;
  photoFile: File;
  roles: any;
  districts: any;
  maritalStatus: any;
  districtOptions = [];
  maritalStatusOptions = [];

  constructor(private adminService: AdminService, private fb: FormBuilder, private router: Router,
    private userService: UserService, private alertify: AlertifyService, private route: ActivatedRoute) { }

  ngOnInit() {
    this.route.data.subscribe(data => {
      this.employee = data['employee'];
      if (this.employee) {
        this.photoUrl = this.employee.photoUrl;
        this.editionMode = true;
      } else {
        this.initValues();
      }
      this.getEmpData();
    });

    this.createEmpForm();
  }

  createEmpForm() {
    this.empForm = this.fb.group({
      lastName: [this.employee.lastName, Validators.required],
      firstName: [this.employee.firstName, Validators.required],
      dateOfBirth: [this.employee.strDateOfBirth, Validators.required],
      gender: [this.employee.gender, Validators.required],
      maritalStatus: [this.employee.maritalStatusId, Validators.required],
      district: [this.employee.districtId, Validators.required],
      email: [this.employee.email, [Validators.required, Validators.email]],
      cell: [this.employee.phoneNumber, Validators.nullValidator],
      phone2: [this.employee.secondPhoneNumber, Validators.nullValidator],
      photoUrl: [this.employee.photoUrl],
      roles: this.fb.array([])
    }, {validator: this.formValidator});
  }

  formValidator(g: FormGroup) {
    return null;
  }

  addRoleItem(id, name, active): void {
    const courses = this.empForm.get('roles') as FormArray;
    courses.push(this.createRoleItem(id, name, active));
  }

  createRoleItem(id, name, active): FormGroup {
    return this.fb.group({
      roleid: id,
      name: name,
      active: active
    });
  }

  initValues() {
    this.employee = {
      id: 0,
      lastName: '',
      firstName: '',
      userName: '',
      phoneNumber: '',
      secondPhoneNumber: '',
      gender: null,
      maritalStatus: null,
      district: null,
      email: '',
      dateOfBirth: null,
      strDateOfBirth: '',
      photoUrl: '',
      photoFile: null,
      roleIds: '',
      active: 1
    };
  }

  getEmpData() {
    this.adminService.getEmpData().subscribe((data: any) => {
      this.roles = data.roles;
      this.districts = data.districts;
      this.maritalStatus = data.maritalStatus;

      if (this.employee.id === 0) {
        for (let i = 0; i < this.roles.length; i++) {
          const role = this.roles[i];
          this.addRoleItem(role.id, role.name, false);
        }
      } else {

      }

      for (let i = 0; i < this.districts.length; i++) {
        const district = this.districts[i];
        this.districtOptions = [...this.districtOptions, {value: district.id, label: district.name}];
      }

      for (let i = 0; i < this.maritalStatus.length; i++) {
        const status = this.maritalStatus[i];
        this.maritalStatusOptions = [...this.maritalStatusOptions, {value: status.id, label: status.name}];
      }
    }, () => {
      this.alertify.error('problème pour récupérer les données');
    });
  }

  imgResult(event) {
    let file: File = null;
    file = <File>event.target.files[0];

    // recuperation de l'url de la photo
    const reader = new FileReader();
    reader.onload = (e: any) => {
      this.photoUrl = e.target.result;
    };
    reader.readAsDataURL(event.target.files[0]);

    this.photoFile = file;
  }

  addEmployee() {
    this.wait = true;
    // get the selected roles for the teacher
    let ids = '';
    for (let i = 0; i < this.empForm.value.roles.length; i++) {
      const elt = this.empForm.value.roles[i];
      if (elt.active === true) {
        if (ids === '') {
          ids = elt.roleid.toString();
        } else {
          ids += ',' + elt.roleid.toString();
        }
      }
    }

    const formData = new FormData();
    if (this.photoFile) {
      formData.append('photoFile', this.photoFile, this.photoFile.name);
    }
    formData.append('id', this.employee.id.toString());
    formData.append('lastName', this.empForm.value.lastName);
    formData.append('firstName', this.empForm.value.firstName);
    formData.append('gender', this.empForm.value.gender);
    formData.append('maritalStatusId', this.empForm.value.maritalStatus);
    formData.append('districtId', this.empForm.value.district);
    formData.append('strDateOfBirth', this.empForm.value.dateOfBirth);
    formData.append('email', this.empForm.value.email);
    formData.append('phoneNumber', this.empForm.value.cell);
    formData.append('secondPhoneNumber', this.empForm.value.phone2);
    formData.append('roleIds', ids);
    formData.append('userTypeId', this.adminTypeId.toString());
    this.userService.addEmployee(formData).subscribe(() => {
      this.alertify.success('employé ajouté avec succès');
      this.wait = false;
      this.router.navigate(['/employees']);
    }, error => {
      this.alertify.error(error);
      this.wait = false;
    });
  }

}
