import { Component, OnInit } from '@angular/core';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { ActivatedRoute } from '@angular/router';
import { SharedAnimations } from 'src/app/shared/animations/shared-animations';
import { ClassService } from 'src/app/_services/class.service';

@Component({
  selector: 'app-classes-panel',
  templateUrl: './classes-panel.component.html',
  styleUrls: ['./classes-panel.component.scss'],
  animations: [SharedAnimations]
})
export class ClassesPanelComponent implements OnInit {
  levels: any[];
  filledLevels = [];

  constructor(private classService: ClassService, private route: ActivatedRoute,
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
  }

  getlevels() {
    this.levels = [];
    this.filledLevels = [];
    this.classService.getClassLevelsDetails().subscribe((data: any) => {
      this.levels = data;
      for (let i = 0; i < this.levels.length; i++) {
        const elt = this.levels[i];
        if (elt.classes.length > 0) {
          this.filledLevels = [... this.filledLevels, elt];
        }
      }
    }, () => {
      this.alertify.error('problème pour récupérer les données');
    }) ;
  }

  resultMode(val: boolean) {
    if (val) {
      this.getlevels();
    }
    // this.addNew = !this.addNew;
  }

}
