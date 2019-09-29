import { Component, OnInit } from '@angular/core';
import { AuthService } from 'src/app/_services/auth.service';
import { User } from 'src/app/_models/user';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { Router, ActivatedRoute } from '@angular/router';
import { UserService } from 'src/app/_services/user.service';
import { SharedAnimations } from 'src/app/shared/animations/shared-animations';

@Component({
  selector: 'app-confirm-email',
  templateUrl: './confirm-email.component.html',
  styleUrls: ['./confirm-email.component.css'],
  animations: [SharedAnimations]
})
export class ConfirmEmailComponent implements OnInit {
  user: User;
  // registerForm: FormGroup;
 // isReadOnly: boolean ;
 userTypes: any;


  constructor(private authService: AuthService, private fb: FormBuilder, private route: ActivatedRoute,
     private alertify: AlertifyService,  private router: Router, private userService: UserService) { }

  ngOnInit() {

    this.route.data.subscribe(data => {
      this.user = data['user'].user;
      });

    // this.userService.getUserTypes().subscribe( response => {
    //   this.userTypes = response;
    // });
      // this.createRegisterForm();
    }

    resultMode(val: boolean) {
        if (val) {
          this.alertify.success('votre mot de passe a bien été enregistré');
          this.router.navigate(['/Home']);
        } else {
          this.alertify.success('erreur survenue');

        }
      }

    // createRegisterForm() {
    //   this.registerForm = this.fb.group({
    //     userTypeId: ['0', Validators.required],
    //      phoneNumber: ['', Validators.required],
    //      secondPhoneNumber: ['', Validators.nullValidator]});
    // }
    // register() {
    //        // this.isReadOnly = true;
    //         if (this.registerForm.valid) {
    //           let u: User;
    //           u = Object.assign({}, this.registerForm.value);
    //           this.user.userTypeId = u.userTypeId;
    //           this.user.phoneNumber = u.phoneNumber;
    //           this.user.secondPhoneNumber = u.secondPhoneNumber;
    //           this.authService.finaliseRegistration(this.user).subscribe(next => {
    //              this.alertify.success('profile updated successfully!');
    //           this.router.navigate(['/members']);
    //           }, error => {
    //             this.alertify.error(error);
    //           });

    //         //   this.userService.updateUser(this.authService.decodedToken.nameid,
    //         //     this.user).subscribe(next => {
    //         //       this.authService.currentUser = this.user;
    //         //     this.alertify.success('profile updated successfully!');
    //         //   this.router.navigate(['/home']);

    //         //  //   this.editForm.reset(this.user);
    //         //   }, error => {
    //         //     this.alertify.error(error);
    //         //   });
    //          }

    //   }
}
