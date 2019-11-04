import { Component, OnInit } from '@angular/core';
import { SharedAnimations } from 'src/app/shared/animations/shared-animations';
import { Periodicity } from 'src/app/_models/periodicity';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-periodicities',
  templateUrl: './periodicities.component.html',
  styleUrls: ['./periodicities.component.scss'],
  animations: [SharedAnimations]
})
export class PeriodicitiesComponent implements OnInit {
  periodicities: Periodicity[];
  constructor(private route: ActivatedRoute) { }

  ngOnInit() {
    this.route.data.subscribe(data => {
      this.periodicities = data.periodicities;
    });
  }

}
