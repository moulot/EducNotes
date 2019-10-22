import { Component, OnInit } from '@angular/core';
import { ClassService } from 'src/app/_services/class.service';
import { TresoService } from 'src/app/_services/treso.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { ClassLevel } from 'src/app/_models/classLevel';
import { environment } from 'src/environments/environment';
import { Product } from 'src/app/_models/product';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-class-level-prod-form',
  templateUrl: './class-level-prod-form.component.html',
  styleUrls: ['./class-level-prod-form.component.scss']
})
export class ClassLevelProdFormComponent implements OnInit {
  levels: any[] = [];
  services: any[] = [];
  schoolServiceId = environment.schoolServiceId;
  lvlProdForm: FormGroup;



  constructor(private classService: ClassService, private tresoService: TresoService, private router: Router,
    private alertify: AlertifyService, private fb: FormBuilder, private route: ActivatedRoute) { }

  ngOnInit() {
    this.getLevels();
    this.getServices();
    this.createProducForm();
  }

  createProducForm() {
    this.lvlProdForm = this.fb.group({
       classLevelIds: [, Validators.required],
       productId: [, Validators.required],
       amount: [, Validators.required]
      });
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

  getServices() {
   this.tresoService.getProductByType(this.schoolServiceId).subscribe((res: any[]) => {
    for (let i = 0; i < res.length; i++) {
      const element = {value: res[i].id, label : res[i].name};
      this.services = [...this.services, element];
    }
   }, error => {
     this.alertify.error(error);
   });
  }

  save() {
  this.tresoService.createlvlProduct(this.lvlProdForm.value).subscribe(() => {
    this.alertify.success('enregistrement terminée...');
    this.router.navigate(['/deadLines']);
  }, error => {
   this.alertify.error(error);
  });
  }

}
