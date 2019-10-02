 import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Class } from 'src/app/_models/class';
import { SharedAnimations } from 'src/app/shared/animations/shared-animations';

@Component({
  selector: 'app-level-classes',
  templateUrl: './level-classes.component.html',
  styleUrls: ['./level-classes.component.scss'],
  animations: [SharedAnimations]
})
export class LevelClassesComponent implements OnInit {

  classes: Class[];
  constructor(private route: ActivatedRoute) { }

  ngOnInit() {
    this.route.data.subscribe(data => {
      this.classes = data.classes;
   });
  }

}
