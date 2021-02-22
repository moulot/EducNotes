import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { Utils } from 'src/app/shared/utils';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { AuthService } from 'src/app/_services/auth.service';
import { UserService } from 'src/app/_services/user.service';

@Component({
  selector: 'app-edit-account',
  templateUrl: './edit-account.component.html',
  styleUrls: ['./edit-account.component.scss']
})
export class EditAccountComponent implements OnInit {
  userid: any;
  user: any;
  userForm: FormGroup;
  wait = false;
  gender = [{value: 0, label: 'femme'}, {value: 1, label: 'homme'}];
  myDatePickerOptions = Utils.myDatePickerOptions;
  photoUrl = '';
  photoFile: File;

  constructor(private userService: UserService, private route: ActivatedRoute, private fb: FormBuilder,
    private alertify: AlertifyService, private authService: AuthService) { }

  ngOnInit() {
    this.route.params.subscribe(params => {
      this.userid = params['id'];
    });

    this.route.data.subscribe(data => {
      this.user = data['user'];
      if (this.user.photoUrl) {
        this.photoUrl = this.user.photoUrl;
      }
    });

    this.createUserForm();
  }

  createUserForm() {
    this.userForm = this.fb.group({
      lastName: [this.user.lastName, Validators.required],
      firstName: [this.user.firstName, Validators.required],
      dateOfBirth: [this.user.strDateOfBirth, Validators.required],
      gender: [this.user.gender, Validators.required],
      email: [this.user.email, [Validators.required, Validators.email]],
      cell: [this.user.phoneNumber, Validators.nullValidator],
      phone2: [this.user.secondPhoneNumber, Validators.nullValidator],
      photoUrl: [this.user.photoUrl]
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

  addUser() {

  }

}
