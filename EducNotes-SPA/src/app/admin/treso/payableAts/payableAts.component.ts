import { Component, OnInit } from '@angular/core';
import { PayableAt } from 'src/app/_models/payable-at';
import { ActivatedRoute } from '@angular/router';
import { SharedAnimations } from 'src/app/shared/animations/shared-animations';

@Component({
  selector: 'app-payableats',
  templateUrl: './payableAts.component.html',
  styleUrls: ['./payableAts.component.scss'],
  animations: [SharedAnimations]
})
export class PayableAtsComponent implements OnInit {
  payableAts: PayableAt[];
  constructor(private route: ActivatedRoute) { }

  ngOnInit() {
    this.route.data.subscribe(data => {
      this.payableAts = data.payableAts;
    });
  }

}
