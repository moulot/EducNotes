import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { User } from 'src/app/_models/user';
import { NzMessageService } from 'ng-zorro-antd';
import { ClassService } from 'src/app/_services/class.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { UserService } from 'src/app/_services/user.service';
import { District } from 'src/app/_models/district';
import { City } from 'src/app/_models/city';
import { ProductService } from 'src/app/shared/services/product.service';
import { SharedAnimations } from 'src/app/shared/animations/shared-animations';
import { Router } from '@angular/router';




@Component({
  selector: 'app-inscription',
  templateUrl: './inscription.component.html',
  styleUrls: ['./inscription.component.scss'],
  animations: [SharedAnimations]
})
export class InscriptionComponent implements OnInit {
  father: User ;
  mother: User ;
  editModel: any;
  children: any = [];
  parents: any = [];
  levels: any[];
  cities: City[];
  cityId: number;
  motherDistricts: District[];
  fatherDistricts: District[];
 editionMode = 'add';
   i = 1;
  selectedIndex = 0;
  position = 'left';
  size = 'default';
  fatherForm: FormGroup;
  motherForm: FormGroup;
  childForm: FormGroup;
  submitText = 'enregistrer';
 errorMessage = [];
 phoneMask = [/\d/, /\d/, '-', /\d/, /\d/, '-', /\d/, /\d/, '-', /\d/, /\d/];
 products$;
 showChildenList = true;
 alerts;
 viewMode: 'list' | 'grid' = 'list';
 sameLocation: false;
  wait = false;
  page = 1;
  pageSize = 15;






  isCompleted: boolean;
  data: any = {
    email: ''
  };
  step2Form: FormGroup;

  constructor(private fb: FormBuilder, private nzMessageService: NzMessageService,
    private userService: UserService, private classService: ClassService, private alertify: AlertifyService,
     private productService: ProductService, private router: Router) { }

  ngOnInit() {
    this.products$ = this.productService.getProducts();
    this.initializeParams('');
    this.createParentsForms();
     this.createChildForm();
    this.classService.getLevels().subscribe((item: any[]) => {
      this.levels = item;
    }, error => {
      console.log(error);
    });
    this.userService.getAllCities().subscribe((res: City[]) => {
      this.cities = res;
    });
  }

  onStep1Next(e) {}
  onStep2Next(e) {
    this.next();
  }
  onStep3Next(e) {
    this.emailsVerification();
  this.parents = [];
  this.parents = [...this.parents, this.father];
  this.parents = [...this.parents, this.mother];


  }
  onComplete(e) {this.save(); }

  createParentsForms() {
    this.fatherForm = this.fb.group({
      lastName: ['', Validators.nullValidator],
      firstName: ['', Validators.nullValidator],
      cityId: [null, Validators.nullValidator],
      districtId: [null, Validators.nullValidator],
      phoneNumber: ['', Validators.nullValidator],
      email: ['', [Validators.nullValidator, Validators.email]],
      secondPhoneNumber: ['', Validators.nullValidator]});

      this.motherForm = this.fb.group({
        lastName: ['', Validators.nullValidator],
        firstName: ['', Validators.nullValidator],
        sameLocationWithFather: [false, Validators.nullValidator],
        cityId: [null, Validators.nullValidator],
        districtId: [null, Validators.nullValidator],
       phoneNumber: ['', Validators.nullValidator],
        email: ['', [Validators.nullValidator, Validators.email]],
        secondPhoneNumber: ['', Validators.nullValidator]});
  }


  createChildForm() {

    this.childForm = this.fb.group({
      dateOfBirth: [this.editModel.dateOfBirth, Validators.required],
       lastName: [this.editModel.lastName, Validators.required],
      firstName: [this.editModel.firstName, Validators.nullValidator],
      gender: [this.editModel.gender, Validators.required],
      levelId: [this.editModel.levelId, Validators.required],
      phoneNumber: [this.editModel.phoneNumber, Validators.nullValidator],
      email: [this.editModel.email, [Validators.nullValidator, Validators.nullValidator]],
      secondPhoneNumber: [this.editModel.secondPhoneNumber, Validators.nullValidator]});
  }

  add() {
    this.submitText = 'ENREGISTER';
    this.initializeParams(this.fatherForm.value.lastName);
    this.createChildForm();
    this.showChildenList = false;
    this.submitText = 'ajouter l\'enfant';

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
  submitChild(): void {
        let enfant: any;
          enfant = Object.assign({}, this.childForm.value);
          enfant.level = this.levels.find(item => item.id === enfant.levelId).name;
          if (this.editionMode === 'add') {
            this.i = this.i + 1;
            enfant.id = this.i;
            this.children = [...this.children, enfant];
          } else {
            const itemIndex = this.children.findIndex(item => item.id === this.selectedIndex);
            Object.assign(this.children[ itemIndex ], enfant);

          }

          this.close();

  }
  open() {
    this.showChildenList = !this.showChildenList;
  }

  close(): void {
    this.showChildenList = true;
  }
  cancel(): void {
    this.nzMessageService.info('suppression annulée');
  }
  back() {
   // this.showDetails = false;
  }
  next() {
    this.father = Object.assign({}, this.fatherForm.value);
    this.mother = Object.assign({}, this.motherForm.value);
    this.father.gender = 1;
    this.mother.gender = 0;
    this.emailsVerification();

  //  this.submitForm();
  }


  confirm(element: any): void {
    this.children.splice(this.children.findIndex(p => p.id === element.id), 1);
    this.nzMessageService.info('suppression éffectuée');
  }

  edit(element: any): void {
    this.submitText = 'enregistrer modifications';
    this.selectedIndex = element.id;
   this.editModel = Object.assign({}, element);
   this.editionMode = 'edit';
   this.createChildForm();
    this.open();
  }
  selectSameLacation(e) {
    if (this.motherForm.value.sameLocationWithFather === true) {
        if (this.fatherForm.value.cityId) {

          const val =  this.fatherForm.value.cityId;
          this.motherForm.patchValue({cityId: val});

          this.getMotherDistricts();
        }
        if (this.fatherForm.value.districtId) {
          const val =  this.fatherForm.value.districtId;
          this.motherForm.patchValue({districtId: val});
       }
    } else {
      this.motherForm.value.cityId = null;
      this.motherForm.value.districtId = null;

    }
  }
  save() {
    this.wait = true;
    const data: any = {};
    data.father = this.father;
    data.mother = this.mother;
    data.children = this.children;
    this.userService.savePreinscription(data).subscribe(() => {
        this.submitText = 'enregistrer';
        this.createParentsForms();
        this.alertify.success('enregistrement terminé...');
        this.router.navigateByUrl('/InscriptionComponent', {skipLocationChange: true}).then(() =>
         this.router.navigate(['/inscriptions']));
          this.wait = false;
          }, error => {
            this.wait = false;
            this.alertify.error(error);

    });
  }

  getMotherDistricts() {
    const id = this.motherForm.value.cityId;
    this.motherForm.value.cityId = '';
    this.motherDistricts = [];
    this.userService.getDistrictsByCityId(id).subscribe((res: District[]) => {
    this.motherDistricts = res;
    }, error => {
      console.log(error);
    });
  }

  getFatherDistricts() {
    const id = this.fatherForm.value.cityId;
    this.fatherForm.value.cityId = '';
    this.fatherDistricts = [];
    this.userService.getDistrictsByCityId(id).subscribe((res: District[]) => {
    this.fatherDistricts = res;
    }, error => {
      console.log(error);
    });
  }


  emailsVerification() {
    this.errorMessage = [];
    if (this.father.email) {
      this.userService.emailExist(this.father.email).subscribe((response: boolean) => {
          if (response === true) {
          this.errorMessage.push('l\'email du père est déja utilisé');
          }
      });

    }
    if (this.mother.email) {
      this.userService.emailExist(this.mother.email).subscribe((response: boolean) => {
          if (response === true) {
          this.errorMessage.push('l\'email de la mere est déja utilisé');
          }
      });

    }

    for (let i = 0; i < this.children.length; i++) {
      if (this.children[i].email) {
        this.userService.emailExist(this.children[i].email).subscribe((response: boolean) => {
            if (response === true) {
              const msg = 'l\'email de l\' enfant ' + this.children[i].lastName + ' ' + this.children[i].firstName + ' est déja utilisé';
            this.errorMessage.push(msg);
            }
        });

      }

    }
  }
}

