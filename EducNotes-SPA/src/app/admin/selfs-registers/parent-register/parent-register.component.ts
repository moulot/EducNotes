import { HttpClient } from '@angular/common/http';
import { Component, Input, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { SharedAnimations } from 'src/app/shared/animations/shared-animations';
import { Utils } from 'src/app/shared/utils';
import { City } from 'src/app/_models/city';
import { District } from 'src/app/_models/district';
import { UserSpaCode } from 'src/app/_models/userSpaCode';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { AuthService } from 'src/app/_services/auth.service';
import { UserService } from 'src/app/_services/user.service';
import { environment } from 'src/environments/environment';



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
  levels: any[] = [];
  cities: any[] = [];
  products: any[] = [];
  productsSelected: any[] = [];
  cityId: number;
  user1Disctricts: any[] = [];
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
  childPhotoUrl = '';
  parentPhotoUrl = '';
  sexe = [
    { value: 1, label: 'Homme' },
    { value: 0, label: 'Femme' }
  ];

  secondFormGroup: FormGroup;



  constructor(private authService: AuthService, private fb: FormBuilder, private route: ActivatedRoute,
    private alertify: AlertifyService, private router: Router, private userService: UserService,
    private http: HttpClient) { }

  ngOnInit() {
    console.log(this.maxChild);
    this.userId = this.user1.id;
    this.createParentsForms();
    this.getCities();
    this.getLevels();


    this.secondFormGroup = new FormGroup({
      active: new FormControl(null, Validators.required)
    });
  }

  onFileSelected(event) {
    //  this.selectedFiles = <File>event.target.files[0];
    // this.savePhotos();
  }
  getCities() {
    this.authService.getAllCities().subscribe((res: City[]) => {
      for (let i = 0; i < res.length; i++) {
        const element = { value: res[i].id, label: res[i].name };
        this.cities = [...this.cities, element];
      }

    });
  }
  getLevels() {
    this.authService.getLevels().subscribe((res: any[]) => {
      for (let i = 0; i < res.length; i++) {
        const element = { value: res[i].id, label: res[i].name };
        this.levels = [...this.levels, element];
      }
    });
  }

  initializeParams(val: string) {
    this.editModel = {};
    this.editModel.lastName = val;
    this.editModel.dateOfBirth = null;
    this.editModel.firstName = '';
    this.editModel.userName = '';
    this.editModel.password = '';
    this.editModel.checkPassword = '';
    this.editModel.gender = null;
    this.editModel.levelId = null;
    this.editModel.phoneNumber = '';
    this.editModel.productIds = [];
    this.editModel.email = '';
    this.editModel.secondPhoneNumber = '';
  }

  createParentsForms() {
    this.user1Form = this.fb.group({
      lastName: ['', Validators.required],
      firstName: ['', Validators.required],
      userName: [''],
      password: ['', Validators.required],
      checkPassword: [null, [Validators.required, this.user1confirmationValidator]],
      gender: [null, Validators.required],
      dateOfBirth: [''],
      cityId: [null, Validators.required],
      districtId: [null, Validators.required],
      phoneNumber: ['', Validators.required],
      email: [this.user1.email, [Validators.email]],
      secondPhoneNumber: ['']
    });

  }

  createChildForm() {

    this.childForm = this.fb.group({
      dateOfBirth: [this.editModel.dateOfBirth, Validators.required],
      lastName: [this.editModel.lastName, Validators.required],
      userName: [this.editModel.userName, Validators.required],
      password: [this.editModel.password, Validators.required],
      checkPassword: [this.editModel.checkPassword, [Validators.required, this.childrenconfirmationValidator]],
      firstName: [this.editModel.firstName, Validators.nullValidator],
      gender: [this.editModel.gender, Validators.required],
      productIds: [this.editModel.productIds],
      levelId: [this.editModel.levelId, Validators.required],
      phoneNumber: [this.editModel.phoneNumber, Validators.nullValidator],
      email: [this.editModel.email, [Validators.nullValidator, Validators.nullValidator]],
      secondPhoneNumber: [this.editModel.secondPhoneNumber, Validators.nullValidator]
    });
  }

  user1confirmationValidator = (control: FormControl): { [s: string]: boolean } => {
    if (!control.value) {
      return { required: true };
    } else if (control.value !== this.user1Form.controls.password.value) {
      return { confirm: true, error: true };
    }
  }

  childrenconfirmationValidator = (control: FormControl): { [s: string]: boolean } => {
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
    let enfant: any = {};
    enfant = Object.assign({}, this.childForm.value);
    for (let i = 0; i < enfant.productIds.length; i++) {
      const prod = this.products.find(a => a.value === enfant.productIds[i]);
      prod.id = prod.value;
      this.productsSelected = [...this.productsSelected, prod];
    }
    enfant.products = this.productsSelected;
    enfant.photoUrl = this.childPhotoUrl;
    enfant.level = this.levels.find(item => item.value === enfant.levelId).label;
    if (this.editionMode === 'add') {
      enfant.spaCode = this.i;
      this.children = [...this.children, enfant];
      this.secondFormGroup.patchValue({ active: 1 });
    } else {
      const itemIndex = this.children.findIndex(item => item.spaCode === this.selectedIndex);
      Object.assign(this.children[itemIndex], enfant);

    }

    this.close();

  }

  add() {
    this.childPhotoUrl = '';
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
    // this.alertify.info('suppression annulée ');
  }

  back() {
    // this.showDetails = false;
  }

  next() {
    this.user1 = Object.assign({}, this.user1Form.value);
    this.user1.userTypeId = this.parentTypeId;
    this.user1.spaCode = 1;
    this.user1.photoUrl = this.parentPhotoUrl;
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
    if (confirm('confirmez-vous cette suppression ?')) {
      this.children.splice(this.children.findIndex(p => p.id === element.id), 1);
      this.secondFormGroup.patchValue({ active: null });
      this.alertify.info('suppression éffectuée');
    }
  }

  edit(element: any): void {
    this.selectedIndex = element.spaCode;
    this.editModel = Object.assign({}, element);
    this.childNameExist = false;
    this.childPhotoUrl = element.photoUrl;
    this.editionMode = 'edit';
    this.createChildForm();
    this.getProducts();
    this.open();
  }

  parentImgResult(event) {
    let file: File = null;
    file = <File>event.target.files[0];

    // recuperation de l'url de la photo
    const reader = new FileReader();
    reader.onload = (e: any) => {
      this.parentPhotoUrl = e.target.result;
    };
    reader.readAsDataURL(event.target.files[0]);
    // fin recupération


    const imageExist = this.selectedFiles.find(t => t.spaCode === 1);
    if (imageExist) {
      // une image existe deja dans le tableau
      imageExist.image = file;
    } else {
      // aucune image dans le tableau => insertion
      const img = { spaCode: 1, image: file };
      this.selectedFiles = [...this.selectedFiles, img];
    }


  }

  childImgResult(event) {
    let file: File = null;
    file = <File>event.target.files[0];

    // recuperation de l'url de la photo
    const reader = new FileReader();
    reader.onload = (e: any) => {
      this.childPhotoUrl = e.target.result;
    };
    reader.readAsDataURL(event.target.files[0]);
    // fin recupération


    const imageExist = this.selectedFiles.find(t => t.spaCode === this.i);
    if (imageExist) {
      // une image existe deja dans le tableau
      imageExist.image = file;
    } else {
      // aucune image dans le tableau => insertion
      const img = { spaCode: this.i, image: file };
      this.selectedFiles = [...this.selectedFiles, img];
    }

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
    const firstStep = this.authService.parentSelfInscription(this.userId, usersToSave).subscribe((res: UserSpaCode[]) => {
      this.usersSpaCode = res;
      firstStep.unsubscribe();
      this.savePhotos();
    }, error => {
      this.wait = false;
      this.alertify.error(error);
    });

  }

  savePhotos() {
    let errorCount = 0;
    for (let i = 0; i < this.usersSpaCode.length; i++) {
      const element = this.usersSpaCode[i];
      const img = this.selectedFiles.find(c => c.spaCode === element.spaCode);
      if (img) {
        // console.log('trouvé : ');
        // mise a jour de la photo
        //  debugger;
        const formData = new FormData();
        formData.append('file', img.image, img.image.name);
        this.authService.addUserPhoto(element.userId, formData).subscribe(() => {
        }, error => {
          this.alertify.error(error);
          errorCount = errorCount + 1;
        });
      }
      if (element === this.usersSpaCode[this.usersSpaCode.length - 1]) {
      this.logUser();

      }
      // fin de la boucle
    }

  }

  getUser1Districts() {
    const id = this.user1Form.value.cityId;
    if (id) {
      this.user1Form.value.cityId = '';
      this.user1Disctricts = [];
      this.authService.getDistrictsByCityId(id).subscribe((res: any[]) => {
        for (let i = 0; i < res.length; i++) {
          const element = { value: res[i].id, label: res[i].name };
          this.user1Disctricts = [...this.user1Disctricts, element];
        }
      }, error => {
        console.log(error);
      });
    }
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
    const user = { userName: this.user1.userName, password: this.user1.password };

    setTimeout(() => {
      this.authService.login(user).subscribe(() => {
        this.router.navigate(['/home']);
      }, error => {
        this.alertify.error(error);
      }); }, 5000);
    // this.authService.login(user).subscribe(() => {
    //   this.router.navigate(['/home']);
    // }, error => {
    //   this.alertify.error(error);
    // });
  }

  getProducts() {
    this.products = [];
    this.productsSelected = [];
    const classLevelId = this.childForm.value.levelId;
    this.authService.getClassLevelProducts(classLevelId).subscribe((res: any[]) => {
      for (let i = 0; i < res.length; i++) {
        const element = res[i];
        element.value = element.id;
        element.label = element.details;
        if (element.isRequired === true) {
          this.productsSelected = [...this.productsSelected, element];
        } else {
          this.products = [...this.products, element];
        }
      }

    }, error => {
      this.alertify.error(error);
    });
  }

}
