import { Component, OnInit } from '@angular/core';
import { UserService } from 'src/app/_services/user.service';
import { AlertifyService } from 'src/app/_services/alertify.service';

@Component({
  selector: 'app-user-types',
  templateUrl: './user-types.component.html',
  styleUrls: ['./user-types.component.css']
})
export class UserTypesComponent implements OnInit {
  userTypes: any;
  total = 0;
  totalActive = 0;
  typeName = '';
  typeId: number;
  visible = false;
  newTypeName = '';
  seledtedType: any;


  constructor(private userService: UserService, private alertify: AlertifyService) { }

  ngOnInit() {
    this.userService.getUserTypesDetails().subscribe((response: any) => {
      for (let i = 0; i < response.length; i++) {
        response[i].visible =  false;
        this.total = this.total + response[i].total;
        this.totalActive = this.totalActive + response[i].totalActive;
      }
      this.userTypes = response;
    });
  }

  cancelTypeEditing() {
    this.typeName = '';
    for (let i = 0; i < this.userTypes.length; i++) {
      const element = this.userTypes[i].visible = false;
    }
  }

  modifierType(typId: number ) {
    this.typeId = typId;
    const itemIndex = this.userTypes.findIndex(item => item.userType.id === typId);
    this.cancelTypeEditing();
    this.typeName =  this.userTypes[itemIndex].userType.name;
    this.userTypes[itemIndex].visible = true;

  }

  saveTypeEdition() {
    this.alertify.confirm('confirmez-vous cette modification ?', () => {
      this.userService.updateUserType(this.typeId, this.typeName).subscribe((response: any) => {
            const itemIndex = this.userTypes.findIndex(item => item.userType.id === this.typeId);
           this.userTypes[itemIndex].userType.name = this.typeName;
            this.cancelTypeEditing();
            this.alertify.success('modfification éffectuée...');
        this.visible = false;

          }, error => {
            console.log(error);
          });

      });
  }
  addNewType() {
    this.alertify.confirm('voulez-vous jouter cet element ?', () => {
      const data = {name: this.newTypeName};
      this.userService.addUserType(data).subscribe((response: any) => {
        const res = {userType : response, total : 0 , totalActive : 0};
        this.userTypes.unshift(res);
        console.log(res);
        this.alertify.success('enregistrment terminée...');
        this.newTypeName = '';
      });
      });

  }
  deleteUserType(id: number) {
    this.alertify.confirm('voulez-vous supprimer cet element ?', () => {
      this.userService.deleteUSerType(id).subscribe(next => {
        this.userTypes.splice(this.userTypes.findIndex(item => item.userType.id === id), 1);
        this.alertify.success('suppressioin effectuée...');
        this.newTypeName = '';
      });
      });

  }

}
