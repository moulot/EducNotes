import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-tuition-list',
  templateUrl: './tuition-list.component.html',
  styleUrls: ['./tuition-list.component.scss']
})
export class TuitionListComponent implements OnInit {
  tuitionList: any;
  invoiced: any;
  paid: any;
  totalTuitions: any;
  totalTuitionsOK: any;

  constructor(private route: ActivatedRoute) { }

  ngOnInit() {
    this.route.data.subscribe(data => {
      const listdata = data['list'];
      this.tuitionList = listdata.tuitionList;
      this.invoiced = listdata.totalInvoiced;
      this.paid = listdata.totalPaid;
      this.totalTuitions = listdata.totalTuitions;
      this.totalTuitionsOK = listdata.totalTuitionsOK;
    });
  }

}
