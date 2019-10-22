import { Component, OnInit } from '@angular/core';
import { SharedAnimations } from 'src/app/shared/animations/shared-animations';
import { ClassService } from 'src/app/_services/class.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { TresoService } from 'src/app/_services/treso.service';

@Component({
  selector: 'app-class-level-products',
  templateUrl: './class-level-products.component.html',
  styleUrls: ['./class-level-products.component.scss'],
  animations :  [SharedAnimations]
})
export class ClassLevelProductsComponent implements OnInit {
  levels: any[] = [];
  services;
  show = false;
  totalAmount = 0;

levelId: number;
  constructor(private classService: ClassService, private alertify: AlertifyService
    , private tresoService: TresoService) { }

  ngOnInit() {
    this.getLevels();
  }

  getLevels() {
    this.classService.getLevels().subscribe((res: any[]) => {
      for (let i = 0; i < res.length; i++) {
        const element = {value: res[i].id, label : res[i].name} ;
       this.levels = [...this.levels, element];
      }

    }, error => {
      this.alertify.error(error);
    });
  }

  searchlvlServices() {
    this.show = false;
    this.totalAmount =0;
   this.tresoService.getClassLevelServices(this.levelId).subscribe((res: any []) => {
     for (let i = 0; i < res.length; i++) {
        res[i].productName = res[i].product.name ;
        this.totalAmount = this.totalAmount + res[i].amount;
     }
     this.services = res;
     this.show = true;
    }, error => {
     this.alertify.error(error);
   });
  }

}
