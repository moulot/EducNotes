import { Component, OnInit } from '@angular/core';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { ActivatedRoute, Router } from '@angular/router';
import { AdminService } from 'src/app/_services/admin.service';
import { SharedAnimations } from 'src/app/shared/animations/shared-animations';

@Component({
  selector: 'app-classes-panel',
  templateUrl: './classes-panel.component.html',
  styleUrls: ['./classes-panel.component.scss'],
  animations: [SharedAnimations]
})
export class ClassesPanelComponent implements OnInit {
  levels: any[];
  filledLevels: any[] = [];
  lev: any [] = [];
  // addNew = false;

  constructor(private adminService: AdminService, private route: ActivatedRoute,
    private alertify: AlertifyService) {}

  ngOnInit() {
    this.route.data.subscribe(data => {
      this.levels = data.levels;
      for (let i = 0; i < this.levels.length; i++) {
        const elt = this.levels[i];
        if (elt.classes.length > 0) {
          this.filledLevels = [... this.filledLevels, elt];
        }
      }
    });
    for (let index = 0; index < this.levels.length; index++) {
      const element: any = {
        id: this.levels[index].id,
        name: this.levels[index].name
      };
      this.lev = [...this.lev, element];
    }
  }

  newClass() {
    // this.addNew = !this.addNew;
  }

  getlevels() {
    this.levels = [];
    this.adminService.getClassLevelsDetails().subscribe((res: any[]) => {
      this.levels = res ;
      for (let index = 0; index < this.levels.length; index++) {
        const element: any = {
          id: this.levels[index].id,
          name: this.levels[index].name
        };
        this.lev = [...this.lev, element];
      }
    }, error => {
      console.log(error);
    }) ;
  }

  resultMode(val: boolean) {
    if (val) {
      this.getlevels();
    }
    // this.addNew = !this.addNew;
  }

}
