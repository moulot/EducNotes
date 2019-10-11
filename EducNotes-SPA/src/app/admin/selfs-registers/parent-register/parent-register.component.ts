import { Component, OnInit, Input } from '@angular/core';
import { FormBuilder, FormGroup, Validators, FormControl } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { UserService } from 'src/app/_services/user.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { SharedAnimations } from 'src/app/shared/animations/shared-animations';
import { City } from 'src/app/_models/city';
import { District } from 'src/app/_models/district';
import { AuthService } from 'src/app/_services/auth.service';
import { environment } from 'src/environments/environment';
import { Utils } from 'src/app/shared/utils';
import { UserSpaCode } from 'src/app/_models/userSpaCode';
import { HttpClient } from '@angular/common/http';


@Component({
  selector: 'app-parent-register',
  templateUrl: './parent-register.component.html',
  styleUrls: ['./parent-register.component.scss'],
  animations: [SharedAnimations]
})
export class ParentRegisterComponent implements OnInit {

  @Input() user1: any;
  @Input() maxChild: number;
  // user2: User;
  userTypes: any;
  editModel: any;
  children: any = [];
  parents: any = [];
  levels: any;
  cities: City[];
  cityId: number;
  user1Disctricts: District[];
  user2Districts: District[];
 editionMode = 'add';
   i = 1;
  selectedIndex = 0;
  position = 'left';
  size = 'default';
  user1Form: FormGroup;
  // user2Form: FormGroup;
  childForm: FormGroup;
 errorMessage = [];
 phoneMask = [/\d/, /\d/, '-', /\d/, /\d/, '-', /\d/, /\d/, '-', /\d/, /\d/];
 birthDateMask = [/\d/, /\d/, '/', /\d/, /\d/, '/', /\d/, /\d/, /\d/, /\d/];
 products$;
 showChildenList = true;
 alerts;
 viewMode: 'list' | 'grid' = 'list';
 sameLocation: false;
  wait = false;
  page = 1;
  pageSize = 15;
  userId: number;
  user1NameExist = false;
  childNameExist = false;
  parentImg: any;
  userImgs: any[] = [];
  studentTypeId = environment.studentTypeId;
  parentTypeId = environment.parentTypeId;
  usersSpaCode: UserSpaCode[] = [];
  // selectedFiles: File[] = [];
  selectedFiles: any[] = [];
  url = '';



  constructor(private authService: AuthService, private fb: FormBuilder, private route: ActivatedRoute,
     private alertify: AlertifyService,  private router: Router, private userService: UserService,
     private http: HttpClient) { }

  ngOnInit() {
    this.userId = this.user1.id;
      this.createParentsForms();
      this.getCities();
      this.getLevels();
   }

   onFileSelected(event) {
    //  this.selectedFiles = <File>event.target.files[0];
    // this.savePhotos();
   }
   getCities() {
     this.authService.getAllCities().subscribe((res: City[]) => {
       this.cities = res;
     });
   }
   getLevels() {
     this.authService.getLevels().subscribe((res) => {
       this.levels = res;
     });
   }

   initializeParams(val: string) {
    this.editModel = {};
   this.editModel.lastName = val;
   this.editModel.dateOfBirth = null;
   this.editModel.firstName = '';
   this.editModel.gender = null;
   this.editModel.levelId = null;
   this.editModel.phoneNumber = '';
   this.editModel.email = '';
   this.editModel.secondPhoneNumber = '';
    }

    createParentsForms() {
      this.user1Form = this.fb.group({
        lastName: ['', Validators.nullValidator],
        firstName: ['', Validators.nullValidator],
        userName: ['', Validators.nullValidator],
        password: ['', Validators.required],
        checkPassword    : [ null, [ Validators.required, this.user1confirmationValidator ] ],
        gender: [null, Validators.required],
        dateOfBirth: [null, Validators.nullValidator],
        cityId: [null, Validators.nullValidator],
        districtId: [null, Validators.nullValidator],
        phoneNumber: ['', Validators.nullValidator],
        email: [this.user1.email, [Validators.nullValidator, Validators.email]],
        secondPhoneNumber: ['']});

    }

    createChildForm() {

      this.childForm = this.fb.group({
        dateOfBirth: [this.editModel.dateOfBirth, Validators.required],
        lastName: [this.editModel.lastName, Validators.required],
        userName: [this.editModel.userName, Validators.required],
        password: ['', Validators.required],
        checkPassword    : [ null, [ Validators.required, this.childrenconfirmationValidator ] ],
        firstName: [this.editModel.firstName, Validators.nullValidator],
        gender: [this.editModel.gender, Validators.required],
        levelId: [this.editModel.levelId, Validators.required],
        phoneNumber: [this.editModel.phoneNumber, Validators.nullValidator],
        email: [this.editModel.email, [Validators.nullValidator, Validators.nullValidator]],
        secondPhoneNumber: [this.editModel.secondPhoneNumber, Validators.nullValidator]});
    }

    user1confirmationValidator = (control: FormControl): { [ s: string ]: boolean } => {
      if (!control.value) {
        return { required: true };
      } else if (control.value !== this.user1Form.controls.password.value) {
        return { confirm: true, error: true };
      }
    }

    childrenconfirmationValidator = (control: FormControl): { [ s: string ]: boolean } => {
      if (!control.value) {
        return { required: true };
      } else if (control.value !== this.childForm.controls.password.value) {
        return { confirm: true, error: true };
      }
    }

    updateUser1ConfirmValidator(): void {
      /** wait for refresh value */
      Promise.resolve().then(() => this.user1Form.controls.checkPassword.updateValueAndValidity());
    }

    updateUser2ConfirmValidator(): void {
      /** wait for refresh value */
      Promise.resolve().then(() => this.user1Form.controls.checkPassword.updateValueAndValidity());
    }

    updateChilConfirmValidator(): void {
      /** wait for refresh value */
      Promise.resolve().then(() => this.user1Form.controls.checkPassword.updateValueAndValidity());
    }

  onStep1Next(e) {
    
  }

  onStep3Next(e) {
  //  this.emailsVerification();
  this.next();
  this.parents = [];
  this.parents = [...this.parents, this.user1];
  // this.parents = [...this.parents, this.user2];
}

  onComplete(e) {
    this.save();
   }

 submitChild(): void {
        let enfant: any;
         enfant = Object.assign({}, this.childForm.value);
         enfant.level = this.levels.find(item => item.id === enfant.levelId).name;
         if (this.editionMode === 'add') {
           enfant.spaCode = this.i;
          //  const image_line = this.userImgs.find( i => i.spaCode === this.i);
          //  if (image_line) {
          //   enfant.image = image_line.image.image;
          //  }
           this.children = [...this.children, enfant];
         } else {
           const itemIndex = this.children.findIndex(item => item.spaCode === this.selectedIndex);
           Object.assign(this.children[ itemIndex ], enfant);

         }

         this.close();

 }

 add() {
  this.editionMode = 'add';
  this.i = this.i + 1;
  this.initializeParams(this.user1Form.value.lastName);
  this.createChildForm();
  this.showChildenList = false;

}

 open() {
   this.showChildenList = !this.showChildenList;
 }

 close(): void {
   this.showChildenList = true;
 }

 cancel(): void {
   this.alertify.info('suppression annulée ');
 }

 back() {
  // this.showDetails = false;
 }

 next() {
   this.user1 = Object.assign({}, this.user1Form.value);
   this.user1.userTypeId = this.parentTypeId;
   this.user1.spaCode = 1;
  //  const img = this.userImgs.find(s => s.spaCode === 1);
  //  if (img) {
  //    this.user1.image = img.image.image;
  //  }

  //  const userBirthOfDate = this.user1Form.value.dateOfBirth;
  //  console.log(userBirthOfDate);
 }

 user1NameVerification() {
   const userName = this.user1Form.value.userName;
   this.user1NameExist = false;
   this.authService.userNameExist(userName).subscribe((res: boolean) => {
     if (res === true) {
        this.user1NameExist = true;
        // this.user1Form.valid = false;
     }
   });
 }
 childNameVerification() {
  const userName = this.childForm.value.userName;
  this.childNameExist = false;
  this.authService.userNameExist(userName).subscribe((res: boolean) => {
    if (res === true) {
       this.childNameExist = true;
      //  childForm.valid = false;
    }
  });
}


 confirm(element: any): void {
   this.children.splice(this.children.findIndex(p => p.id === element.id), 1);
   this.alertify.info('suppression éffectuée');
 }

 edit(element: any): void {
   this.selectedIndex = element.spaCodek;
  this.editModel = Object.assign({}, element);
  this.editionMode = 'edit';
  this.createChildForm();
   this.open();
 }

 parentImgResult(event) {
  // const img = {spaCode : 1, image: file};
  // this.userImgs = [...this.userImgs , img];
  // console.log(file);
  let file: File = null;
  file = <File>event.target.files[0];
  const img  = {spaCode : 1 , image : file};
  this.selectedFiles = [...this.selectedFiles, img];

  const reader = new FileReader();
  reader.readAsDataURL(file); // read file as data url

  reader.onload = (ev) => { // called once readAsDataURL is completed
    this.url = event.target.result;
  }
}

 childImgResult(event) {
  let file: File = null;
  file = <File>event.target.files[0];
  const img  = {spaCode : this.i , image : file};
  this.selectedFiles = [...this.selectedFiles, img];
}

save() {
  this.wait = true;

  const usersToSave: any = {};
  usersToSave.user1 = this.user1;
  const dt = usersToSave.user1.dateOfBirth;
  // formatage date de naissance du père
  usersToSave.user1.dateOfBirth = Utils.inputDateDDMMYY(dt, '/');

  // formatage date de naissance des enfants
    for (let i = 0; i < this.children.length; i++) {
      const elt = this.children[i];
      elt.dateOfBirth = Utils.inputDateDDMMYY(elt.dateOfBirth, '/');
      elt.userTypeId = this.studentTypeId;
    }
    usersToSave.children = this.children;

    // first Step : Enregistrement des Users
    this.authService.parentSelfInscription(this.userId, usersToSave).subscribe((res: UserSpaCode[]) => {
      this.usersSpaCode = res;
      this.savePhotos();
    }, error => {
        this.wait = false;
        this.alertify.error(error);
    });

}

savePhotos() {
  let errorCount = 0;
    let allCompleted = false;
    for (let i = 0; i < this.usersSpaCode.length; i++) {
    const element = this.usersSpaCode[i];
      const img = this.selectedFiles.find( c => c.spaCode === element.spaCode);
      if (img) {
        console.log('trouvé : ');
        // mise a jour de la photo
      //  debugger;
      const formData = new FormData();
      formData.append('file', img.image, img.image.name);
        this.authService.addUserPhoto(element.userId, formData).subscribe(() => {
          allCompleted = true;
        }, error => {
          this.alertify.error(error);
          errorCount = errorCount + 1;
          allCompleted = false;
        });
      }
     if (element === this.usersSpaCode[this.usersSpaCode.length - 1]) {
       this.logUser();
     }
      //fin de la boucle
    }

    if (allCompleted) {
      console.log('terminé...  : ');

      this.alertify.success('enregistrement terminé...');
      this.wait = false;
    }
}

 getUser1Districts() {
   const id = this.user1Form.value.cityId;
   this.user1Form.value.cityId = '';
   this.user1Disctricts = [];
   this.authService.getDistrictsByCityId(id).subscribe((res: District[]) => {
   this.user1Disctricts = res;
   }, error => {
     console.log(error);
   });
 }


 emailsVerification() {
   this.errorMessage = [];
   if (this.user1.email) {
     this.authService.emailExist(this.user1.email).subscribe((response: boolean) => {
         if (response === true) {
         this.errorMessage.push('l\'email du père est déja utilisé');
         }
     });

   }

   for (let i = 0; i < this.children.length; i++) {
     if (this.children[i].email) {
       this.authService.emailExist(this.children[i].email).subscribe((response: boolean) => {
           if (response === true) {
             const msg = 'l\'email de l\' enfant ' + this.children[i].lastName + ' ' + this.children[i].firstName + ' est déja utilisé';
           this.errorMessage.push(msg);
           }
       });

     }

   }
 }

 logUser() {
   const user = {userName : this.user1.userName , password : this.user1.password};
  this.authService.login(user).subscribe(() => {
    this.router.navigate(['/home']);
  }, error => {
    this.alertify.error(error);
  });
 }

}
