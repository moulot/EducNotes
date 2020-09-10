import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-tuition-list',
  templateUrl: './tuition-list.component.html',
  styleUrls: ['./tuition-list.component.scss']
})
export class TuitionListComponent implements OnInit {
  tuitionList: any;
  amount: any;
  amountOK: any;
  totalTuitions: any;
  totalTuitionsOK: any;

  constructor(private route: ActivatedRoute) { }

  ngOnInit() {
    this.route.data.subscribe(data => {
      const listdata = data['list'];
      this.tuitionList = listdata.tuitionList;
      this.amount = listdata.totalAmount;
      this.amountOK = listdata.totalAmountOK;
      this.totalTuitions = listdata.totalTuitions;
      this.totalTuitionsOK = listdata.totalTuitionsOK;
    });
  }

}
